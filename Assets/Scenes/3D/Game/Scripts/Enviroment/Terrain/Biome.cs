using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        [CreateAssetMenu(fileName = "New Biome", menuName = "InfinnaWorld/Enviroment/Terrain/Biome")]
        public class Biome : ScriptableObject {
			public string Name;
			public float appearValue;

			
            [Tooltip("The max deep and height of the desert dunes, low values")][Range(0, Constants.MAX_HEIGHT - 1)]
			public int maxHeightDifference = Constants.MAX_HEIGHT / 5;

			[Tooltip("Number vertex (y), where the snow end and the rock start")][Range(0, Constants.MAX_HEIGHT - 1)]
			public int snowDeep;// = Constants.MAX_HEIGHT / 5;

			[Header("Ice columns configuration")]
			[Tooltip("Scale of the noise used for the ice columns appear")][Range(0, 100)]
			public int iceNoiseScale = 40;
			[Tooltip("Value in the ice noise map where the ice columns appear")][Range(0, 1)]
			public float iceApearValue = 0.8f;
			[Tooltip("Ice columns max height")][Range(0, Constants.MAX_HEIGHT - 1)]
			public int iceMaxHeight = 5;
			[Tooltip("Amplitude decrease of reliefs")]
			[Range(0.001f, 1f)]
			public float IcePersistance = 0.5f;
			[Tooltip("Frequency increase of reliefs")]
			[Range(1, 20)]
			public float IceLacunarity = 2f;


            [Header("Noise / terrain generation")]
			[Tooltip("Animation curve for attenuate the height in some ranges")]
			public AnimationCurve terrainHeightCurve = AnimationCurve.Linear(0,0,1,1);
			[Tooltip("Scale of the noise map")][Range(0.001f, 100f)]
			public float scale = 50f;
			[Tooltip("Number of deferents relief apply to the terrain surface")][Range(1, 5)]
			public int octaves = 4;
			[Tooltip("Amplitude decrease of reliefs")][Range(0.001f, 1f)]
			public float persistance = 0.5f;
			[Tooltip("Frequency increase of reliefs")][Range(1, 20)]
			public float lacunarity = 2f;

			protected int isoLevel;

            public byte firstLayerOfBlocks;
            public byte secondLayerOfBlocks;
			bool toppingBlock;
            public byte thirdLayerOfBlocks;



            void Start() {
                isoLevel = World.Instance.debugTest.isoLevel;
            }

			public byte[] GenerateChunkData(Vector2Int vecPos, float[] biomeMerge, Vector3Int Pos) {
				byte[] chunkData = new byte[Constants.CHUNK_BYTES];
				float[] noise = World.GenerateNoiseMap(scale, octaves, persistance, lacunarity, vecPos);
				float[] iceNoise = World.GenerateNoiseMap(iceNoiseScale,2,IcePersistance,IceLacunarity, vecPos);
		
				for (int z = 0; z < Constants.CHUNK_VERTEX_SIZE; z++) {
					for (int x = 0; x < Constants.CHUNK_VERTEX_SIZE; x++) {
						// Get surface height of the x,z position 
						float height = Mathf.Lerp(
							World.Instance.worldConfig.surfaceLevel,//Biome merge height
							(((terrainHeightCurve.Evaluate(noise[x + z * Constants.CHUNK_VERTEX_SIZE]) * 2 - 1) * maxHeightDifference) + World.Instance.worldConfig.surfaceLevel),//Desired biome height
							biomeMerge[x + z * Constants.CHUNK_VERTEX_SIZE]);//Merge value,0 = full merge, 1 = no merge

						int heightY = Mathf.CeilToInt(height);//Vertex Y where surface start
						int lastVertexWeigh = (int)((255 - isoLevel) * (height % 1) + isoLevel);//Weigh of the last vertex

						//Ice calculations
						int iceExtraHeigh = 0;
						if (iceNoise[x + z * Constants.CHUNK_VERTEX_SIZE] > iceApearValue) {
							iceExtraHeigh = Mathf.CeilToInt((1- iceNoise[x + z * Constants.CHUNK_VERTEX_SIZE] ) / iceApearValue * iceMaxHeight);
						}

						for (int y = 0; y < Constants.CHUNK_VERTEX_HEIGHT; y++) {
							int index = (x + z * Constants.CHUNK_VERTEX_SIZE + y * Constants.CHUNK_VERTEX_AREA) * Constants.CHUNK_POINT_BYTE;
							if (y < heightY - snowDeep) {
								chunkData[index] = 255;
								chunkData[index + 1] = secondLayerOfBlocks;//Rock
								World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = secondLayerOfBlocks;  //Secondlayer is 4
							}
							else if (y < heightY+ iceExtraHeigh) {
								chunkData[index] = 255;
								if(y <= heightY) {
									chunkData[index + 1] = firstLayerOfBlocks;//snow
									World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = firstLayerOfBlocks;
								}	
								else {
									if(toppingBlock) {
										chunkData[index + 1] = secondLayerOfBlocks;
										World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = secondLayerOfBlocks;
									}
									else {
										chunkData[index + 1] = firstLayerOfBlocks;//snow
										World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = firstLayerOfBlocks;
									}
								}
							}
							else if (y == heightY+ iceExtraHeigh) {
								chunkData[index] = (byte)lastVertexWeigh;
								if (y <= heightY) {
									chunkData[index + 1] = firstLayerOfBlocks;//snow
								}
								else {
									if(toppingBlock) {
										chunkData[index + 1] = secondLayerOfBlocks;
										World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = secondLayerOfBlocks;
									}
									else {
										chunkData[index + 1] = firstLayerOfBlocks;//snow
										World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = firstLayerOfBlocks;
									}
								}
							}
							//else {
							//	chunkData[index] = 7;
							//	World.Instance.terrainMap[Pos.x, Pos.y, Pos.z] = 7;
							//	chunkData[index + 1] = Constants.NUMBER_MATERIALS;
							//}
						}
					}
				}
				return chunkData;
			}
        }
    }
}