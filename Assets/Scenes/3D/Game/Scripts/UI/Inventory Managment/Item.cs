using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class Item {
            public ItemTypes itemType;
            public int Amount;
            public string ID;

            /// <summary>
            /// The type of item
            /// </summary>
            public enum ItemTypes {
                Bedrock_Block,
                Dirt_Block,
                Day_Stone,
                Grass_Block,
                Log_Block,
                Mutation_Remote,
                Obsidian_Block,
                Sand_Block,
                Snow_Blow,
                Stone_Block,
                Uncooked_Alligator_Meat,
                Wood_Block,




                Invisible_UI
            }

            public Material GetMaterial() {
                //ALPHABETICAL
                switch(itemType) {
                    case ItemTypes.Bedrock_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[0].itemMat;
                    case ItemTypes.Dirt_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[1].itemMat;
                    case ItemTypes.Day_Stone:
                        return World.Instance.scriptableObjects.items.itemInfo[2].itemMat;
                    case ItemTypes.Grass_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[3].itemMat;
                    case ItemTypes.Log_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[4].itemMat;
                    case ItemTypes.Mutation_Remote:
                        return World.Instance.scriptableObjects.items.itemInfo[5].itemMat;
                    case ItemTypes.Obsidian_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[6].itemMat;
                    case ItemTypes.Sand_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[7].itemMat;
                    case ItemTypes.Snow_Blow:
                        return World.Instance.scriptableObjects.items.itemInfo[8].itemMat;
                    case ItemTypes.Stone_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[9].itemMat;
                    case ItemTypes.Uncooked_Alligator_Meat:
                        return World.Instance.scriptableObjects.items.itemInfo[10].itemMat;
                    case ItemTypes.Wood_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[11].itemMat;
                    default:
                        //Debug.Log("There is no item in the game with this name");
                        return World.Instance.scriptableObjects.items.itemInfo[0].itemMat;
                }
            }

            public Mesh GetMesh() {
                //ALPHABETICAL
                switch(itemType) {
                    case ItemTypes.Bedrock_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[0].itemMesh;
                    case ItemTypes.Dirt_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[1].itemMesh;
                    case ItemTypes.Day_Stone:
                        return World.Instance.scriptableObjects.items.itemInfo[2].itemMesh;
                    case ItemTypes.Grass_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[3].itemMesh;
                    case ItemTypes.Log_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[4].itemMesh;
                    case ItemTypes.Mutation_Remote:
                        return World.Instance.scriptableObjects.items.itemInfo[5].itemMesh;
                    case ItemTypes.Obsidian_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[6].itemMesh;
                    case ItemTypes.Sand_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[7].itemMesh;
                    case ItemTypes.Snow_Blow:
                        return World.Instance.scriptableObjects.items.itemInfo[8].itemMesh;
                    case ItemTypes.Stone_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[9].itemMesh;
                    case ItemTypes.Uncooked_Alligator_Meat:
                        return World.Instance.scriptableObjects.items.itemInfo[10].itemMesh;
                    case ItemTypes.Wood_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[11].itemMesh;
                    default:
                        //Debug.Log("There is no item in the game with this name");
                        return World.Instance.scriptableObjects.items.itemInfo[0].itemMesh;
                }
            }

            public Sprite GetSprite() {
                //ALPHABETICAL
                switch(itemType) {
                    case ItemTypes.Bedrock_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[0].sprite;
                    case ItemTypes.Dirt_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[1].sprite;
                    case ItemTypes.Day_Stone:
                        return World.Instance.scriptableObjects.items.itemInfo[2].sprite;
                    case ItemTypes.Grass_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[3].sprite;
                    case ItemTypes.Log_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[4].sprite;
                    case ItemTypes.Mutation_Remote:
                        return World.Instance.scriptableObjects.items.itemInfo[5].sprite;
                    case ItemTypes.Obsidian_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[6].sprite;
                    case ItemTypes.Sand_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[7].sprite;
                    case ItemTypes.Snow_Blow:
                        return World.Instance.scriptableObjects.items.itemInfo[8].sprite;
                    case ItemTypes.Stone_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[9].sprite;
                    case ItemTypes.Uncooked_Alligator_Meat:
                        return World.Instance.scriptableObjects.items.itemInfo[10].sprite;
                    case ItemTypes.Wood_Block:
                        return World.Instance.scriptableObjects.items.itemInfo[11].sprite;
                    case ItemTypes.Invisible_UI:
                        return World.Instance.scriptableObjects.items.errorSprite;
                    default:
                        Debug.Log("There is no item in the game with this name");
                        return World.Instance.scriptableObjects.items.errorSprite;
                }
            }

            /// <summary>
            /// Determines which items are stackackable
            /// </summary>
            public bool IsStackable() {
                switch(itemType) {
                    case ItemTypes.Grass_Block:
                    case ItemTypes.Sand_Block:
                    case ItemTypes.Snow_Blow:
                    case ItemTypes.Dirt_Block:
                    case ItemTypes.Stone_Block:
                    case ItemTypes.Bedrock_Block:
                    case ItemTypes.Obsidian_Block:
                    case ItemTypes.Wood_Block:
                    case ItemTypes.Log_Block:
                    case ItemTypes.Day_Stone:
                    case ItemTypes.Uncooked_Alligator_Meat:
                        return true;
                    case ItemTypes.Invisible_UI:
                    case ItemTypes.Mutation_Remote:
                        return false;
                    default:
                        //Debug.Log("There is no item in the game with this name");
                        return true;
                }
            }
        }
    }
}
