using UnityEngine;
using UnityEngine.EventSystems;

namespace InfinnaWorld {
    namespace MainMenu {
        public class GameManager : MonoBehaviour
        {
            void Start() {
                CreateScene();    
            }

            void CreateScene() {
                CreateObject("Camera");
                CreateObject("Sun");
                CreateObject("Event System");
                CreateObject("UI");

                Destroy(this.gameObject);
            }

            void CreateObject(string objName) {
                switch(objName) {
                    case "Sun":
                        //Init
                        GameObject sunObj = new GameObject("Sun");
                        sunObj.transform.tag = "Sun";
                        sunObj.transform.SetSiblingIndex(0);

                        //Add components
                        Light sunLight = sunObj.AddComponent<Light>();

                        //Modify
                        sunObj.transform.eulerAngles = new Vector3(50.0f, 30.0f, 0.0f);
                        sunLight.type = LightType.Directional;
                        sunLight.color = new Color(1.00000000000f, 0.95686274509f, 0.83921568627f);
                        
                        break;
                    case "Event System":
                        //Init 
                        GameObject esObj = new GameObject("Event System");
                        esObj.transform.tag = "Event System";

                        //Add Components
                        EventSystem esES = esObj.AddComponent<EventSystem>();
                        StandaloneInputModule esSIM = esObj.AddComponent<StandaloneInputModule>();

                        break;
                    case "UI":
                        GameObject uiObj = new GameObject();
                        UI uiScript = uiObj.AddComponent<UI>();
                        uiScript.SetCanvas();

                        break;
                    case "Camera": 
                        //Init Stuff
                        GameObject camObj = new GameObject();
                        camObj.transform.name = "Main Camera";
                        camObj.transform.tag = "Cam";

                        //AddComponents
                        Camera camCam = camObj.AddComponent<Camera>();
                        AudioListener camAL = camObj.AddComponent<AudioListener>();

                        //Modify
                        camCam.targetDisplay = 0;
                        break;
                }
            }
        }
    }
}
