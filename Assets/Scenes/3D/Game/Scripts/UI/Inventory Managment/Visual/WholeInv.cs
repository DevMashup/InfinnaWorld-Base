using System.Data;
using DevMashup.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DevMashup {
    namespace GamePlay {
        public class WholeInv : MonoBehaviour {
            public Inventory inventory;
            Transform itemSlotContainer;
            Transform itemSlotTemplate;

            Transform Player;

            [HideInInspector]public Item[] items;
            [HideInInspector]public RectTransform rowOneContainer;
            int itemIndex;
            int[] invValue;
            [HideInInspector]public RectTransform[] itemRect;
            RectTransform[] hotbarRects;


            void Start()
            {
                
                
            }

            void SetInvisibleUI() {
                for(int i = 0; i < inventory.GetItemList().Count; i++) {
                    if(items[i].itemType == Item.ItemTypes.Invisible_UI) {
                        //itemRect[i].Find("Hotbar Slot Draggable").Find("Image Template").GetComponent<Image>().color = Color.clear;
                        //itemRect[i].Find("Hotbar Slot Draggable").Find("Item Amount(Txt)").GetComponent<TextMeshProUGUI>().text = " ";
                    }
                }
            }

            public void StartHotbar() {
                for(int i = 0; i < 10; i++) {
                    hotbarRects[i].Find("Hotbar Slot Draggable").Find("Image Template").gameObject.GetComponent<Image>().sprite = 
                    itemRect[i].Find("Hotbar Slot Draggable").Find("Image Template").gameObject.GetComponent<Image>().sprite;
                }
            }

            public void DeleteAndRefrenceRowOne(string Mode, int dragging, int dragged) {
                int draggingIndex = dragging - 1;
                int draggedIndex = dragged - 1;

                int x = 0;
                int y = 0;
                float itemSlotDstX = 60.0f;
                float itemSlotDstY = 0.0f;



                //Switcharoo stuff
                if(dragging != dragged) {
                    if(items[draggingIndex] == null) {
                        items[draggingIndex] = items[draggedIndex];
                        items[draggedIndex] = null; 

                        int tempValue = dragged;
                        dragged = dragging;
                        dragging = tempValue;
                    }
                    else if(items[draggedIndex] == null) {
                        items[draggedIndex] = items[draggingIndex];
                        items[draggingIndex] = null;

                        int tempValue = dragging;
                        dragging = dragged;
                        dragged = tempValue; 
                    }
                    else if(items[draggingIndex] != null&& items[draggedIndex] != null) {
                        Item tempItem = items[draggingIndex];
                        items[draggingIndex] = items[draggedIndex];
                        items[draggedIndex] = tempItem;

                        int tempValue = dragging;
                        dragging = dragged;
                        dragged = tempValue; 
                    }                        
                }


                //Equal the cooresponding items in the first row to the hotbar
                if(Mode == "Start") {
                    for(int i = 0; i < 10; i++) {
                        hotbarRects[i] = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                        hotbarRects[i].gameObject.SetActive(true);
                        hotbarRects[i].transform.name = "Item " + i + 1 + "(Refrenced)";
                        hotbarRects[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        x++;

                        //if(x == 9) {
                        //    x = 0;
                        //}
                    }
                }
                else {
                    foreach(Transform child in itemSlotContainer) {
                        if(child == itemSlotTemplate) continue;
                        Destroy(child.gameObject);
                    }


                    for(int i = 0; i < 10; i++) {
                        hotbarRects[i] = Instantiate(rowOneContainer.GetChild(i), itemSlotContainer).GetComponent<RectTransform>();
                        hotbarRects[i].gameObject.SetActive(true);
                        hotbarRects[i].transform.name = "Item " + i + 1 + "(Refrenced)";
                        hotbarRects[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        x++;

                        //if(x == 9) {
                        //    x = 0;
                        //}
                    }
                }

            }

            public void UpdateRowOne() {

            }

            public void FindObj() {
                itemSlotContainer = transform.Find("Hotbar Slot Container");
                itemSlotTemplate = itemSlotContainer.Find("Hotbar Slot Template");
            }

            public void SetInventory(Inventory inventory) {
                this.inventory = inventory;
                inventory.OnItemListChanged += Inventory_OnItemListChanged;

                CreateInventory();
            } 
            
            public void SetPlayer(Transform Player) {
                this.Player = Player;
            }

            void Inventory_OnItemListChanged(object sender, System.EventArgs e) {
                RefreshItemInv();
            }

            void CreateInventory() {
                foreach(Transform child in itemSlotContainer) {
                    if(child == itemSlotTemplate) continue;
                    Destroy(child.gameObject);
                }
                int x = 0;
                int y = 0;
                float itemSlotDstX = 60.0f;
                float itemSlotDstY = 0.0f;

                items = new Item[40];
                itemRect = new RectTransform[40];
                hotbarRects = new RectTransform[10];
                invValue = new int[40];
                itemIndex = 0;

                foreach(Item item in inventory.GetItemList()) {
                    items[itemIndex] = item;

                    itemIndex++;
                }

                itemIndex = 0;

                for(int i = 0; i < 40; i++) {


                    int nameExtended = itemIndex + 1;

                    itemRect[itemIndex] = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                    itemRect[itemIndex].gameObject.SetActive(true);
                    itemRect[itemIndex].gameObject.name = "Item " + nameExtended;
                    itemRect[itemIndex].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);

                    x++;
                    itemIndex++;

                    if(x > 10) {
                        inventory.createWholeInv = true;
                    }
                }

                itemIndex = 0;

                foreach(Item item in inventory.GetItemList()) {
                    Image image = itemRect[itemIndex].Find("Hotbar Slot Draggable").Find("Image Template").GetComponent<Image>();
                    TextMeshProUGUI TMP = itemRect[itemIndex].Find("Hotbar Slot Draggable").Find("Item Amount(Txt)").GetComponent<TextMeshProUGUI>();
                    image.sprite = items[itemIndex].GetSprite();

                    if(items[itemIndex].Amount > 1) {
                        TMP.SetText(items[itemIndex].Amount.ToString());
                    }
                    else {
                        TMP.SetText(" ");
                    }

                    itemIndex++;
                }

                itemIndex = 0;
                x = 0;
            }

            public void RefreshItemInv() {
                foreach(Transform child in rowOneContainer) {
                    if(items[World.Instance.playerScript.playerChosesItem] != null) {
                        if(child == itemRect[World.Instance.playerScript.playerChosesItem] && items[World.Instance.playerScript.playerChosesItem].Amount < 1) {
                            child.Find("Hotbar Slot Draggable").Find("Image Template").GetComponent<Image>().sprite = Resources.Load<Sprite>(Sources.TEX_BUTTON_REG_UI);
                        }
                    }
                }

                foreach(Transform child in itemSlotContainer) {
                    if(child == itemSlotTemplate) continue;
                    Destroy(child.gameObject);
                }
                
                itemIndex = 0;

                foreach(Item item in inventory.GetItemList()) {

                    TextMeshProUGUI TMP = itemRect[itemIndex].Find("Hotbar Slot Draggable").Find("Item Amount(Txt)").GetComponent<TextMeshProUGUI>();

                    if(items[itemIndex] != null) {
                        if(items[itemIndex].Amount > 1) {
                            TMP.SetText(items[itemIndex].Amount.ToString());
                        }
                        else {
                            TMP.SetText(" ");
                        }
                    }

                    itemIndex++;
                }
                DeleteAndRefrenceRowOne("Refresh", 0, 0);
            }

            public void OrganizeInv() {
                Debug.Log("Organized");
                //Varabile initialization
                Transform row1 = World.Instance.uiScript.invPanel.transform.Find("Inventory Screen").Find("Row 1");
                Transform row2 = World.Instance.uiScript.invPanel.transform.Find("Inventory Screen").Find("Row 2");
                Transform row3 = World.Instance.uiScript.invPanel.transform.Find("Inventory Screen").Find("Row 3");
                Transform row4 = World.Instance.uiScript.invPanel.transform.Find("Inventory Screen").Find("Row 4");

                rowOneContainer = Instantiate(itemSlotContainer, row1).GetComponent<RectTransform>();
                RectTransform rowTwoContainer = Instantiate(itemSlotContainer, row2).GetComponent<RectTransform>();
                RectTransform rowThreeContainer = Instantiate(itemSlotContainer, row3).GetComponent<RectTransform>();
                RectTransform rowFourContainer = Instantiate(itemSlotContainer, row4).GetComponent<RectTransform>();

                int x = 0;
                int y = 0;
                float itemSlotDstX = 60.0f;
                float itemSlotDstY = 70.0f;


                //We need to delete every child in each row otherwise it'll copy the hotbar
                //for(int i = 0; i < rowOneContainer.childCount; i++) {
                //    if(i > 9) {
                //        Destroy(rowOneContainer.GetChild(i).gameObject);
                //    } 
                //}

                foreach(Transform child in rowOneContainer) {
                    Destroy(child.gameObject);
                }

                foreach(Transform child in rowTwoContainer) {
                    Destroy(child.gameObject);
                }

                foreach(Transform child in rowThreeContainer) {
                    Destroy(child.gameObject);
                }

                foreach(Transform child in rowFourContainer) {
                    Destroy(child.gameObject);
                }



                for(int i = 0; i < 40; i++){
                    if(i < 10) {
                        int extendedNum = i + 1;
                        itemRect[i].transform.SetParent(rowOneContainer);
                        itemRect[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        invValue[i] = extendedNum;
                        itemRect[i].Find("Hotbar Slot Draggable").GetComponent<DraggableItems>().invValue = invValue[i];
                        
                        x++;
//
                        if(i == 9) {
                            x = 0;
                        }
                    }
                    if(i > 9 && i < 20) {
                        int extendedNum = i + 1;
                        itemRect[i].transform.SetParent(rowTwoContainer);
                        itemRect[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        x++;
                        invValue[i] = extendedNum;
                        itemRect[i].Find("Hotbar Slot Draggable").GetComponent<DraggableItems>().invValue = invValue[i];

                        if(i == 19) {
                            x = 0;
                        }
                    }
                    else if(i > 19 && i < 30) {
                        int extendedNum = i + 1;
                        itemRect[i].transform.SetParent(rowThreeContainer);
                        itemRect[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        x++;
                        invValue[i] = extendedNum;
                        itemRect[i].Find("Hotbar Slot Draggable").GetComponent<DraggableItems>().invValue = invValue[i];

                        if(i == 29) {
                            x = 0;
                        }
                    }
                    else if(i > 29 && i < 40){
                        int extendedNum = i + 1;
                        itemRect[i].transform.SetParent(rowFourContainer);
                        itemRect[i].anchoredPosition = new Vector2(x * itemSlotDstX, y * itemSlotDstY);
                        x++;
                        invValue[i] = extendedNum;
                        itemRect[i].Find("Hotbar Slot Draggable").GetComponent<DraggableItems>().invValue = invValue[i];
                    }
                }

                //SetInvisibleUI();
                DeleteAndRefrenceRowOne("Start", 0, 0);
                StartHotbar();
            }
        }
    }
}
