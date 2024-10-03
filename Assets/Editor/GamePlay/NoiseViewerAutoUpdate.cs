using UnityEngine;
using UnityEditor;


namespace DevMashup {
    namespace GamePlay {
        [CustomEditor(typeof(World))]
        public class NoiseViewerAutoUpdate : Editor {
            // Function executed each time user interact with the inspector
            public override void OnInspectorGUI() {
                if (DrawDefaultInspector() && Application.isPlaying) {//If we changue some value in the NoiseTerrainViewer and there is a NoiseTerrainViewer we update the terrain
                    World world = (World)target;
                    world.CreateStartWorld();
                }
            }
        }
    }
}

