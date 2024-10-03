using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

using DevMashup.Base;

namespace DevMashup {
	namespace GamePlay {
        /// <summary>
        /// Initilizes the world script, initilizes the mob manager,scriptible objects, generates basic terrain, creates the light, player,
        /// canvas, event system, containers....the 'brain' script behind all the generation  
        /// </summary>
		public class World : Singleton<World> {


            //PUBLICS & EXCEPTIONS
			[Header("World configuration")]
			public WorldConfig worldConfig; //Along with certain biome values, this generates terrain(Plus you have access to some pretty
            //awesome enum customasation)
            public ScriptableObjects scriptableObjects; //Holds all the data for all the bioms, what properties each block has, item sprites/spawning
            //prefab/mesh and extra setting, such as sunrise and sunset
            public DebugTest debugTest; //Extra Useful Info





			[System.Serializable]
            /// <summary>
            /// //Along with certain biome values, this generates terrain(Plus you have access to some pretty
            /// awesome enum customasation)
            /// </summary>
			public class WorldConfig {
                [Tooltip("The biomes the world should start as")]
                public InitialChunks startChunks;
                [Tooltip("Survival means you have health and take damage, creative means you''re imortal andhave total freedom")]
                public GameMode gameMode; //IMPLEMENT THIS
                [Tooltip("If you're testing on a certain biome(Mak sure DebugTest->Endless Terrain is false) and impleting a building you can choose this building her")]
                public Building building; //IMPLEMNT THIS


				[Tooltip("Seed of the world (0 seed generate randoms different worlds -testing-)")]
				[Range(int.MinValue + 100, int.MaxValue - 100)]
				public int worldSeed = 0;
				[Tooltip("Biomes sizes")]
				public float biomeScale = 150;

				[Header("Biome merge configuration")]
				[Tooltip("biomes.appearValue difference for merge")]
				[Range(0.01f, 0.5f)]
				public float diffToMerge = 0.025f;
				[Tooltip("Surface desired level, height where biomes merge")]
				[Range(1, Constants.MAX_HEIGHT)]
				public int surfaceLevel = 64;
				[Tooltip("Octaves used in the biome noise")]
				[Range(1, 5)]
				public int octaves = 5;
				[Tooltip("Amplitude decrease of biomes per octave,very low recommended")]
				[Range(0.001f, 1f)]
				public float persistance = 0.1f;
				[Tooltip("Frequency increase of biomes per octave")]
				[Range(1, 20)]
				public float lacunarity = 9f;
			}

            [System.Serializable]
            /// <summary>
            /// //Holds all the data for all the bioms, what properties each block has, item sprites/spawning
            /// prefab/mesh and extra setting, such as sunrise and sunset
            /// </summary>
            public class ScriptableObjects {
                public Extra extra;
                public Items items;
                public Blocks blocks;
                public Biome[] biomes;
            }

            [System.Serializable]
            /// <summary>
            /// //Extra Useful Info
            /// </summary>
            public class DebugTest {
                [Tooltip("Number of chunks of the view area")]
                [Range(1, 20)]
                public int testSize = 1;
                [Tooltip("Offset from the chunk (0,0), move the whole map generation")]
                public Vector2Int chunkOffset;
                public bool endlessTerrain = true;
                [Range(3, Constants.REGION_SIZE/2)][Tooltip("Chunks load and visible for the player,radius distance.")]
                public int chunkViewDistance = 10;
                [Range(0.1f, 0.6f)][Tooltip("Distance extra for destroy inactive chunks, this chunks consume ram, but load faster.")]
                public float chunkMantainDistance = 0.3f;
                [Tooltip("Use the camera position to calculate the player position. True-> use Camera.main tag / False-> use Player tag")]
                public bool useCameraPosition = false;
                [Tooltip("F4 to active. Show the current chunk and the data of the voxel you are looking. Important: You need activate gizmos in Game tab!!")]
                public bool debugMode = false;
                [Tooltip("Value from which the vertices are inside the figure")][Range(0, 255)]
                public int isoLevel = 128;
                [Tooltip("Allow to get a middle point between the voxel vertices in function of the weight of the vertices")]
                public bool interpolate = false;
            }



            //PRIVATES AND EXCEPTIONS
			private Dictionary<Vector2Int, Chunk> chunkDictOne = new Dictionary<Vector2Int, Chunk>();
            private World noiseManagerOne;
            private Region fakeRegionOne;//Used because chunks need a fahter region
			private Dictionary<Vector2Int, Chunk> chunkDictTwo = new Dictionary<Vector2Int, Chunk>();
            private Dictionary<Vector2Int, Region> regionDict = new Dictionary<Vector2Int, Region>();
            private List<Vector2Int> chunkLoadList = new List<Vector2Int>();

            private World noiseManagerTwo;
            private Transform player;
            private Vector3 lastPlayerPos;
            private int lastChunkViewDistance;
            private float hideDistance;
            private float removeDistance;
            private float loadRegionDistance;
            public enum InitialChunks { //The biomes the world should start as
                //Random,
                Artic_Islands,
                Savannah,
                Swamp
            }
            public enum GameMode { //Survival means you have health and take damage, creative means you''re imortal andhave total freedom
                Survival,
                Creative,
                Story_Mode
            }
            public enum Building { //If you're testing on a certain biome(Mak sure DebugTest->Endless Terrain is false) and impleting a building
            //you can choose this building here
                None,
            } 
            int biomeID;
            bool differentBiomes = false;



            GameObject esObj;
            /// <summary>
            /// Chunk Container, Mob Container, Item Container, Alligator Container, OTHEER container
            /// </summary>
            [HideInInspector]public GameObject[] Containers;
            [HideInInspector]public MobManager mobManager;
            [HideInInspector]public UI uiScript;
            [HideInInspector]public GameObject sunObj;
            [HideInInspector]public GameObject playerObj;
            [HideInInspector]public GameObject fpCamObj;
            [HideInInspector]public GameObject tpCamObj;
            [HideInInspector]public Player playerScript;
            [HideInInspector]public bool isGamePaused = false;
            [HideInInspector]public bool isGameOver = false;
            [HideInInspector]public bool isEnviromentPaused = false;


            [HideInInspector]public bool isStartingWorldDoneLoading = false;
            [HideInInspector]public byte[,,] terrainMap;
            [HideInInspector]public Vector3Int vectorValues;


			public void Start()
			{
                terrainMap = new byte[Constants.CHUNK_SIZE * 8, Constants.MAX_HEIGHT * 8, Constants.CHUNK_SIZE * 8];
                noiseManagerOne = World.Instance;
                fakeRegionOne = new Region(1000,1000);
                transform.name = "Normal World";
                CreateContainers();
                CreateScene();

                SetStartBiome();


                if(!debugTest.endlessTerrain) {
                    CreateStartWorld();
                }

				if (worldConfig.worldSeed == 0 && !WorldManager.IsCreated()) {//Generate random seed when use 0 and is scene testing (no WorldManager exists)
					Debug.Log("worldSeed 0 detected, generating random seed world");
					string selectedWorld = WorldManager.GetSelectedWorldName();
					WorldManager.DeleteWorld(selectedWorld);//Remove previous data
					WorldManager.CreateWorld(selectedWorld, worldConfig);//Create a new world folder for correct working
					worldConfig.worldSeed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				}
				else if((Constants.AUTO_CLEAR_WHEN_NOISE_CHANGE) && !WorldManager.IsCreated()) {//If AUTO_CLEAR_WHEN_NOISE_CHANGE true and world manager not exist, we clear old world data (we assume we are using a debug scene)
					string selectedWorld = WorldManager.GetSelectedWorldName();
					WorldConfig loadedWorldConfig = WorldManager.GetSelectedWorldConfig();
					//If worldConfig loaded is different to the current one, remove old data and save the new config
					if(loadedWorldConfig.worldSeed != worldConfig.worldSeed || loadedWorldConfig.biomeScale != worldConfig.biomeScale || loadedWorldConfig.diffToMerge != worldConfig.diffToMerge || loadedWorldConfig.surfaceLevel != worldConfig.surfaceLevel ||
						loadedWorldConfig.octaves != worldConfig.octaves || loadedWorldConfig.persistance != worldConfig.persistance || loadedWorldConfig.lacunarity != worldConfig.lacunarity) {
						WorldManager.DeleteWorld(selectedWorld);//Remove old world
						WorldManager.CreateWorld(selectedWorld, worldConfig);//Create new world with the new worldConfig
					}

				}
				else if(WorldManager.IsCreated()) {//Load config of the world
					worldConfig = WorldManager.GetSelectedWorldConfig();
				}


				if(scriptableObjects.biomes.Length == 0) {
					Biome[] biomeArray = GetComponents<Biome>();
					scriptableObjects.biomes = new Biome[biomeArray.Length];
					for (int i = 0; i< biomeArray.Length; i++) {
						scriptableObjects.biomes[i] = biomeArray[i];
						scriptableObjects.biomes[i].appearValue = (float)(biomeArray.Length-i) / biomeArray.Length;
					}
				}



                Initialize();


                //mobManager = new MobManager(this);
                //Cursor.lockState = CursorLockMode.Locked;
			}

            void Update() {
                //if(!isGamePaused && !isGameOver) {
                    if (lastChunkViewDistance != debugTest.chunkViewDistance) {
                        CalculateDistances();
                    }

                    HiddeRemoveChunk();

                    if (debugTest.endlessTerrain) {
                        CheckNewChunks();
                    }

                    LoadChunkFromList();
                    CheckRegion();
                    //if (Input.GetKeyDown(KeyCode.F4)) {
                    //    debugTest.debugMode = !debugTest.debugMode;
                    //}

                    if (chunkLoadList.Count == 0 && !isStartingWorldDoneLoading) {
                        isStartingWorldDoneLoading = true;
                        uiScript.gameObject.SetActive(true);
                    }


                    //scriptableObjects.extra.UpdateTime();
                //}
            }
            
            /// <summary>
            /// Containers[0] = Chunk Container, Containers[1] = Mob Container, Containers[2] = Item Container, Containers[3] = Alligator Container
            /// </summary>
            void CreateContainers() {
                Containers = new GameObject[5];

                string[] containerNames = {"GAMEPLAY", "Chunk Container", "Mob Container", "Item Container", "OTHER"};
                for(int i = 0; i < 5; i++) {
                    //Init Stuff
                    Containers[i] = new GameObject();
                    Containers[i].transform.name = containerNames[i];
                    Containers[i].transform.tag = "Container";
                    switch(i) {
                        case 0: //GAMEPLAY
                            Containers[i].transform.SetParent(null);
                            transform.SetParent(Containers[i].transform);
                            break;
                        case 1 or 2 or 3:
                            Containers[i].transform.SetParent(this.transform);
                            break;
                        case 4: //Other
                            Containers[i].transform.SetParent(null);
                            break;
                    }

                    //Containers[i].GetComponent<Transform>().position = new Vector3(0.0f, 0.0f, 0.0f);

                }
            }


            /// <summary>
            /// Initializes the world script as well as adds the other scence objects
            /// </summary>
            void CreateScene() {
                CreateItemSO();
                CreateObjects();
                InitWorldScript();
            }

            void InitWorldScript() {
                InitWorldConfig();
                InitScriptableObjects();
                InitDebugTest();
            }

            void InitWorldConfig() {
                worldConfig.worldSeed = 49652736;
                worldConfig.biomeScale = 150.0f;
                worldConfig.diffToMerge = 0.025f;
                worldConfig.surfaceLevel = 38;
                worldConfig.octaves = 5;
                worldConfig.persistance = 0.1f;
                worldConfig.lacunarity = 9;
            }

            void InitScriptableObjects() {
                CreateExtraSO();
                CreateBlockSO();
                CreateBiomesSO();
            }

            void CreateBlockSO() {
                scriptableObjects.blocks = Resources.Load<Blocks>(Sources.SO_BLOCKS);
                scriptableObjects.blocks.Atlas = Resources.Load<Material>(Sources.MAT_TERRAIN);
                scriptableObjects.blocks.blockTypes = new BlockTypes[9];
                for(int i = 0; i < 9; i++) {
                    scriptableObjects.blocks.blockTypes[i] = new BlockTypes();
                }

                CreateBlockTypes();
            }

            void CreateItemSO() {
                scriptableObjects.items = Resources.Load<Items>(Sources.SO_ITEMS);
                scriptableObjects.items.Prefab = Resources.Load<GameObject>(Sources.PREFAB_BASE_DROP);
                scriptableObjects.items.errorSprite = Resources.Load<Sprite>(Sources.TEX_ERROR_BLOCK_UI);
                scriptableObjects.items.itemInfo = new ItemInfo[12]; //How many UI items there re i the game

                for(int i = 0; i < 12; i++) {
                    scriptableObjects.items.itemInfo[i] = new ItemInfo();
                }
                CreateItems();
            }

            void CreateBlockTypes() {
                //Texture Order = top left to bottom right going across then down
                scriptableObjects.blocks.blockTypes[0].Name = "Bedrock";
                scriptableObjects.blocks.blockTypes[1].Name = "Grass";
                scriptableObjects.blocks.blockTypes[2].Name = "Sand";
                scriptableObjects.blocks.blockTypes[3].Name = "Snow";
                scriptableObjects.blocks.blockTypes[4].Name = "Dirt";
                scriptableObjects.blocks.blockTypes[5].Name = "Stone";
                scriptableObjects.blocks.blockTypes[6].Name = "Obsidian";
                scriptableObjects.blocks.blockTypes[7].Name = "Wood";
                scriptableObjects.blocks.blockTypes[8].Name = "Log";
            }

            void CreateItems() {
                scriptableObjects.items.itemInfo[0].Name = "Bedrock Block";
                scriptableObjects.items.itemInfo[0].itemMesh = Resources.Load<Mesh>(Sources.ITEM_BEDROCK_BLOCK);
                scriptableObjects.items.itemInfo[0].itemMat = Resources.Load<Material>(Sources.ITEM_BEDROCK_BLOCK);
                scriptableObjects.items.itemInfo[0].sprite = Resources.Load<Sprite>(Sources.TEX_BEDROCK_BLOCK_UI);

                scriptableObjects.items.itemInfo[1].Name = "Dirt Block";
                scriptableObjects.items.itemInfo[1].itemMesh = Resources.Load<Mesh>(Sources.ITEM_DIRT_BLOCK);
                scriptableObjects.items.itemInfo[1].itemMat = Resources.Load<Material>(Sources.ITEM_DIRT_BLOCK);
                scriptableObjects.items.itemInfo[1].sprite = Resources.Load<Sprite>(Sources.TEX_DIRT_BLOCK_UI);
                
                scriptableObjects.items.itemInfo[2].Name = "Day Stone";
                scriptableObjects.items.itemInfo[2].itemMesh = Resources.Load<Mesh>(Sources.ITEM_DAY_STONE);
                scriptableObjects.items.itemInfo[2].itemMat = Resources.Load<Material>(Sources.ITEM_DAY_STONE);
                scriptableObjects.items.itemInfo[2].sprite = Resources.Load<Sprite>(Sources.TEX_DAY_STONE_UI);

                scriptableObjects.items.itemInfo[3].Name = "Grass Block";
                scriptableObjects.items.itemInfo[3].itemMesh = Resources.Load<Mesh>(Sources.ITEM_GRASS_BLOCK);
                scriptableObjects.items.itemInfo[3].itemMat = Resources.Load<Material>(Sources.ITEM_GRASS_BLOCK);
                scriptableObjects.items.itemInfo[3].sprite = Resources.Load<Sprite>(Sources.TEX_GRASS_BLOCK_UI);

                scriptableObjects.items.itemInfo[4].Name = "Log Block";
                scriptableObjects.items.itemInfo[4].itemMesh = Resources.Load<Mesh>(Sources.ITEM_LOG_BLOCK);
                scriptableObjects.items.itemInfo[4].itemMat = Resources.Load<Material>(Sources.ITEM_LOG_BLOCK);
                scriptableObjects.items.itemInfo[4].sprite = Resources.Load<Sprite>(Sources.TEX_LOG_BLOCK_UI);

                scriptableObjects.items.itemInfo[5].Name = "Mutation Remote";
                scriptableObjects.items.itemInfo[5].itemMesh = Resources.Load<Mesh>(Sources.ITEM_MUTATION_REMOTE);
                scriptableObjects.items.itemInfo[5].itemMat = Resources.Load<Material>(Sources.ITEM_MUTATION_REMOTE);
                scriptableObjects.items.itemInfo[5].sprite = Resources.Load<Sprite>(Sources.TEX_MUTATION_REMOTE_UI);

                scriptableObjects.items.itemInfo[6].Name = "Obsidian Block";
                scriptableObjects.items.itemInfo[6].itemMesh = Resources.Load<Mesh>(Sources.ITEM_OBSIDIAN_BLOCK);
                scriptableObjects.items.itemInfo[6].itemMat = Resources.Load<Material>(Sources.ITEM_OBSIDIAN_BLOCK);
                scriptableObjects.items.itemInfo[6].sprite = Resources.Load<Sprite>(Sources.TEX_OBSIDIAN_BLOCK_UI);

                scriptableObjects.items.itemInfo[7].Name = "Sand Block";
                scriptableObjects.items.itemInfo[7].itemMesh = Resources.Load<Mesh>(Sources.ITEM_SAND_BLOCK);
                scriptableObjects.items.itemInfo[7].itemMat = Resources.Load<Material>(Sources.ITEM_SAND_BLOCK);
                scriptableObjects.items.itemInfo[7].sprite = Resources.Load<Sprite>(Sources.TEX_SAND_BLOCK_UI);

                scriptableObjects.items.itemInfo[8].Name = "Snow Block";
                scriptableObjects.items.itemInfo[8].itemMesh = Resources.Load<Mesh>(Sources.ITEM_SNOW_BLOCK);
                scriptableObjects.items.itemInfo[8].itemMat = Resources.Load<Material>(Sources.ITEM_SNOW_BLOCK);
                scriptableObjects.items.itemInfo[8].sprite = Resources.Load<Sprite>(Sources.TEX_SNOW_BLOCK_UI);

                scriptableObjects.items.itemInfo[9].Name = "Stone Block";
                scriptableObjects.items.itemInfo[9].itemMesh = Resources.Load<Mesh>(Sources.ITEM_STONE_BLOCK);
                scriptableObjects.items.itemInfo[9].itemMat = Resources.Load<Material>(Sources.ITEM_STONE_BLOCK);
                scriptableObjects.items.itemInfo[9].sprite = Resources.Load<Sprite>(Sources.TEX_STONE_BLOCK_UI);

                scriptableObjects.items.itemInfo[10].Name = "Uncooked Alligator Meat";
                scriptableObjects.items.itemInfo[10].itemMesh = Resources.Load<Mesh>(Sources.ITEM_UNCOOKED_ALLIGATOR_MEAT);
                scriptableObjects.items.itemInfo[10].itemMat = Resources.Load<Material>(Sources.ITEM_UNCOOKED_ALLIGATOR_MEAT);
                scriptableObjects.items.itemInfo[10].sprite = Resources.Load<Sprite>(Sources.TEX_UNCOOKED_ALLIGATOR_MEAT_UI);

                scriptableObjects.items.itemInfo[11].Name = "Wood Block";
                scriptableObjects.items.itemInfo[11].itemMesh = Resources.Load<Mesh>(Sources.ITEM_WOOD_BLOCK);
                scriptableObjects.items.itemInfo[11].itemMat = Resources.Load<Material>(Sources.ITEM_WOOD_BLOCK);
                scriptableObjects.items.itemInfo[11].sprite = Resources.Load<Sprite>(Sources.TEX_WOOD_BLOCK_UI);
            }

            void CreateBiomesSO() {
                scriptableObjects.biomes = new Biome[3];
                for(int i = 0; i < 3; i++) {
                    scriptableObjects.biomes[i] = (Biome)ScriptableObject.CreateInstance("Biome");
                }

                scriptableObjects.biomes[0] = Resources.Load<Biome>(Sources.SO_BIOME_ARTIC_ISLANDS);
                scriptableObjects.biomes[1] = Resources.Load<Biome>(Sources.SO_BIOME_SAVANNAH);
                scriptableObjects.biomes[2] = Resources.Load<Biome>(Sources.SO_BIOME_SWAMP);

                CreateBiomeData();
            }

            void CreateBiomeData() {
                ArticIslandsData();
                SavannahData();
                SwampData();
            }

            void ArticIslandsData() {
                scriptableObjects.biomes[0].Name = "Artic Islands";
                scriptableObjects.biomes[0].appearValue = 1.0f;


                scriptableObjects.biomes[0].maxHeightDifference = 16;
                scriptableObjects.biomes[0].snowDeep = 16;
                scriptableObjects.biomes[0].iceNoiseScale = 40;
                scriptableObjects.biomes[0].iceApearValue = 0.8f;
                scriptableObjects.biomes[0].iceMaxHeight = 5;
                scriptableObjects.biomes[0].IcePersistance = 0.5f;
                scriptableObjects.biomes[0].IceLacunarity = 2;

                scriptableObjects.biomes[0].terrainHeightCurve = AnimationCurve.Linear(0,0,1,1);
                scriptableObjects.biomes[0].scale = 50;
                scriptableObjects.biomes[0].octaves = 4;
                scriptableObjects.biomes[0].persistance = 0.5f;
                scriptableObjects.biomes[0].lacunarity = 2f;

                scriptableObjects.biomes[0].firstLayerOfBlocks = 4;
                scriptableObjects.biomes[0].secondLayerOfBlocks = 5;
                scriptableObjects.biomes[0].thirdLayerOfBlocks = 6;
            }

            void SavannahData() {
                scriptableObjects.biomes[1].Name = "Savannah";
                scriptableObjects.biomes[1].appearValue = 0.5f;


                scriptableObjects.biomes[1].maxHeightDifference = 16;
                scriptableObjects.biomes[1].snowDeep = 16;
                scriptableObjects.biomes[1].iceNoiseScale = 40;
                scriptableObjects.biomes[1].iceApearValue = 0.8f;
                scriptableObjects.biomes[1].iceMaxHeight = 5;
                scriptableObjects.biomes[1].IcePersistance = 0.5f;
                scriptableObjects.biomes[1].IceLacunarity = 2;

                scriptableObjects.biomes[1].terrainHeightCurve = AnimationCurve.Linear(0,0,1,1);
                scriptableObjects.biomes[1].scale = 50;
                scriptableObjects.biomes[1].octaves = 4;
                scriptableObjects.biomes[1].persistance = 0.5f;
                scriptableObjects.biomes[1].lacunarity = 2f;

                scriptableObjects.biomes[1].firstLayerOfBlocks = 6;
                scriptableObjects.biomes[1].secondLayerOfBlocks = 6;
                scriptableObjects.biomes[1].thirdLayerOfBlocks = 6;
            }

            void SwampData() {
                scriptableObjects.biomes[2].Name = "Swamp";
                scriptableObjects.biomes[2].appearValue = 0f;


                scriptableObjects.biomes[2].maxHeightDifference = 5;
                scriptableObjects.biomes[2].snowDeep = 5;
                scriptableObjects.biomes[2].iceNoiseScale = 40;
                scriptableObjects.biomes[2].iceApearValue = 0.8f;
                scriptableObjects.biomes[2].iceMaxHeight = 5;
                scriptableObjects.biomes[2].IcePersistance = 0.5f;
                scriptableObjects.biomes[2].IceLacunarity = 2;

                scriptableObjects.biomes[2].terrainHeightCurve = AnimationCurve.Linear(0,0,1,1);
                scriptableObjects.biomes[2].scale = 50;
                scriptableObjects.biomes[2].octaves = 4;
                scriptableObjects.biomes[2].persistance = 0.5f;
                scriptableObjects.biomes[2].lacunarity = 2f;

                scriptableObjects.biomes[2].firstLayerOfBlocks = 4;
                scriptableObjects.biomes[2].secondLayerOfBlocks = 4;
                scriptableObjects.biomes[2].thirdLayerOfBlocks = 3;
            }

            void CreateExtraSO() {
                scriptableObjects.extra = Resources.Load<Extra>(Sources.SO_EXTRA);
                scriptableObjects.extra.Equinoxes = new Equinox[1];
                scriptableObjects.extra.Equinoxes[0] = new Equinox();

                ExtraData();
            }

            void ExtraData() {
                scriptableObjects.extra.Equinoxes[0].Name = "Spring";
                scriptableObjects.extra.Equinoxes[0].sunRise = 442; //The time(in minutes) on the spring equinox in kansas city that the sun rose
                scriptableObjects.extra.Equinoxes[0].sunSet = 1169; //The time(in minutes) on the spring equinox in kansas city that the sun set
            
                scriptableObjects.extra.HordeNightFrequency = 7; //Setting the nights which this mobs span
                scriptableObjects.extra.Day = 1;
                scriptableObjects.extra.Clock = uiScript.dayText.GetComponent<TextMeshProUGUI>();
                scriptableObjects.extra.ClockSpeed = 0.2f; //How fast things occur in the game
                scriptableObjects.extra.TimeOfDay = 776;
            }


            void InitDebugTest() {
                if(!debugTest.endlessTerrain) {
                    debugTest.testSize = 2;
                }
                else {
                    debugTest.testSize = 10;
                }
                debugTest.chunkViewDistance = 10;
                debugTest.chunkMantainDistance = 0.3f;
                debugTest.useCameraPosition = false;
                debugTest.debugMode = false;
                debugTest.isoLevel = 128;
                debugTest.interpolate = true;
                
            }

            void CreateObjects() {
                CreateObject("Sun");
                CreateObject("Event System");
                CreateObject("UI");
                CreateObject("Player");
            }

            public void CreateObject(string objName) {
                switch(objName) {
                    case "Sun":
                        //Init
                        sunObj = new GameObject("Sun");
                        sunObj.transform.tag = "Sun";
                        sunObj.transform.SetParent(Containers[0].transform);
                        sunObj.transform.SetSiblingIndex(0);

                        //Add components
                        Light sunLight = sunObj.AddComponent<Light>();

                        //Modify
                        sunLight.type = LightType.Directional;
                        sunLight.color = new Color(1.00000000000f, 0.95686274509f, 0.83921568627f);
                        
                        break;
                    case "Event System":
                        //Init 
                        esObj = new GameObject("Event System");
                        esObj.transform.tag = "Event System";
                        esObj.transform.SetParent(Containers[0].transform);

                        //Add Components
                        EventSystem esES = esObj.AddComponent<EventSystem>();
                        StandaloneInputModule esSIM = esObj.AddComponent<StandaloneInputModule>();

                        break;
                    case "UI":
                        GameObject uiObj = new GameObject();
                        uiObj.transform.SetParent(Containers[0].transform);                        
                        uiScript = uiObj.AddComponent<UI>();
                        uiScript.SetCanvas();

                        esObj.transform.SetSiblingIndex(3);

                        break;
                    case "Player":
                        playerObj = new GameObject();
                        playerScript = playerObj.AddComponent<Player>();

                        playerScript.SetPlayer(gameObject);
                        break;
                    case "First Person Camera":
                        fpCamObj = new GameObject();
                        PlayerCam fpCamScript = fpCamObj.AddComponent<PlayerCam>();

                        fpCamScript.SetFirstPersonCam(playerObj);
                        break;
                    case "Third Person Camera":
                        tpCamObj = new GameObject();

                        PlayerCam tpCamScript = tpCamObj.AddComponent<PlayerCam>();
                        tpCamScript.SetThirdPersonCam(playerObj);
                        break;                        
                    default:
                        Debug.Log("This object isnt supposed to be created at the start of playtime");
                        break;
                }
            }


            /// <summary>
            /// Creates the same biome throughtout all the start chunks(without the player moving) 
            /// </summary>
            void SetStartBiome() {
                int rndStartBiomeGen = UnityEngine.Random.Range(0, 3);

                //if(worldConfig.startChunks == InitialChunks.Random && rndStartBiomeGen == 0) {
                //    biomeID = 0;
                //}
                //else if(worldConfig.startChunks == InitialChunks.Random && rndStartBiomeGen == 1) {
                //    biomeID = 1;
                //}
                //else if(worldConfig.startChunks == InitialChunks.Random && rndStartBiomeGen == 2) {
                //    biomeID = 2;
                //}
                if(worldConfig.startChunks == InitialChunks.Artic_Islands) {
                    biomeID = 0;
                } 
                else if(worldConfig.startChunks == InitialChunks.Savannah) {
                    biomeID = 1;
                }
                else if(worldConfig.startChunks == InitialChunks.Swamp) {
                    biomeID = 2;
                }
            }

			/// <summary>
            /// Check the distance to the player for inactive or remove the chunk.  
            /// </summary>
            void HiddeRemoveChunk() {
                List<Vector2Int> removeList = new List<Vector2Int>(); ;
                foreach (KeyValuePair<Vector2Int, Chunk> chunk in chunkDictTwo) {
                    float distance = Mathf.Sqrt(Mathf.Pow((player.position.x - chunk.Value.transform.position.x), 2) + Mathf.Pow((player.position.z - chunk.Value.transform.position.z), 2));
                    if (distance > removeDistance) {
                        chunk.Value.SaveChunkInRegion();//Save chunk only in case that get some modifications
                        Destroy(chunk.Value.gameObject);
                        removeList.Add(chunk.Key);
                    }
                    else if (distance > hideDistance && chunk.Value.gameObject.activeSelf) {
                        chunk.Value.gameObject.SetActive(false);
                    }
                }

                //remove chunks
                if(removeList.Count != 0) {
                    foreach(Vector2Int key in removeList) {
                        chunkDictTwo.Remove(key);
                    }
                }
            }

            /// <summary>
            /// Load in chunkLoadList or active Gameobject chunks at the chunkViewDistance radius of the player
            /// </summary>
            void CheckNewChunks() {
                //Gets the chunk the player is standing on
                Vector2Int actualChunk =new Vector2Int(Mathf.CeilToInt((player.position.x- Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE ),
                                                        Mathf.CeilToInt((player.position.z - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE ));
        
                for(int x= actualChunk.x-debugTest.chunkViewDistance; x< actualChunk.x + debugTest.chunkViewDistance; x++) {
                    for (int z = actualChunk.y - debugTest.chunkViewDistance; z < actualChunk.y + debugTest.chunkViewDistance; z++) {
                        if (Mathf.Pow((actualChunk.x - x), 2) + Mathf.Pow((actualChunk.y - z), 2) > debugTest.chunkViewDistance * debugTest.chunkViewDistance) {
                            continue;
                        }

                        Vector2Int key = new Vector2Int(x, z);

                        //sets the chunls visile
                        if (!chunkDictTwo.ContainsKey(key)) {
                            if(!chunkLoadList.Contains(key)) {
                                chunkLoadList.Add(key);
                            }
                        }
                        else {
                            if(!chunkDictTwo[key].gameObject.activeSelf) {
                                chunkDictTwo[key].gameObject.SetActive(true);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Load one chunk per frame from the chunkLoadList
            /// </summary>
            void LoadChunkFromList() {
                if (chunkLoadList.Count == 0) {
                    return;
                }

                if(isStartingWorldDoneLoading) {
                    differentBiomes = true;
                }

                Vector2Int key = chunkLoadList[0];

                Vector2Int regionPos = new Vector2Int(Mathf.FloorToInt(((float)key.x) / Constants.REGION_SIZE), Mathf.FloorToInt(((float)key.y) / Constants.REGION_SIZE));
                if(!regionDict.ContainsKey(regionPos)) {//In case that the chunk isn't in the loaded regions we remove it, tp or too fast movement
                    chunkLoadList.RemoveAt(0);
                    return;
                }
                GameObject chunkObj = new GameObject("Chunk (" + key.x + "," + key.y + ")", typeof(MeshFilter), typeof(MeshRenderer));
                chunkObj.transform.parent = Containers[1].transform;
                chunkObj.transform.tag = "Terrain";
                chunkObj.transform.position = new Vector3(key.x * Constants.CHUNK_SIDE, 0.0f, key.y * Constants.CHUNK_SIDE);
                //Debug.Log("Try load: "+x+"|"+z +" in "+regionPos);

                Vector2Int keyInsideChunk = new Vector2Int(key.x - regionPos.x * Constants.REGION_SIZE , key.y - regionPos.y * Constants.REGION_SIZE);
        //We get X and Y in the world position, we need calculate the x and y in the region.
                int chunkIndexInRegion = regionDict[regionPos].GetChunkIndex(keyInsideChunk.x, keyInsideChunk.y);
                if (chunkIndexInRegion != 0) {//Load chunk from a region data
                    chunkDictTwo.Add(key, chunkObj.AddComponent<Chunk>().ChunkInit(regionDict[regionPos].GetChunkData(chunkIndexInRegion), keyInsideChunk.x, keyInsideChunk.y, regionDict[regionPos], false));
                }
                else {//Generate chunk with the noise generator
                    chunkDictTwo
                    .Add(key, chunkObj.AddComponent<Chunk>()
                    .ChunkInit(noiseManagerTwo.
                    GenerateChunkData(key), keyInsideChunk.x, keyInsideChunk.y, regionDict[regionPos], Constants.SAVE_GENERATED_CHUNKS));
                }
                chunkLoadList.RemoveAt(0);
            }

            /// <summary>
            /// Check chunk manager need load a new regions area
            /// </summary>
            void CheckRegion() {
                if (Mathf.Abs(lastPlayerPos.x - player.position.x) > loadRegionDistance || Mathf.Abs(lastPlayerPos.z - player.position.z) > loadRegionDistance ) {
                    int actualX = Mathf.FloorToInt(player.position.x / loadRegionDistance) ;
                    lastPlayerPos.x = actualX * loadRegionDistance + loadRegionDistance / 2;
                    int actualZ = Mathf.FloorToInt(player.position.z / loadRegionDistance);
                    lastPlayerPos.z = actualZ * loadRegionDistance + loadRegionDistance / 2;
                    LoadRegion(actualX, actualZ);
                }
            }


   
            /// <summary>
            /// Calculate the distances of hide, remove and load chunks.
            /// </summary>
            void CalculateDistances() {
                lastChunkViewDistance = debugTest.chunkViewDistance;
                hideDistance = Constants.CHUNK_SIDE * debugTest.chunkViewDistance;
                removeDistance = hideDistance + hideDistance * debugTest.chunkMantainDistance;
            }

            /// <summary>
            /// Modify voxels in a specific point of a chunk.
            /// </summary>
            public void ModifyChunkData(Vector3 modificationPoint, float range, float modification, int mat = -1) {
                Vector3 originalPint = modificationPoint;
                modificationPoint = new Vector3(modificationPoint.x / Constants.VOXEL_SIDE, modificationPoint.y / Constants.VOXEL_SIDE, modificationPoint.z / Constants.VOXEL_SIDE);

                //Chunk voxel position (based on the chunk system)
                Vector3 vertexOrigin = new Vector3((int)modificationPoint.x, (int)modificationPoint.y, (int)modificationPoint.z);

                //intRange (convert Vector3 real world range to the voxel size range)
                int intRange = (int)(range / 2 * Constants.VOXEL_SIDE);//range /2 because the for is from -intRange to +intRange

                for (int y = -intRange; y <= intRange; y++) {
                    for (int z = -intRange; z <= intRange; z++) {
                        for (int x = -intRange; x <= intRange; x++) {
                            //Avoid edit the first and last height vertex of the chunk, for avoid non-faces in that heights
                            if (vertexOrigin.y + y >= Constants.MAX_HEIGHT / 2 || vertexOrigin.y + y <= -Constants.MAX_HEIGHT / 2) {
                                continue;
                            }
                            //Edit vertex of the chunk
                            Vector3 vertexPoint = new Vector3(vertexOrigin.x + x, vertexOrigin.y + y, vertexOrigin.z + z);

                            float distance = Vector3.Distance(vertexPoint, modificationPoint);
                            if (distance > range) {//Not in range of modification, we check other vertexs
                                //Debug.Log("no Rango: "+ distance + " > " + range+ " |  "+ vertexPoint +" / " + modificationPoint);
                                continue;
                            }

                            //Chunk of the vertexPoint
                            Vector2Int hitChunk = new Vector2Int(Mathf.CeilToInt((vertexPoint.x + 1 - Constants.CHUNK_SIZE / 2) / Constants.CHUNK_SIZE),
                                                    Mathf.CeilToInt((vertexPoint.z + 1 - Constants.CHUNK_SIZE / 2) / Constants.CHUNK_SIZE));
                            //Position of the vertexPoint in the chunk (x,y,z)
                            Vector3Int vertexChunk = new Vector3Int((int)(vertexPoint.x - hitChunk.x * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2),
                                (int)(vertexPoint.y + Constants.CHUNK_VERTEX_HEIGHT / 2),
                                (int)(vertexPoint.z - hitChunk.y * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2));

                            int chunkModification = (int)(modification * (1 - distance / range));
                            //Debug.Log( vertexPoint + " | chunk: "+ hitChunk+ " / " + vertexChunk);//Debug Vertex point to chunk and vertexChunk
                            chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);//vertexChunk

                            //Functions for change last vertex of chunk (vertex that touch others chunk)
                            if (vertexChunk.x == 0 && vertexChunk.z == 0) {//Interact with chunk(-1,-1), chunk(-1,0) and chunk(0,-1)
                                //Vertex of chunk (-1,0)
                                hitChunk.x -= 1;//Chunk -1
                                vertexChunk.x = Constants.CHUNK_SIZE; //Vertex of a chunk -1, last vertex
                                chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);
                                //Vertex of chunk (-1,-1)
                                hitChunk.y -= 1;
                                vertexChunk.z = Constants.CHUNK_SIZE;
                                chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);
                                //Vertex of chunk (0,-1)
                                hitChunk.x += 1;
                                vertexChunk.x = 0;
                                chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);
                            }
                            else if (vertexChunk.x == 0) {//Interact with vertex of chunk(-1,0)
                                hitChunk.x -= 1;
                                vertexChunk.x = Constants.CHUNK_SIZE;
                                chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);
                            }
                            else if (vertexChunk.z == 0) {//Interact with vertex of chunk(0,-1)
                                hitChunk.y -= 1;
                                vertexChunk.z = Constants.CHUNK_SIZE;
                                chunkDictTwo[hitChunk].ModifyTerrain(originalPint, chunkModification, mat);
                            }
                        }
                    }
                }
            }


            /// <summary>
            /// Get the material(byte) from a specific point in the world
            /// </summary>
            public byte GetMaterialFromPoint(Vector3 point) {
                point = new Vector3(point.x / Constants.VOXEL_SIDE, point.y / Constants.VOXEL_SIDE, point.z / Constants.VOXEL_SIDE);

                Vector3 vertexOrigin = new Vector3((int)point.x, (int)point.y, (int)point.z);

                //Chunk containing the point
                Vector2Int hitChunk = new Vector2Int(Mathf.CeilToInt((vertexOrigin.x + 1 - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE),
                                        Mathf.CeilToInt((vertexOrigin.z + 1 - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE));
                //VertexPoint of the point in the chunk (x,y,z)
                Vector3Int vertexChunk = new Vector3Int((int)(vertexOrigin.x - hitChunk.x * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2),
                    (int)(vertexOrigin.y + Constants.CHUNK_VERTEX_HEIGHT / 2),
                    (int)(vertexOrigin.z - hitChunk.y * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2));

                if (chunkDictTwo[hitChunk].GetMaterial(vertexChunk) != Constants.NUMBER_MATERIALS) {//not air material, we return it
                    return chunkDictTwo[hitChunk].GetMaterial(vertexChunk);
                }
                else {//Loop next vertex for get a other material different to air
                    //we check six next vertex 
                    Vector3[] nextVertexPoints = new Vector3[6];
                    nextVertexPoints[0] = new Vector3(vertexOrigin.x + 0, vertexOrigin.y - 1, vertexOrigin.z + 0);
                    nextVertexPoints[1] = new Vector3(vertexOrigin.x + 1, vertexOrigin.y + 0, vertexOrigin.z + 0);
                    nextVertexPoints[2] = new Vector3(vertexOrigin.x - 1, vertexOrigin.y + 0, vertexOrigin.z + 0);
                    nextVertexPoints[3] = new Vector3(vertexOrigin.x + 0, vertexOrigin.y + 0, vertexOrigin.z + 1);
                    nextVertexPoints[4] = new Vector3(vertexOrigin.x + 0, vertexOrigin.y + 0, vertexOrigin.z + 1);
                    nextVertexPoints[5] = new Vector3(vertexOrigin.x + 0, vertexOrigin.y + 1, vertexOrigin.z + 0);
                    List<byte> mats = new List<byte>();
                    for (int i = 0; i < nextVertexPoints.Length; i++){
                        //Chunk of the vertexPoint
                        hitChunk = new Vector2Int(Mathf.CeilToInt((nextVertexPoints[i].x + 1 - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE),
                                                        Mathf.CeilToInt((nextVertexPoints[i].z + 1 - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE));
                        //Position of the vertexPoint in the chunk (x,y,z)
                        vertexChunk = new Vector3Int((int)(nextVertexPoints[i].x - hitChunk.x * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2),
                            (int)(nextVertexPoints[i].y + Constants.CHUNK_VERTEX_HEIGHT / 2),
                            (int)(nextVertexPoints[i].z - hitChunk.y * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2));

                        if (chunkDictTwo[hitChunk].GetMaterial(vertexChunk) != Constants.NUMBER_MATERIALS) {//not air material, we return it
                            return chunkDictTwo[hitChunk].GetMaterial(vertexChunk);
                        }
                    }
                }

                return Constants.NUMBER_MATERIALS;//only air material in that point.
            }


            /// <summary>
            /// Save all chunk and regions data when user close the game.
            /// </summary>
            void OnApplicationQuit() {
                //save chunks
                foreach(Chunk chunk in chunkDictTwo.Values) {
                    chunk.SaveChunkInRegion();
                }

                //save regions
                foreach (Region region in regionDict.Values) {
                    region.SaveRegionData();
                }
            }



            #region DebugMode
            //The code of the region is used for the Debug system, allow you to check your current chunk and see data from the voxels.

            #if UNITY_EDITOR
            private void OnDrawGizmos() {
                if (debugTest.debugMode && Application.isPlaying) {
                    //Show chunk
                    Vector2Int actualChunk = new Vector2Int(Mathf.CeilToInt((player.position.x - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE),
                                                Mathf.CeilToInt((player.position.z - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE));
                    Vector3 chunkCenter = new Vector3(actualChunk.x * Constants.CHUNK_SIDE, 0, actualChunk.y * Constants.CHUNK_SIDE);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(chunkCenter, new Vector3(Constants.CHUNK_SIDE, Constants.MAX_HEIGHT * Constants.VOXEL_SIDE, Constants.CHUNK_SIDE));

                    //Show voxel
                    RaycastHit hitInfo;
                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 100.0f)) {
                        Vector2Int chunkHit = new Vector2Int(Mathf.CeilToInt((hitInfo.point.x - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE),
                                    Mathf.CeilToInt((hitInfo.point.z - Constants.CHUNK_SIDE / 2) / Constants.CHUNK_SIDE));
                        Vector3Int vertexChunk = new Vector3Int((int)(hitInfo.point.x - chunkHit.x * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2),
                                    (int)(hitInfo.point.y + Constants.CHUNK_VERTEX_HEIGHT / 2),
                                    (int)(hitInfo.point.z - chunkHit.y * Constants.CHUNK_SIZE + Constants.CHUNK_VERTEX_SIZE / 2));
                        Vector3 voxelRealPosition = new Vector3((Mathf.FloorToInt(hitInfo.point.x / Constants.VOXEL_SIDE)) * Constants.VOXEL_SIDE + Constants.VOXEL_SIDE/2,
                                    (Mathf.FloorToInt(hitInfo.point.y / Constants.VOXEL_SIDE)) * Constants.VOXEL_SIDE + Constants.VOXEL_SIDE/2,
                                    (Mathf.FloorToInt(hitInfo.point.z / Constants.VOXEL_SIDE)) * Constants.VOXEL_SIDE + Constants.VOXEL_SIDE/2);

                        Gizmos.color = Color.red;
                        Gizmos.DrawWireCube(voxelRealPosition, Vector3.one * Constants.VOXEL_SIDE);
                    }
                }
            }
			#endif
			#endregion

			public void Initialize() {
                noiseManagerTwo = World.Instance;
                if (debugTest.useCameraPosition) {
                    player = Camera.main.transform;//Use the Camera.main as player pos
                }
                else {
                    player = GameObject.FindGameObjectWithTag("Player").transform;//Search gameobject with tag Player
                    loadRegionDistance = Constants.CHUNK_SIDE * Constants.REGION_SIZE * Constants.VOXEL_SIDE * 0.9f;
                    lastPlayerPos.x = Mathf.FloorToInt(player.position.x / loadRegionDistance) * loadRegionDistance + loadRegionDistance / 2;
                    lastPlayerPos.z = Mathf.FloorToInt(player.position.z / loadRegionDistance) * loadRegionDistance + loadRegionDistance / 2;
                    InitRegion(Mathf.FloorToInt(player.position.x / loadRegionDistance), Mathf.FloorToInt(player.position.z/ loadRegionDistance));
                }
            }

            /// <summary>
            /// Load surrounding regions of the player when first load
            /// </summary>
            void InitRegion(int initX, int initZ) {
                for (int x = initX-1; x < initX+2; x++) {
                    for (int z = initZ-1; z < initZ + 2; z++) {
                        regionDict.Add(new Vector2Int(x,z), new Region(x,z));
                    }
                }
            }

            /// <summary>
            /// Load new regions and unload the older.
            /// </summary>
            void LoadRegion(int initX, int initZ) {
                Dictionary<Vector2Int, Region> newRegionDict = new Dictionary<Vector2Int, Region>();

                for (int x = initX-1; x < initX + 2; x++) {
                    for (int z = initZ-1; z < initZ + 2; z++) {
                        if (regionDict.ContainsKey(new Vector2Int(x,z))) {
                            newRegionDict.Add(new Vector2Int(x, z), regionDict[new Vector2Int(x, z)]);
                            regionDict.Remove(new Vector2Int(x, z));
                        }
                        else {
                            newRegionDict.Add(new Vector2Int(x,z), new Region(x, z));
                        }
                    }
                }
                //save old regions
                foreach (Region region in regionDict.Values) {
                    region.SaveRegionData();
                }
                //Assign new region area
                regionDict = newRegionDict;
            }

			public void CreateStartWorld() {
                if(chunkDictOne.Count != 0) {
                    foreach(Chunk chunk in chunkDictOne.Values) {
                        Destroy(chunk.gameObject);
                    }
                    chunkDictOne.Clear();
                }


                int halfSize = Mathf.FloorToInt(debugTest.testSize / 2);
                for(int z= -halfSize; z< halfSize+1; z++) {
                    for (int x = -halfSize; x < halfSize+1; x++) {
                        Vector2Int key = new Vector2Int(x, z);
                        GameObject chunkObj = new GameObject("Chunk (" + key.x + "," + key.y + ")", typeof(MeshFilter), typeof(MeshRenderer));
                        chunkObj.transform.parent = transform;
                        chunkObj.transform.tag = "Terrain";
                        chunkObj.transform.position = new Vector3(key.x * Constants.CHUNK_SIDE, 0, key.y * Constants.CHUNK_SIDE);

                        Vector2Int offsetKey = new Vector2Int(x + debugTest.chunkOffset.x, z+ debugTest.chunkOffset.y);
                        chunkDictOne.Add(key, chunkObj.AddComponent<Chunk>().ChunkInit(noiseManagerOne.GenerateChunkData(offsetKey), key.x, key.y, fakeRegionOne, false));
                    }
                }
            }

			public byte[] GenerateChunkData(Vector2Int vecPos) {
				byte[] chunkData = new byte[Constants.CHUNK_BYTES];

				float[] biomeNoise = GenerateNoiseMap(worldConfig.biomeScale * scriptableObjects.biomes.Length, worldConfig.octaves, worldConfig.persistance, worldConfig.lacunarity, vecPos);//Biomes noise (0-1) of each (x,z) position
				float[] mergeBiomeTable;//Value(0-1) of merged with other biomes in a (x,z) position
				int[] biomeTable = GetChunkBiomes(biomeNoise, out mergeBiomeTable);//biomes index in the array of BiomeProperties

				byte[][] biomesData = new byte[scriptableObjects.biomes.Length][];//Data generate from biomes.biome.GenerateChunkData()

				for (int z = 0;  z < Constants.CHUNK_VERTEX_SIZE; z++) {
					for(int x = 0; x <Constants.CHUNK_VERTEX_SIZE; x++) {
						int index = x + z * Constants.CHUNK_VERTEX_SIZE;

						if (biomesData[biomeTable[index]] == null) {
							biomesData[biomeTable[index]] = scriptableObjects.biomes[biomeTable[index]].GenerateChunkData(vecPos, mergeBiomeTable, vectorValues); //Problem with this line when I move fron the singletons prefab to a single gameobject
                        }
                        else {
                        }

						for (int y = 0; y < Constants.CHUNK_VERTEX_HEIGHT; y++) {
							int chunkByteIndex = (index + y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;
							chunkData[chunkByteIndex] = biomesData[biomeTable[index]][chunkByteIndex];
							chunkData[chunkByteIndex+1] = biomesData[biomeTable[index]][chunkByteIndex+1];
						}
					}
				}

				return chunkData;

			}

			/// <summary>
			/// Get the index from the biomes array, the bool out is for get the merge biome
			/// </summary>
			private int[] GetChunkBiomes(float[] noise, out float[] mergeBiome) {
				float[] mergeBiomeTable= new float[Constants.CHUNK_VERTEX_AREA];//value of merge with other biome, 1 = nothing, 0 full merge
				int[] biomeTable = new int[Constants.CHUNK_VERTEX_AREA];//Value with the index of the biomes of each (x,z) position
				for (int z = 0; z< Constants.CHUNK_VERTEX_SIZE; z++) {
					for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE; x++) {
						int index = x + z * Constants.CHUNK_VERTEX_SIZE;
						for (int i = scriptableObjects.biomes.Length - 1; i >= 0; i--) {
							if (noise[index] < scriptableObjects.biomes[i].appearValue) {
								if (i != 0 && worldConfig.diffToMerge + noise[index] > scriptableObjects.biomes[i].appearValue) {//Biome merged with top biome
									mergeBiomeTable[index] = (scriptableObjects.biomes[i].appearValue - noise[index]) / worldConfig.diffToMerge;
									//Debug.Log("TOP: "+biomes[i].appearValue + " - " + noise[index] + " / " + diffToMerge + " = " + mergeBiomeTable[index]);
								}
								else if (i != scriptableObjects.biomes.Length - 1 && worldConfig.diffToMerge - noise[index] < scriptableObjects.biomes[i+1].appearValue) {//Biome merged with bottom biome
									mergeBiomeTable[index] = (noise[index] - scriptableObjects.biomes[i + 1].appearValue) / worldConfig.diffToMerge;
									//Debug.Log("BOT: "+noise[index] + " - " + biomes[i + 1].appearValue + " / " + diffToMerge + " = " + mergeBiomeTable[index]);
								}
								else {
									mergeBiomeTable[index] = 1;//No biome merge needed
								}

                                if(differentBiomes) {
                                    biomeTable[index] = i;
                                }
                                else {
                                    biomeTable[index] = biomeID;
                                }
								break;//We get the texture, we exit from texture loop( i loop)

							}
						}
					}
				}

				mergeBiome = mergeBiomeTable;
				return biomeTable;
			}


			/// <summary>
			/// Calculate the PerlinNoise used in the relief generation, only the chunk size (no slope calculation).
			/// </summary>
			public static float[] GenerateNoiseMap (float scale, int octaves, float persistance, float lacunarity, Vector2Int offset) {
				float[] noiseMap = new float[Constants.CHUNK_VERTEX_AREA];//Size of vertex + all next borders (For the slope calculation)

				System.Random random = new System.Random(Instance.worldConfig.worldSeed);//Used System.random, because unity.Random is global, can cause problems if there is other random running in other script
				Vector2[] octaveOffsets = new Vector2[octaves];

				float maxPossibleHeight = 0;
				float amplitude = 1;
				float frequency = 1;

				for (int i = 0; i < octaves; i++) {
					float offsetX = random.Next(-100000, 100000) + offset.x * Constants.CHUNK_SIZE;
					float offsetY = random.Next(-100000, 100000) + offset.y * Constants.CHUNK_SIZE;
					octaveOffsets[i] = new Vector2(offsetX, offsetY);

					maxPossibleHeight += amplitude;
					amplitude *= persistance;
				}

				float halfVertexArea = Constants.CHUNK_VERTEX_SIZE / 2f;

				for (int z = 0; z < Constants.CHUNK_VERTEX_SIZE; z++) {
					for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE; x++){
						amplitude = 1;
						frequency = 1;
						float noiseHeight = 0;

						for (int i = 0; i < octaves; i++)
						{
							float sampleX = (x - halfVertexArea + octaveOffsets[i].x) / scale * frequency ;
							float sampleY = (z - halfVertexArea + octaveOffsets[i].y) / scale * frequency ;

							float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) ;
							noiseHeight += perlinValue * amplitude;

							amplitude *= persistance;
							frequency *= lacunarity;
						}
				
						noiseMap[x + z * Constants.CHUNK_VERTEX_SIZE] = noiseHeight / (maxPossibleHeight * 0.9f);//*0.9 because reach the max points it's really dificult.

					}
				}

				return noiseMap;
			}

			/// <summary>
			/// Calculate the PerlinNoise used in the relief generation, with a extra edge in each side of the chunk, for the slope calculations.
			/// </summary>
			public static float[] GenerateExtendedNoiseMap(float scale, int octaves, float persistance, float lacunarity, Vector2Int offset) {
				float[] noiseMap = new float[(Constants.CHUNK_VERTEX_SIZE + 2) * (Constants.CHUNK_VERTEX_SIZE + 2)];//Size of vertex + all next borders (For the slope calculation)

				System.Random random = new System.Random(World.Instance.worldConfig.worldSeed);//Used System.random, because unity.Random is global, can cause problems if there is other random running in other script
				Vector2[] octaveOffsets = new Vector2[octaves];

				float maxPossibleHeight = 0;
				float amplitude = 1;
				float frequency = 1;

				for (int i = 0; i < octaves; i++) {
					float offsetX = random.Next(-100000, 100000) + offset.x * Constants.CHUNK_SIZE - 1;
					float offsetY = random.Next(-100000, 100000) + offset.y * Constants.CHUNK_SIZE - 1;
					octaveOffsets[i] = new Vector2(offsetX, offsetY);

					maxPossibleHeight += amplitude;
					amplitude *= persistance;
				}

				float halfVertexArea = Constants.CHUNK_VERTEX_SIZE / 2f;

				for (int z = 0; z < Constants.CHUNK_VERTEX_SIZE + 2; z++) {
					for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE + 2; x++) {
						amplitude = 1;
						frequency = 1;
						float noiseHeight = 0;

						for (int i = 0; i < octaves; i++) {
							float sampleX = (x - halfVertexArea + octaveOffsets[i].x) / scale * frequency;
							float sampleY = (z - halfVertexArea + octaveOffsets[i].y) / scale * frequency;

							float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
							noiseHeight += perlinValue * amplitude;

							amplitude *= persistance;
							frequency *= lacunarity;
						}

						noiseMap[x + z * (Constants.CHUNK_VERTEX_SIZE + 2)] = noiseHeight / (maxPossibleHeight * 0.9f);//*0.9 because reach the max points it's really dificult.

					}
				}

				return noiseMap;
			}

			/// <summary>
			/// Similar that GenerateNoiseMap but use only one octave, for that reason use less parameters and less operations
			/// </summary>
			public static float[] GenenerateSimpleNoiseMap(float scale, Vector2Int offset)
			{
				float[] noiseMap = new float[Constants.CHUNK_VERTEX_AREA];//Size of vertex + all next borders (For the slope calculation)

				System.Random random = new System.Random(World.Instance.worldConfig.worldSeed);//Used System.random, because unity.Random is global, can cause problems if there is other random running in other script

				float offsetX = random.Next(-100000, 100000) + offset.x * Constants.CHUNK_SIZE ;
				float offsetY = random.Next(-100000, 100000) + offset.y * Constants.CHUNK_SIZE ;

				float halfVertexArea = Constants.CHUNK_VERTEX_SIZE / 2f;

				for (int z = 0; z < Constants.CHUNK_VERTEX_SIZE; z++)
				{
					for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE; x++)
					{
						float sampleX = (x - halfVertexArea + offsetX) / scale;
						float sampleY = (z - halfVertexArea + offsetY) / scale;

						noiseMap[x + z * Constants.CHUNK_VERTEX_SIZE] = Mathf.PerlinNoise(sampleX, sampleY);
					}
				}

				return noiseMap;
			}

            public Mesh BuildChunk(byte[] b) {
                BuildChunkJob buildChunkJob = new BuildChunkJob {
                    chunkData = new NativeArray<byte>(b, Allocator.TempJob),
                    isoLevel = this.debugTest.isoLevel,
                    interpolate = this.debugTest.interpolate,
                    vertex = new NativeList<float3>(500, Allocator.TempJob),
                    uv = new NativeList<float2>(100, Allocator.TempJob),
                };
                JobHandle jobHandle = buildChunkJob.Schedule();
                jobHandle.Complete();

                //Get all the data from the jobs and use to generate a Mesh
                Mesh meshGenerated = new Mesh();
                Vector3[] meshVert = new Vector3[buildChunkJob.vertex.Length];
                int[] meshTriangles = new int[buildChunkJob.vertex.Length];
                for (int i = 0; i < buildChunkJob.vertex.Length; i++) {
                    meshVert[i] = buildChunkJob.vertex[i];
                    meshTriangles[i] = i;
                }
                meshGenerated.vertices = meshVert;

                Vector2[] meshUV = new Vector2[buildChunkJob.vertex.Length];

                for (int i = 0; i < buildChunkJob.vertex.Length; i++) {
                    meshUV[i] = buildChunkJob.uv[i];
                }
                meshGenerated.uv = meshUV;
                meshGenerated.triangles = meshTriangles;
                meshGenerated.RecalculateNormals();
                meshGenerated.RecalculateTangents();

                //Dispose (Clear the jobs NativeLists)
                buildChunkJob.vertex.Dispose();
                buildChunkJob.uv.Dispose();
                buildChunkJob.chunkData.Dispose();

                return meshGenerated;
            }

            //This old code was adapted in the "BuildChunkJob" script and don't used anymore. (Stay if someone want to use the ) 
            #region Original code (Deprecated)

            /// <summary>
            /// Method that calculate cubes, vertex and mesh in that order of a chunk.
            /// </summary>
            /// <param name="b"> data of the chunk</param>
            public Mesh BuildChunkDeprecated(byte[] b) {
                List<Vector3> vertexArray = new List<Vector3>();
                List<Vector2> matVert = new List<Vector2>();
                for (int y = 0; y < Constants.MAX_HEIGHT; y++) { //height
                    for (int z = 1; z < Constants.CHUNK_SIZE + 1; z++) { //column, start at 1, because Z axis is inverted and need -1 as offset
                        for (int x = 0; x < Constants.CHUNK_SIZE; x++) { //line 
                            Vector4[] cube = new Vector4[8];
                            int mat = Constants.NUMBER_MATERIALS;
                            cube[0] = CalculateVertexChunk(x, y, z, b, ref mat);
                            cube[1] = CalculateVertexChunk(x + 1, y, z, b, ref mat);
                            cube[2] = CalculateVertexChunk(x + 1, y, z - 1, b, ref mat);
                            cube[3] = CalculateVertexChunk(x, y, z - 1, b, ref mat);
                            cube[4] = CalculateVertexChunk(x, y + 1, z, b, ref mat);
                            cube[5] = CalculateVertexChunk(x + 1, y + 1, z, b, ref mat);
                            cube[6] = CalculateVertexChunk(x + 1, y + 1, z - 1, b, ref mat);
                            cube[7] = CalculateVertexChunk(x, y + 1, z - 1, b, ref mat);
                            vertexArray.AddRange(CalculateVertex(cube, mat, ref matVert));
                        }
                    }
                }
                return buildMesh(vertexArray, matVert);
            }

            /// <summary>
            /// It generate a mesh from a group of vertex. Flat shading type.(Deprecated)
            /// </summary>
            public Mesh buildMesh(List<Vector3> vertex, List<Vector2> textures = null) {
                Mesh mesh = new Mesh();
                int[] triangles = new int[vertex.Count];

                mesh.vertices = vertex.ToArray();
        
                if (textures != null) {
                    mesh.uv = textures.ToArray();
                }

                for (int i = 0; i < triangles.Length; i++) {
                    triangles[i] = i;
                }

                mesh.triangles = triangles;
                return mesh;
            }

            /// <summary>
            ///  Calculate the vertices of the voxels, get the vertices of the triangulation table and his position in the world. Also check materials of that vertex (UV position).(Deprecated)
            /// </summary>
            public List<Vector3> CalculateVertex(Vector4[] cube, int colorVert, ref List<Vector2> matVert) {
                //Values above isoLevel are inside the figure, value of 0 means that the cube is entirely inside of the figure.
                int cubeindex = 0;
                if (cube[0].w < debugTest.isoLevel) cubeindex |= 1;
                if (cube[1].w < debugTest.isoLevel) cubeindex |= 2;
                if (cube[2].w < debugTest.isoLevel) cubeindex |= 4;
                if (cube[3].w < debugTest.isoLevel) cubeindex |= 8;
                if (cube[4].w < debugTest.isoLevel) cubeindex |= 16;
                if (cube[5].w < debugTest.isoLevel) cubeindex |= 32;
                if (cube[6].w < debugTest.isoLevel) cubeindex |= 64;
                if (cube[7].w < debugTest.isoLevel) cubeindex |= 128;

                List<Vector3> vertexArray = new List<Vector3>();

                for (int i = 0; Constants.triTable[cubeindex, i] != -1; i++) {
                    int v1 = Constants.cornerIndexAFromEdge[Constants.triTable[cubeindex, i]];
                    int v2 = Constants.cornerIndexBFromEdge[Constants.triTable[cubeindex, i]];

                    if (debugTest.interpolate) {
                        vertexArray.Add(InterporlateVertex(cube[v1], cube[v2], cube[v1].w, cube[v2].w));
                    }
                    else {
                        vertexArray.Add(MiddlePointVertex(cube[v1], cube[v2]));
                    }

                    const float uvOffset = 0.01f; //Small offset for avoid pick pixels of other textures
                    //NEED REWORKING FOR CORRECT WORKING, now have problems with the directions of the uv
                    if (i % 6 == 0) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE*(colorVert % Constants.MATERIAL_FOR_ROW)+ Constants.MATERIAL_SIZE-uvOffset, 1- Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW)-uvOffset));
                    }
                    else if (i % 6 == 1) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE * (colorVert % Constants.MATERIAL_FOR_ROW) + Constants.MATERIAL_SIZE - uvOffset, 1 - Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW)- Constants.MATERIAL_SIZE + uvOffset));
                    }
                    else if(i % 6 == 2) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE * (colorVert % Constants.MATERIAL_FOR_ROW) + uvOffset, 1 - Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW) -uvOffset));
                    }
                    else if (i % 6 == 3) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE * (colorVert % Constants.MATERIAL_FOR_ROW) + Constants.MATERIAL_SIZE - uvOffset, 1 - Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW) - Constants.MATERIAL_SIZE + uvOffset));
                    }
                    else if (i % 6 == 4) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE * (colorVert % Constants.MATERIAL_FOR_ROW) + uvOffset, 1 - Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW) - Constants.MATERIAL_SIZE + uvOffset));
                    }
                    else if (i % 6 == 5) {
                        matVert.Add(new Vector2(Constants.MATERIAL_SIZE * (colorVert % Constants.MATERIAL_FOR_ROW + uvOffset), 1 - Constants.MATERIAL_SIZE * Mathf.Floor(colorVert / Constants.MATERIAL_FOR_ROW) - uvOffset));
                    }
                }
                return vertexArray;
            }


            /// <summary>
            /// Calculate the data of a vertex of a voxel.(Deprecated)
            /// </summary>
            private Vector4 CalculateVertexChunk(int x, int y, int z, byte[] b, ref int colorVoxel) {
                int index = (x + z * Constants.CHUNK_VERTEX_SIZE + y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;
                int material = b[index+1];
        
        
                if (b[index] >= debugTest.isoLevel && material < colorVoxel) {
                    colorVoxel = material;
                }
                return new Vector4(
                    (x - Constants.CHUNK_SIZE / 2) * Constants.VOXEL_SIDE,
                    (y - Constants.MAX_HEIGHT / 2) * Constants.VOXEL_SIDE,
                    (z - Constants.CHUNK_SIZE / 2) * Constants.VOXEL_SIDE,
                    b[index]);
            }

            /// <summary>
            /// Overload of the CalculateVertex method but without material calculations.
            /// </summary>
            public List<Vector3> CalculateVertex(Vector4[] cube) {
                //Values above isoLevel are inside the figure, value of 0 means that the cube is entirely inside of the figure.(Deprecated)
                int cubeindex = 0;
                if (cube[0].w < debugTest.isoLevel) cubeindex |= 1;
                if (cube[1].w < debugTest.isoLevel) cubeindex |= 2;
                if (cube[2].w < debugTest.isoLevel) cubeindex |= 4;
                if (cube[3].w < debugTest.isoLevel) cubeindex |= 8;
                if (cube[4].w < debugTest.isoLevel) cubeindex |= 16;
                if (cube[5].w < debugTest.isoLevel) cubeindex |= 32;
                if (cube[6].w < debugTest.isoLevel) cubeindex |= 64;
                if (cube[7].w < debugTest.isoLevel) cubeindex |= 128;

                List<Vector3> vertexArray = new List<Vector3>();

                for (int i = 0; Constants.triTable[cubeindex, i] != -1; i++) {
                    int v1 = Constants.cornerIndexAFromEdge[Constants.triTable[cubeindex, i]];
                    int v2 = Constants.cornerIndexBFromEdge[Constants.triTable[cubeindex, i]];

                    if (debugTest.interpolate) {
                        vertexArray.Add(InterporlateVertex(cube[v1], cube[v2], cube[v1].w, cube[v2].w));
                    }
                    else {
                        vertexArray.Add(MiddlePointVertex(cube[v1], cube[v2]));
                    }
                }

                return vertexArray;

            }

            //HelpMethods

            /// <summary>
            /// Calculate a point between two vertex using the weight of each vertex , used in interpolation voxel building.(Deprecated)
            /// </summary>
            public Vector3 InterporlateVertex(Vector3 p1, Vector3 p2,float val1,float val2) {
                return Vector3.Lerp(p1, p2, (debugTest.isoLevel - val1) / (val2 - val1));
            }
            /// <summary>
            /// Calculate the middle point between two vertex, for no interpolation voxel building.(Deprecated)
            /// </summary>
            public Vector3 MiddlePointVertex(Vector3 p1, Vector3 p2) {
                return (p1 + p2) / 2;
            }
            #endregion
        }
	}
}