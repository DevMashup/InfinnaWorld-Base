using UnityEngine;
using UnityEditor;

namespace DevMashup {
    namespace GamePlay {
        [CustomEditor(typeof(Biome), true)]
        public class BiomeAutoUpdate : Editor {


        // Function executed each time user interact with the inspector
        public override void OnInspectorGUI() {
            if (DrawDefaultInspector() && Application.isPlaying) {//If we changue some value inside a biome and there is a NoiseTerrainViewer we update the terrai
                Biome obj = (Biome)target;
                World world = GameObject.Find("World").GetComponent<World>();
                    if (world != null) {
                        world.CreateStartWorld();
                    }
                }
            }
        }
    }
}
