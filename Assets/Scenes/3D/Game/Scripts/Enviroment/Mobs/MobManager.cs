using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class MobManager : MonoBehaviour {
        
            [HideInInspector]public AlligatorBrain alligatorBrain;
            [HideInInspector]public GameObject Alligator;
            
            
            
            /// <summary>
            /// Constructor to spawn, despawn mobs
            /// </summary>
            public MobManager(World world) {
                SpawnAlligator(world);
            }

            void SpawnAlligator(World world) {
                Alligator = Instantiate(Resources.Load<GameObject>("Prefabs/GamePlay/3D/Enviroment/Mobs/Hostile/Alligator/Baby"));
                alligatorBrain = Alligator.AddComponent<AlligatorBrain>();

                alligatorBrain.CreateAlligator();
            }
        }
    }
}
