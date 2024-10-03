using UnityEngine;
using UnityEditor;

namespace DevMashup {
    namespace GamePlay {
        [CustomEditor(typeof(World))]
        public class NoiseTerrainAutoUpdate : Editor {
    
    
            // Function executed each time user interact with the inspector
            public override void OnInspectorGUI() {
                if(DrawDefaultInspector() && Application.isPlaying) { //If we changue some value in the  NoiseManager and there is a NoiseTerrainViewer we update the terrain
                    World obj = (World)target;
                    World world  = obj.gameObject.GetComponent<World>();
            
                    if (world != null) { 
                        world.CreateStartWorld();
                    }
                }
            }
        }
    }
}




