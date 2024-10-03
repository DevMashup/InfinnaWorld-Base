using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class PlayerCam : MonoBehaviour {
        
            Player playerScript;
            Camera tpCam;
            Camera fpCam;
            float rotX;
            float rotY;
            float speedH;
            float speedV;


            //CAMERA TERRAIN MODIFIER
            [Tooltip("Range where the player can interact with the terrain")]
            public float rangeHit = 8;
            [Tooltip("Force of modifications applied to the terrain")]
            public float modiferStrengh = 6;
            [Tooltip("Size of the brush, number of vertex modified")]
            public float sizeHit = 3;
            [Tooltip("Color of the new voxels generated")][Range(0, Constants.NUMBER_MATERIALS-1)]
            public int buildingMaterial = 0;

            private RaycastHit hit;
            private World noiseManager;
            LootTable lootTable;

            float timeInSec = 0;
            float mobDamageTime = 0;
            bool hasHitMob = false;
            Camera camCam;



            void Awake() {
                noiseManager = World.Instance;
            }

            void Start()
            {
                lootTable = new LootTable();


                playerScript = World.Instance.playerObj.GetComponent<Player>();
                tpCam = World.Instance.tpCamObj.GetComponent<Camera>();
                fpCam = World.Instance.fpCamObj.GetComponent<Camera>();

                speedV = 2.0f;
                speedH = 2.0f;
            }

            
            void Update() {
                if (playerScript.tpActivate == true) {
                    tpCam.targetDisplay = 0;
                    fpCam.targetDisplay = 1;
                }
                else {
                    fpCam.targetDisplay = 0;
                    tpCam.targetDisplay = 1;
                }

                if(!World.Instance.isGamePaused && !World.Instance.isGameOver && !World.Instance.isEnviromentPaused) {
                    timeInSec += Time.deltaTime;
                    mobDamageTime += Time.deltaTime;
                    CheckVars();
                    if (timeInSec > 0.25f) {
                        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
                            float modification = (Input.GetMouseButton(0)) ? modiferStrengh : -modiferStrengh;

                            if (Physics.Raycast(transform.position, transform.forward, out hit, rangeHit)) {
                                if(hit.transform.tag == "Terrain") {
                                    //byte mat = World.Instance.terrainMap[Mathf.FloorToInt(hit.point.x), Mathf.FloorToInt(hit.point.y), Mathf.FloorToInt(hit.point.z)];

                                    //Debug.Log(mat);
                                    noiseManager.ModifyChunkData(hit.point, sizeHit, modification, buildingMaterial);
                                }
                                else {
                                    MobHit();
                                }
                            }

                            timeInSec = 0;
                        }
                    }

                    if(mobDamageTime > 0.3f && hasHitMob) { //If wehit the mob over a second ago
                        if(World.Instance.mobManager.Alligator != null) { //If there is a alligator game object 
                            foreach(Transform child in World.Instance.mobManager.Alligator.GetComponentInChildren<Transform>()) {
                                if(child.GetComponent<SkinnedMeshRenderer>() != null) { //If the alligator's scene children has a component
                                    Material[] matArray = child.GetComponent<SkinnedMeshRenderer>().materials; 
                                    matArray[1] = Resources.Load<Material>("Visuals/Materials/Mobs/2nd/Clear");
                                    child.GetComponent<SkinnedMeshRenderer>().materials = matArray;


                                    hasHitMob = false; //Return the hashitmpbtofalse
                                }
                            }
                        }
                    }

                    if(Input.GetKey(KeyCode.Space)) {
                        transform.position += Vector3.up * 10.0f * Time.deltaTime;
                    }
                    else if(Input.GetKey(KeyCode.LeftShift)) {
                        transform.position -= Vector3.up * 10.0f * Time.deltaTime;
                    }
                }
            }

            void CheckVars() {
                if(!World.Instance.isGamePaused) {
                    rotX = speedH -= Input.GetAxis("Mouse Y");
                    rotY = speedV += Input.GetAxis("Mouse X");

                    rotX = Mathf.Clamp(rotX, -90f, 90f);

                    transform.eulerAngles = new Vector3(rotX, rotY, 0);
                }
            }

            void MobHit() {
                if(hit.transform.tag == "Alligator") {
                    foreach(Transform child in World.Instance.mobManager.Alligator.GetComponentInChildren<Transform>()) { //Get all the children belonging to the alligator object
                        if(child.GetComponent<SkinnedMeshRenderer>() != null) { //If a child has this component
                            mobDamageTime = 0; //We hit the alligator so we reset the effect

                            Material[] matArray = child.GetComponent<SkinnedMeshRenderer>().materials;
                            matArray[1] = Resources.Load<Material>("Visuals/Materials/Mobs/2nd/Damaged"); //Load a material onto the hit alligator
                            child.GetComponent<SkinnedMeshRenderer>().materials = matArray;
                            hasHitMob = true; //Now we hit the mob, so we set hadhitmob to true
                        }
                    }
                    World.Instance.mobManager.alligatorBrain.Health -= 1;
                    if(World.Instance.mobManager.alligatorBrain.Health == 0) {
                        lootTable.MobDrops("Alligator");
                        
                        World.Instance.uiScript.damagePanel.SetActive(false);

                        Destroy(World.Instance.mobManager.alligatorBrain.gameObject);
                    }
                }
            }

            public void SetFirstPersonCam(GameObject Player) {
                //Init Stuff
                transform.name = "First Person Cam";
                transform.tag = "Player Cam";
                transform.SetParent(Player.transform);

                //Add Components
                camCam = gameObject.AddComponent<Camera>();
                AudioListener camAL = gameObject.AddComponent<AudioListener>(); 

                //Modify
                transform.localPosition = new Vector3(0.0f, 0.8f, 0.0f);
                camCam.targetDisplay = 0;
            }

            public void SetThirdPersonCam(GameObject Player) {
                //Init Stuff
                transform.name = "Third Person Cam";
                transform.tag = "Player Cam";
                transform.SetParent(Player.transform);

                //Add Components
                Camera camCam = gameObject.AddComponent<Camera>();

                //Modify
                transform.localPosition = new Vector3(-5.0f, 2.0f, -5.0f);
                camCam.targetDisplay = 1;
            }
        }
    }
}
