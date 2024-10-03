using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class Inventory {
            public event EventHandler OnItemListChanged; //Event to add/remove something from our inventory
            public List<Item> itemList;
            Action<Item> useItem;
            Action<Item> disposeItem;
            
            public bool createWholeInv = false;

            /// <summary>
            /// Initilizes the list and eaction
            /// </summary>
            public Inventory(Action<Item> useItem, Action<Item> disposeItem) {
                this.useItem = useItem;
                this.disposeItem = disposeItem;
                itemList = new List<Item>();

                AddItem(new Item{itemType = Item.ItemTypes.Dirt_Block, Amount = 5, ID = "Dirt"}); //Player Starts of with this
                AddItem(new Item{itemType = Item.ItemTypes.Bedrock_Block, Amount = 5, ID = "Bedrock"});
                AddItem(new Item{itemType = Item.ItemTypes.Day_Stone, Amount = 5, ID = "Day Stone"});
                AddItem(new Item{itemType = Item.ItemTypes.Grass_Block, Amount = 5, ID = "Grass"});
                AddItem(new Item{itemType = Item.ItemTypes.Log_Block, Amount = 5, ID = "Log"});
                AddItem(new Item{itemType = Item.ItemTypes.Mutation_Remote, Amount = 1, ID = "Mutation Remote"});
                AddItem(new Item{itemType = Item.ItemTypes.Obsidian_Block, Amount = 5, ID = "Obsidian"});
                AddItem(new Item{itemType = Item.ItemTypes.Sand_Block, Amount = 5, ID = "Sand"});
                AddItem(new Item{itemType = Item.ItemTypes.Snow_Blow, Amount = 5, ID = "Snow Block"});
                AddItem(new Item{itemType = Item.ItemTypes.Stone_Block, Amount = 5, ID = "Stone"});
                AddItem(new Item{itemType = Item.ItemTypes.Uncooked_Alligator_Meat, Amount = 5, ID = "Uncooked Alligator Meat"});
                AddItem(new Item{itemType = Item.ItemTypes.Wood_Block, Amount = 5, ID = "Wood"});
            }

            public void AddItem(Item item) {
                if(item.IsStackable()) {
                    bool isItemInInv = false;
                    foreach(Item invItem in itemList) {
                        if(invItem.itemType == item.itemType) {
                            invItem.Amount += item.Amount;
                            isItemInInv = true;
                        }
                    }

                    if(!isItemInInv) {
                        itemList.Add(item);
                    }
                }
                else {
                    itemList.Add(item);
                }

                OnItemListChanged?.Invoke(this, EventArgs.Empty);
            }

            void AddMultipleItems(int Amount) {
                for(int i = 0; i < Amount; i++) {
                    AddItem(new Item{itemType = Item.ItemTypes.Invisible_UI, Amount = 5, ID = " "});
                }
            }

            public void RemoveItem(Item item) {
                if(item.IsStackable()) {
                    Item itemInInv = null;
                    foreach(Item invItem in itemList) {
                        if(invItem.itemType == item.itemType) {
                            invItem.Amount -= item.Amount;
                            itemInInv = invItem;
                        }
                    }

                    if(itemInInv != null && itemInInv.Amount <= 0) {
                        itemList.Remove(item);
                    }
                }
                else {
                    itemList.Remove(item);
                }

                OnItemListChanged?.Invoke(this, EventArgs.Empty);
            }

            public List<Item> GetItemList() {
                return itemList;
            }

            public void UseItem(Item item) {
                useItem(item);
            }

            public void DisposeItem(Item item) {
                disposeItem(item);
            }
        }
    }
}
