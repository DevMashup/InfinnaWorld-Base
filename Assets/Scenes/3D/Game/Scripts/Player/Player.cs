using System;
using System.Security;
using TMPro;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        /// <summary>
        /// A class hat contains stuff when a player hits a certain key, scrolls a mouse, click mouse...etc
        /// </summary>
        public class Player : MonoBehaviour
        {
            //Actions
            [HideInInspector]public Action DropItem;
            [HideInInspector]public Action MouseClick;





            [HideInInspector]public int Health = 20;

            [HideInInspector]public bool tpActivate = false;
            [HideInInspector]public bool hideStartPanel = false; 
            [HideInInspector]public bool hideDebugPanel = true;
            [HideInInspector]public bool hideAirContainer = true; //Move
            [HideInInspector]public bool hideInvPanel = true;
            [HideInInspector]public int playerChosesItem = 0;
            bool canCreateCameras = true;

            Vector3 spawnPos;
            public WholeInv wholeInv;
            Inventory inventory;




            int decreaseNumber = 9;
            
            void OnTriggerEnter(Collider other) {
                ItemWorld itemWorld = other.GetComponent<ItemWorld>();
                if(itemWorld != null) {
                    inventory.AddItem(itemWorld.GetItem());
                    itemWorld.DestroyItem();
                    
                    Debug.Log(inventory.GetItemList().Count);
                }    
            }


            // Update is called once per frame
            void Update() {
                //Need to have access to change these whether or not the game is paused or not
                if(Input.GetKeyDown(KeyCode.Escape)) {
                    World.Instance.isGamePaused = !World.Instance.isGamePaused;
                }
                else if(Input.GetKeyDown(KeyCode.E)) {
                    hideInvPanel = !hideInvPanel;
                }
                else if(Input.GetKeyDown(KeyCode.F3)) {
                    tpActivate = !tpActivate;
                }
                
                CheckVars();
            }

            void ClickButtonsorScroll() {

                //MOVEMENT
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) {
                    GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationY & ~RigidbodyConstraints.FreezePosition; //Unfreezes player

                    transform.position = Vector3.MoveTowards(transform.position, transform.position + (World.Instance.fpCamObj.transform.forward * Input.GetAxis("Vertical")) + (World.Instance.fpCamObj.transform.right * Input.GetAxis("Horizontal")), Time.deltaTime * 10f);
                }


                //UNUSUAL KEYS
                if(Input.GetKeyDown(KeyCode.F1)) {
                    hideStartPanel = !hideStartPanel;
                }
                else if(Input.GetKeyDown(KeyCode.F4)) {
                    hideDebugPanel = !hideDebugPanel;
                }
                else if(Input.GetKeyDown(KeyCode.LeftControl)) { //Show
                    hideAirContainer = !hideAirContainer;
                }
                else if(Input.GetKeyDown(KeyCode.D)) {
                    World.Instance.uiScript.airBubbles[decreaseNumber].SetActive(false);

                    decreaseNumber--;
                }
                else if(Input.GetKeyDown(KeyCode.Q)) {
                    if(DropItem != null) DropItem();
                } 
                else if(Input.GetMouseButtonDown(0)) {
                    if(MouseClick != null) MouseClick();
                }

                if(Input.mouseScrollDelta.y == 0) {
                    if(wholeInv.items[playerChosesItem] == null) {
                        World.Instance.uiScript.blockNames[0].SetActive(false);
                        Debug.Log("This is supposedto be null");
                    }
                    else {
                        Debug.Log("This is not be null");
                        World.Instance.uiScript.blockNames[0].SetActive(true);
                        //Debug.Log("You're selecting a: " + wholeInv.items[playerChosesItem].ID);
                        World.Instance.uiScript.blockNames[1].GetComponent<TextMeshProUGUI>().text = wholeInv.items[playerChosesItem].ID;
                    
                        MouseClick = () => {
                            inventory.UseItem(wholeInv.items[playerChosesItem]);
                        };

                        DropItem = () => {
                            Debug.Log("Item is" + wholeInv.items[playerChosesItem].ID);
                            Item duppedItem = new Item{itemType = wholeInv.items[playerChosesItem].itemType, Amount = wholeInv.items[playerChosesItem].Amount};
//
                            if(wholeInv.items[playerChosesItem].Amount > 1) {
                                inventory.DisposeItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                            else if(wholeInv.items[playerChosesItem].Amount == 1) {
                                inventory.RemoveItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                        };

                        if(Input.GetKeyDown(KeyCode.L)) {
                            SelectedItem(wholeInv.items[playerChosesItem]);
                        }
                    }
                }
                else if(Input.mouseScrollDelta.y > 0) {
                    World.Instance.uiScript.Outline.GetComponent<RectTransform>().anchoredPosition += new Vector2(60.0f, 0.0f);

                    playerChosesItem++;
                    if(playerChosesItem > 9) {
                        playerChosesItem = 0;
                        World.Instance.uiScript.Outline.GetComponent<RectTransform>().anchoredPosition = new Vector2(-270.0f, 0.0f);
                        World.Instance.uiScript.blockNames[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(-330.0f, 77.0f);
                    }

                    if(wholeInv.items[playerChosesItem] == null) {
                        World.Instance.uiScript.blockNames[0].SetActive(false);
                    }
                    else {
                        World.Instance.uiScript.blockNames[0].SetActive(true);

                        //Set and size the block name dynamically
                        World.Instance.uiScript.blockNames[1].GetComponent<TextMeshProUGUI>().text = wholeInv.items[playerChosesItem].ID;
                        World.Instance.uiScript.blockNames[0].GetComponent<RectTransform>().anchoredPosition += new Vector2(60.0f, 0.0f);



                        //Debug.Log("You've selected a " + hotbarInv.items[playerChosesItem].itemType);
                        MouseClick = () => {
                            //inventory.UseItem(wholeInv.items[playerChosesItem]);
                        };

                        DropItem = () => {
                            Item duppedItem = new Item{itemType = wholeInv.items[playerChosesItem].itemType, Amount = wholeInv.items[playerChosesItem].Amount};

                            if(wholeInv.items[playerChosesItem].Amount > 1) {
                                inventory.DisposeItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                            else if(wholeInv.items[playerChosesItem].Amount == 1) {
                                inventory.RemoveItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                        };
                        
                    }
                }
                else if(Input.mouseScrollDelta.y < 0) {
                    World.Instance.uiScript.Outline.GetComponent<RectTransform>().anchoredPosition -= new Vector2(60.0f, 0.0f);

                    playerChosesItem--;
                    if(playerChosesItem < 0) {
                        playerChosesItem = 9;
                        World.Instance.uiScript.Outline.GetComponent<RectTransform>().anchoredPosition = new Vector2(270.0f, 0.0f);
                        World.Instance.uiScript.blockNames[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(330.0f, 77.0f);
                    }

                    if(wholeInv.items[playerChosesItem] == null) {
                        World.Instance.uiScript.blockNames[0].SetActive(false);
                    }
                    else {
                        World.Instance.uiScript.blockNames[0].SetActive(true);

                        //Set and size the block name dynamically
                        World.Instance.uiScript.blockNames[1].GetComponent<TextMeshProUGUI>().text = wholeInv.items[playerChosesItem].ID;
                        World.Instance.uiScript.blockNames[0].GetComponent<RectTransform>().anchoredPosition -= new Vector2(60.0f, 0.0f);



                        //Debug.Log("You've selected a " + hotbarInv.items[playerChosesItem].itemType);
                        MouseClick = () => {
                            //inventory.UseItem(wholeInv.items[playerChosesItem]);
                        };

                        DropItem = () => {
                            Item duppedItem = new Item{itemType = wholeInv.items[playerChosesItem].itemType, Amount = wholeInv.items[playerChosesItem].Amount};

                            if(wholeInv.items[playerChosesItem].Amount > 1) {
                                inventory.DisposeItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                            else if(wholeInv.items[playerChosesItem].Amount == 1) {
                                inventory.RemoveItem(wholeInv.items[playerChosesItem]);
                                ItemWorld.DropItem(transform.position, duppedItem);
                            }
                        };
                        
                    }
                }
            }

            void CheckVars() {
                if(!World.Instance.isGamePaused && !World.Instance.isGameOver && !World.Instance.isEnviromentPaused && World.Instance.isStartingWorldDoneLoading) { //The game is not frozen
                    GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezeRotationY & ~RigidbodyConstraints.FreezePosition;

                    if(canCreateCameras) {
                        World.Instance.CreateObject("First Person Camera");
                        World.Instance.CreateObject("Third Person Camera");
                    
                        canCreateCameras = false;
                    }
                    ClickButtonsorScroll();
                }
                else { //The game is frozen
                    GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
            }


            /// <summary>
            /// Creates the player
            /// </summary>
            public void SetPlayer(GameObject world) {
                GameObject playerContainer = new GameObject("Player Container");
                playerContainer.transform.tag = "Container";
                playerContainer.transform.SetParent(World.Instance.Containers[2].transform);


                tpActivate = false;
                GameObject Capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                spawnPos = new Vector3((World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE, World.Instance.worldConfig.surfaceLevel + 10.0f, (World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE);
                
                //Init Stuff
                transform.name = "Player";
                transform.tag = "Player";
                transform.SetParent(playerContainer.transform);

                //Add Components
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                

                //Modify Components
                transform.position = spawnPos;
                meshFilter.mesh = Capsule.GetComponent<MeshFilter>().mesh;
                meshRenderer.material = Resources.Load<Material>("Visuals/Materials/Player/Player");
                rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                capsuleCollider.height = 2f;

                Destroy(Capsule);
                
                //Inventory
                inventory = new Inventory(UseItem, DisposeItem);
                wholeInv = World.Instance.uiScript.hotBar.GetComponent<WholeInv>();
                wholeInv.FindObj();
                wholeInv.SetPlayer(gameObject.transform);
                wholeInv.SetInventory(inventory);
            }

            public void Respawn() {
                spawnPos = new Vector3((World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE, World.Instance.worldConfig.surfaceLevel + 10.0f, (World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE);
            }

            void UseItem(Item item) {
                switch(item.itemType) {
                    case Item.ItemTypes.Mutation_Remote:
                        World.Instance.uiScript.mrTextIndex++;

                        if(World.Instance.uiScript.mrTextIndex > 4) {
                            World.Instance.uiScript.mrTextIndex = 1;
                        }

                        World.Instance.uiScript.ChangeMRText();

                        break;
                    case Item.ItemTypes.Bedrock_Block:
                        Debug.Log("There is no use action for bedrock");
                        break;
                    case Item.ItemTypes.Dirt_Block:
                        Debug.Log("There is no use action for dirt");
                        break;
                    case Item.ItemTypes.Grass_Block:
                        Debug.Log("There is no use action for Grass Blocks");
                        break;
                    case Item.ItemTypes.Day_Stone:
                        Debug.Log("There is no use action for Day Stones");
                        break;
                    case Item.ItemTypes.Log_Block:
                        Debug.Log("There is no use action for log block");
                        break;
                    case Item.ItemTypes.Obsidian_Block:
                        Debug.Log("There is no use action for obsidian block");
                        break;
                    case Item.ItemTypes.Sand_Block:
                        Debug.Log("There is no use action for Sand block");
                        break;
                    case Item.ItemTypes.Snow_Blow:
                        Debug.Log("There is no use action for snow block");
                        break;
                    case Item.ItemTypes.Stone_Block:
                        Debug.Log("There is no use action for stone block");
                        break;
                    case Item.ItemTypes.Uncooked_Alligator_Meat:
                        Debug.Log("Hold this in your handand play an animation");
                        break;
                }
            }

            void DisposeItem(Item item) {
                switch(item.itemType) {
                    case Item.ItemTypes.Bedrock_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Bedrock_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Dirt_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Dirt_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Day_Stone:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Day_Stone, Amount = 1});
                        break;
                    case Item.ItemTypes.Grass_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Grass_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Log_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Log_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Mutation_Remote:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Mutation_Remote, Amount = 1});
                        break;
                    case Item.ItemTypes.Obsidian_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Obsidian_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Sand_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Sand_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Snow_Blow:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Snow_Blow, Amount = 1});
                        break;
                    case Item.ItemTypes.Stone_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Stone_Block, Amount = 1});
                        break;
                    case Item.ItemTypes.Uncooked_Alligator_Meat:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Uncooked_Alligator_Meat, Amount = 1});
                        break;
                    case Item.ItemTypes.Wood_Block:
                        inventory.RemoveItem(new Item{itemType = Item.ItemTypes.Wood_Block, Amount = 1});
                        break;
                    default:
                        Debug.Log("HI");
                        break;
                }
            }
        
            void SelectedItem(Item item) {
                switch(item.itemType) {
                    case Item.ItemTypes.Mutation_Remote:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Bedrock_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Dirt_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Grass_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Day_Stone:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Log_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Obsidian_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Sand_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Snow_Blow:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Stone_Block:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                    case Item.ItemTypes.Uncooked_Alligator_Meat:
                        Debug.Log("You're selecting the " + wholeInv.items[playerChosesItem].ID);
                        break;
                }
            }
        }
    }
}
