using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        [CreateAssetMenu(fileName = "Items", menuName = "InfinnaWorld/Enviroment/Items")]
        public class Items : ScriptableObject {
            public GameObject Prefab;
            public Sprite errorSprite;
            public ItemInfo[] itemInfo;

        }

        [System.Serializable]
        public struct ItemInfo {
            public string Name;
            public Mesh itemMesh;
            public Material itemMat;
            public Sprite sprite;
        }
    }
}
