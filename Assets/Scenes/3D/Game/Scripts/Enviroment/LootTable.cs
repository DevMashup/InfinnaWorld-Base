using UnityEngine;

using DevMashup.Base;

namespace DevMashup {
    namespace GamePlay {
        public class LootTable : MonoBehaviour {

            Vector3 dropOffset {get {return new Vector3(1.0f, 1.0f, 1.0f); } }


            public void BlockDrops(string blockName) {
                int rnd = Random.Range(0, 5);


                switch(blockName) {
                    case "Grass":
                        //Init Stuff
                        GameObject Grass = Instantiate(Resources.Load<GameObject>(Sources.ITEM_GRASS_BLOCK));
                        Grass.transform.name = "Grass";
                        Grass.transform.tag = "Item";
                        Grass.transform.SetParent(World.Instance.Containers[2].transform);

                        //Modify Stuff
                        Grass.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Grass.transform.position = World.Instance.playerObj.transform.position + dropOffset;

                        if(rnd == 4) {
                            GameObject dayStone = Instantiate(Resources.Load<GameObject>(Sources.PREFAB_DAY_STONE_OBJ));
                            dayStone.transform.name = "Day Stone";
                            dayStone.transform.tag = "Item";
                            dayStone.transform.SetParent(World.Instance.Containers[2].transform);
                            
                            
                            dayStone.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                            dayStone.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        }
                        break;
                    case "Sand":
                        //Init Stuff
                        GameObject Sand = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Sand Drop"));
                        Sand.transform.name = "Sand";
                        Sand.transform.tag = "Item";
                        Sand.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        //Modify Stuff
                        Sand.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Sand.transform.position = World.Instance.playerObj.transform.position + dropOffset;

                        if(rnd == 4) {
                            GameObject dayStone = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Percent/Day Stone"));
                            dayStone.transform.name = "Day Stone";
                            dayStone.transform.tag = "Item";
                            dayStone.transform.SetParent(World.Instance.Containers[2].transform);
                            
                            dayStone.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                            dayStone.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        }
                        break;
                    case "Snow":
                        GameObject Snow = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Snow Drop"));
                        Snow.transform.name = "Snow";
                        Snow.transform.tag = "Item";
                        Snow.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        Snow.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Snow.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        
                        if(rnd == 4) {
                            GameObject dayStone = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Percent/Day Stone"));
                            dayStone.transform.name = "Day Stone";
                            dayStone.transform.tag = "Item";
                            dayStone.transform.SetParent(World.Instance.Containers[2].transform);
                            
                            dayStone.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                            dayStone.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        }
                        break;
                    case "Dirt":
                        GameObject Dirt = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Dirt Drop"));
                        Dirt.transform.name = "Dirt";
                        Dirt.transform.tag = "Item";
                        Dirt.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        Dirt.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Dirt.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                    case "Stone":
                        GameObject Stone = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Stone Drop"));
                        Stone.transform.name = "Stone";
                        Stone.transform.tag = "Item";
                        Stone.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        Stone.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Stone.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                    case "Bedrock":
                        GameObject Bedrock = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Bedrock Drop"));
                        
                        Bedrock.transform.name = "Bedrock";
                        Bedrock.transform.tag = "Item";
                        Bedrock.transform.SetParent(World.Instance.Containers[2].transform);
                        Bedrock.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Bedrock.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                    case "Obsidian":
                        GameObject Obsidian = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Obsidian Drop"));
                        Obsidian.transform.name = "Obsidian";
                        Obsidian.transform.tag = "Item";
                        Obsidian.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        Obsidian.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Obsidian.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                    case "Wood":
                        GameObject Wood = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Wood Drop"));
                        Wood.transform.name = "Wood";
                        Wood.transform.tag = "Item";
                        Wood.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        Wood.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Wood.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                    case "Log":
                        GameObject Log = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Block Drops/Certain/Log Drop"));
                        Log.transform.name = "Log";
                        Log.transform.tag = "Item";
                        Log.transform.SetParent(World.Instance.Containers[2].transform);
                        
                        
                        Log.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                        Log.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                }
            }
        
            public void MobDrops(string mobName) {
                switch(mobName) {
                    case "Alligator":
                        GameObject AlligatorMeat = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Items/Mob Drops/Hostile/Alligator/Certain/Uncooked Alligator Meat"));
                        AlligatorMeat.transform.name = "Alligator";
                        AlligatorMeat.transform.tag = "Item";
                        AlligatorMeat.transform.SetParent(World.Instance.Containers[2].transform);


                        AlligatorMeat.transform.position = World.Instance.playerObj.transform.position + dropOffset;
                        break;
                }
            }
        }
    }
}
