using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        [CreateAssetMenu(fileName = "Block", menuName = "InfinnaWorld/Enviroment/Blocks")]
        public class Blocks : ScriptableObject {
            public Material Atlas; //Texture Atlas for blocks
            public BlockTypes[] blockTypes;

        }

        [System.Serializable]
        public struct BlockTypes {
            public string Name;
        }
    }
}
