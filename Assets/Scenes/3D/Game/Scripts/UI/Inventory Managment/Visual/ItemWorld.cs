using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class ItemWorld : MonoBehaviour {
            Item item;
            MeshFilter meshFilter;
            MeshRenderer meshRenderer;

            //void Start() {
            //    meshFilter = GetComponent<MeshFilter>();
            //}

            public static ItemWorld SpawnItemWorld(Vector3 Pos, Item item) {
                Transform transform = Instantiate(World.Instance.scriptableObjects.items.Prefab.transform, Pos, Quaternion.identity);

                ItemWorld itemWorld = transform.GetComponent<ItemWorld>();
                itemWorld.SetItem(item);

                return itemWorld;
            }

            public void SetItem(Item item) {
                meshFilter = GetComponent<MeshFilter>();
                meshRenderer = GetComponent<MeshRenderer>();


                this.item = item;
                meshFilter.mesh = item.GetMesh();
                meshRenderer.material = item.GetMaterial();
            }

            public static ItemWorld DropItem(Vector3 Pos, Item item) {
                ItemWorld itemWorld = SpawnItemWorld(Pos + Vector3.one, item);
                return itemWorld;
            }

            public Item GetItem() {
                return item;
            }

            public void DestroyItem() {
                Destroy(gameObject);
            }
        }
    }
}
