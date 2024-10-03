using TMPro;
using UnityEngine;
using UnityEngine.UI;

using DevMashup.Base;
using System.Runtime.Serialization.Formatters;
using System.Runtime.CompilerServices;

namespace DevMashup {
    namespace GamePlay {
        public class UI : MonoBehaviour {
            
            GameObject pausePanel; //Not public because we just check a bool from another class 
            GameObject gameOverPanel;
            GameObject mrTxt;
            GameObject startPanel;
            [HideInInspector]public GameObject invPanel;
            GameObject debugPanel;
            GameObject airBubbleCointainer;
            [HideInInspector]public GameObject imageTemplate;
            [HideInInspector]public GameObject hotBar;
            [HideInInspector]public GameObject damagePanel;
            [HideInInspector]public GameObject dayText;
            [HideInInspector]public GameObject[] airBubbles;
            [HideInInspector]public GameObject[] blockNames;
            [HideInInspector]public GameObject[] emptyHearts;
            [HideInInspector]public GameObject[] damageTris;
            [HideInInspector]public GameObject Outline;

            Vector2 firstPos = new Vector2(-270.0f, 0.0f);

            [HideInInspector]public int mrTextIndex = 0;
            string[] mrtxtTXT = {"NULL", "MODE: Go to Previous Mutation", "MODE: Go to Base Animal", "MODE: Lock Mutation", "MODE: Mutant Info"};
            
            void Update() {
                //PAUSE MENU
                if(World.Instance.isGamePaused && !pausePanel) { //If the player presses escape, but the pause meny does not exist yet
                    CreatePausePanel();
                    Cursor.lockState = CursorLockMode.None;
                }
                else if(World.Instance.isGamePaused && pausePanel) {
                    pausePanel.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }
                else if(!World.Instance.isGamePaused && pausePanel){
                    pausePanel.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;
                }


                //Game over
                if(World.Instance.isGameOver && !gameOverPanel) { //If the player presses escape, but the pause meny does not exist yet
                    CreateGameOverPanel();
                    Cursor.lockState = CursorLockMode.None;
                }
                else if(World.Instance.isGameOver && gameOverPanel) {
                    gameOverPanel.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }
                else if(!World.Instance.isGameOver && gameOverPanel){
                    gameOverPanel.SetActive(false);
                    Cursor.lockState = CursorLockMode.None;
                }




                //DEBUG MENU
                if(!World.Instance.playerObj.GetComponent<Player>().hideDebugPanel && !debugPanel) { //If the player presses escape, but the pause meny does not exist yet
                    CreateDebugPanel();
                    dayText.transform.SetParent(debugPanel.transform);
                    dayText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -29.0f);
                    dayText.GetComponent<RectTransform>().localScale = Vector3.one;
                }
                else if(World.Instance.playerObj.GetComponent<Player>().hideDebugPanel && debugPanel) {
                    debugPanel.SetActive(false);
                }
                else if(!World.Instance.playerObj.GetComponent<Player>().hideDebugPanel && debugPanel){
                    debugPanel.SetActive(true);
                }


                //If you have more than 10 items in your inventory
                if(World.Instance.playerScript.wholeInv.inventory.createWholeInv && !invPanel) {
                    CreateInventory();
                }



                if(!World.Instance.playerObj.GetComponent<Player>().hideInvPanel && !invPanel) {
                    CreateInventory();
                }
                else if(World.Instance.playerObj.GetComponent<Player>().hideInvPanel && invPanel) {
                    invPanel.SetActive(false);
                    World.Instance.isEnviromentPaused = true;
                }
                else if(!World.Instance.playerObj.GetComponent<Player>().hideInvPanel && invPanel) {
                    invPanel.SetActive(true);
                    World.Instance.isEnviromentPaused = false;
                }



                //START PANEL
                if(World.Instance.playerObj.GetComponent<Player>().hideStartPanel) {
                    startPanel.SetActive(false);
                }
                else {
                    startPanel.SetActive(true);
                }

                //Air Cointainer
                if(World.Instance.playerObj.GetComponent<Player>().hideAirContainer) {
                    airBubbleCointainer.SetActive(false);
                }
                else {
                    airBubbleCointainer.SetActive(true);
                }


                //CHECK ACTIVE SELFS
                if(invPanel.activeSelf) {
                    World.Instance.isEnviromentPaused = true;
                }
                else {
                    World.Instance.isEnviromentPaused = false;
                }

            }
            /// <summary>
            /// Adds the canvas' components as well as adds every UI item 
            /// </summary>
            public void SetCanvas() {
                //Init Stuff
                transform.name = "Canvas";
                transform.tag = "UI";

                //Add Components
                Canvas canvas = gameObject.AddComponent<Canvas>();
                CanvasScaler canvasScaler = gameObject.AddComponent<CanvasScaler>();
                GraphicRaycaster graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

                //Modify Components
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.pixelPerfect = true;
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1024.0f, 768.0f); //Resoulution for most landscape games
                canvasScaler.matchWidthOrHeight = 0.5f;
                gameObject.SetActive(false);

                CreateStartPanel();
                CreateDamagePanel();
                CreateDayText();
                UsingItemPanel();
                CreateBlockNameObjs();
            }

            void CreateStartPanel() {
                //Init Stuff
                startPanel = new GameObject();
                startPanel.transform.name = "Start Panel";
                startPanel.transform.tag = "UI";
                startPanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = startPanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = startPanel.AddComponent<CanvasRenderer>();
                Image image = startPanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = Color.clear;

                CreateCrosshairs();
                CreateHUDPanel();
            }

            void CreateCrosshairs() {
                //Init Stuff
                GameObject Crosshair = new GameObject();
                Crosshair.transform.name = "Crosshairs";
                Crosshair.transform.tag = "UI";
                Crosshair.transform.SetParent(startPanel.transform);

                //Add Components
                RectTransform rectTransform = Crosshair.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = Crosshair.AddComponent<CanvasRenderer>();
                Image image = Crosshair.AddComponent<Image>();

                //Modify
                rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                rectTransform.sizeDelta = new Vector2(15.0f, 15.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Crosshairs");
            }

            void CreateHUDPanel() {
                //Init Stuff
                GameObject hudPanel = new GameObject();
                hudPanel.transform.name = "HUD Panel";
                hudPanel.transform.tag = "UI";
                hudPanel.transform.SetParent(startPanel.transform);

                //Add Components
                RectTransform rectTransform =   hudPanel.AddComponent<RectTransform>();

                //Modify
                rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 0.18229166666f);

                CreateHotbarandHUDContainers(hudPanel);
            }

            void CreateHotbarandHUDContainers(GameObject hudPanel) {
                //1 = hotbar, 2 is Exp bar container, 3 is expbar fill container, 4 is text, 5 is health container, 6 is health outline container,
                //7 is hunger bar container, 8 is hunger bar outline container, 9 is thirst baar container, 10 is thirstbar outline container
                //11 is air container, 12 is slot container, 13 is slot template

                GameObject[] hotbarAndContainers = new GameObject[14];

                Image image = null;
                TextMeshProUGUI TMP = new TextMeshProUGUI();
                GridLayoutGroup gridLayoutGroup = null;
                
                string[] hotbarAndContainerNames = {"Hotbar",     "Air Container", "Exp Container","Health Container","Hunger Container","Thirst Container",     "Exp Filler","Empty Hearts","Empty Hunger","Empty Thirst",     "Exp level(TMP)",     "Hotbar Slot Container","Hotbar Slot Template", "Hotbar Slot Draggable"};
                Vector2[] hotbarAndContainersAnchoredPos = {new Vector2(0.0f, 40.0f),     new Vector2(-450.0f, 30f),new Vector2(0.0f, 82.0f), new Vector2(-175.0f, 105.0f),new Vector2(175.0f, 105.0f), new Vector2(0.0f, 128.0f),     new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),     new Vector2(0.0f, 105.0f),     new Vector2(-270.0f, 0.0f),new Vector2(-0.0f,0.0f),     new Vector2(0.0f, 0.0f)};
                Vector2[] hotbarAndContainerAnchorMin = {new Vector2(0.5f, 0.0f),     new Vector2(0.5f, 0.0f),new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.0f),new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.0f),     new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),     new Vector2(0.5f, 0.0f),     new Vector2(0.5f, 0.5f),new Vector2(0.0f, 0.0f),     new Vector2(0.5f, 0.5f)};
                Vector2[] hotbarAndContainerAnchorMax = {new Vector2(0.5f, 0.0f),     new Vector2(0.5f, 0.0f),new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.0f),new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.0f),     new Vector2(1.0f, 1.0f),new Vector2(1.0f, 1.0f),new Vector2(1.0f, 1.0f),new Vector2(1.0f, 1.0f),     new Vector2(0.5f, 0.0f),     new Vector2(0.5f, 0.5f),new Vector2(1.0f, 1.0f),     new Vector2(0.5f, 0.5f)};
                Vector2[] hotbarAndContainerSizeDelta = {new Vector2(590.0f, 50.0f),     new Vector2(195.0f, 15.0f),new Vector2(590.0f, 20.0f), new Vector2(240.0f, 15.0f),new Vector2(240.0f, 15.0f), new Vector2(240.0f, 15.0f),     new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),new Vector2(0.0f, 0.0f),     new Vector2(73.0f, 20.0f),     new Vector2(50.0f, 50.0f),new Vector2(0.0f, 0.0f),     new Vector2(50.0f, 50.0f)};

                for(int i = 0; i < 14; i++) {
                    //Init Stuff
                    hotbarAndContainers[i] = new GameObject();
                    hotbarAndContainers[i].transform.name = hotbarAndContainerNames[i];
                    hotbarAndContainers[i].transform.tag = "UI";
                    switch(i) {
                        case 0 or 1 or 2 or 3 or 4 or 5 or 10:
                            hotbarAndContainers[i].transform.SetParent(hudPanel.transform);
                            break;
                        case 6 or 7 or 8 or 9:
                            hotbarAndContainers[i].transform.SetParent(hotbarAndContainers[i - 4].transform);
                            break;
                        case 11:
                            hotbarAndContainers[i].transform.SetParent(hotbarAndContainers[0].transform);
                            break;
                        case 12:
                            hotbarAndContainers[i].transform.SetParent(hotbarAndContainers[11].transform);
                            break;
                        case 13:
                            hotbarAndContainers[i].transform.SetParent(hotbarAndContainers[12].transform);
                            break;
                    }


                    //Add Components
                    RectTransform rectTransform = hotbarAndContainers[i].AddComponent<RectTransform>();
                    switch(i) {
                        case 0:
                            image = hotbarAndContainers[i].AddComponent<Image>();
                            WholeInv hotbarInv = hotbarAndContainers[i].AddComponent<WholeInv>();
                            break;
                        case 10:
                            CanvasRenderer canvasRenderer = hotbarAndContainers[i].AddComponent<CanvasRenderer>(); 
                            TMP = hotbarAndContainers[i].AddComponent<TextMeshProUGUI>();
                            break;
                        case 12: //Item Template
                            gridLayoutGroup = hotbarAndContainers[i].AddComponent<GridLayoutGroup>();
                            InvSlot invSlot = hotbarAndContainers[i].AddComponent<InvSlot>();
                            break;
                        case 13:
                            CanvasRenderer canvasRenderer1 = hotbarAndContainers[i].AddComponent<CanvasRenderer>();
                            image = hotbarAndContainers[i].AddComponent<Image>();
                            DraggableItems draggableItems = hotbarAndContainers[i].AddComponent<DraggableItems>();
                            break;
                    }

                    //Modify
                    rectTransform.anchoredPosition = hotbarAndContainersAnchoredPos[i];
                    rectTransform.anchorMin = hotbarAndContainerAnchorMin[i];
                    rectTransform.anchorMax = hotbarAndContainerAnchorMax[i];
                    rectTransform.sizeDelta = hotbarAndContainerSizeDelta[i];
                    switch(i) {
                        case 0:
                            if (image != null) {
                                image.sprite = Resources.Load<Sprite>(Sources.TEX_HOTBAR_UI);
                                image.preserveAspect = true;
                            }
                            break;
                        case 10:
                            TMP.text = "100000";
                            TMP.fontSize = 21.0f;
                            TMP.color = Color.green;
                            TMP.alignment = TextAlignmentOptions.Center;
                            break;
                        case 12:
                            gridLayoutGroup.cellSize = new Vector2(50.0f, 50);
                            gridLayoutGroup.childAlignment =  TextAnchor.MiddleCenter;
                            hotbarAndContainers[i].SetActive(false);
                            break;
                        case 13:
                            image.color = Color.clear;
                            break;
                    }
                }
                hotBar = hotbarAndContainers[0];
                airBubbleCointainer = hotbarAndContainers[1];
                airBubbleCointainer.SetActive(false);
                AddHUDImagesAndChildText(hotbarAndContainers, hudPanel);
            }

            void AddHUDImagesAndChildText(GameObject[] hotbarAndContainers, GameObject hudPanel) {
                GameObject[] HHT = new GameObject[7]; //2, 3, 4, 5 for parents

                string[] Names = {"Exp Bar", "Health Bar", "Hunger Bar", "Thirst Bar",    "Image Template", "Item Amount(Txt)",     "Select Item Outline"};
                string[] Images = {"Visuals/Textures/UI/Start Panel/Exp Bar/Exp Bar","Visuals/Textures/UI/Start Panel/Health Bar/Health Bar",
                "Visuals/Textures/UI/Start Panel/Hunger Bar/Hunger Bar","Visuals/Textures/UI/Start Panel/Thirst Bar/Thirst Bar",     Sources.TEX_BUTTON_REG_UI};

                for(int i = 0; i < 7; i++) {
                    //Init Stuff
                    HHT[i] = new GameObject();
                    HHT[i].transform.name = Names[i];
                    HHT[i].transform.tag = "UI";
                    switch(i) {
                        case 0 or 1 or 2 or 3:
                            HHT[i].transform.SetParent(hotbarAndContainers[i + 2].transform);
                            break;
                        case 4 or 5:
                            HHT[i].transform.SetParent(hotbarAndContainers[13].transform);
                            break;
                        case 6:
                            HHT[i].transform.SetParent(hotbarAndContainers[0].transform);
                            break;
                    }

                    //Add Components
                    RectTransform rectTransform = HHT[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = HHT[i].AddComponent<CanvasRenderer>();
                    Image image = null;
                    TextMeshProUGUI TMP = null;
                    switch(i) {
                        case 0 or 1 or 2 or 3:
                            image = HHT[i].AddComponent<Image>();
                            break;
                        case 4:
                            image = HHT[i].AddComponent<Image>();
                            break;
                        case 5:
                            TMP = HHT[i].AddComponent<TextMeshProUGUI>(); 
                            break;
                        case 6:
                            image = HHT[i].AddComponent<Image>();
                            break;
                    }

                    //Modify Components
                    switch(i) {
                        case 0 or 1 or 2 or 3:
                            rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                            rectTransform.sizeDelta = new Vector2(0.0f,0.0f);
                            rectTransform.anchorMin = new Vector2(0.0f,0.0f);
                            rectTransform.anchorMax = new Vector2(1.0f,1.0f);

                            image.sprite = Resources.Load<Sprite>(Images[i]);
                            image.useSpriteMesh = true;

                            break;
                        case 4:
                            rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                            rectTransform.sizeDelta = new Vector2(30.0f,30.0f);
                            rectTransform.anchorMin = new Vector2(0.5f,0.5f);
                            rectTransform.anchorMax = new Vector2(0.5f,0.5f);
                            
                            image.sprite = Resources.Load<Sprite>(Images[i]);
                            image.raycastTarget = false;
                            image.useSpriteMesh = true;

                            break;
                        case 5:
                            rectTransform.anchoredPosition = new Vector2(0.0f, 9.0f);
                            rectTransform.sizeDelta = new Vector2(14.0f,7.0f);
                            rectTransform.anchorMin = new Vector2(0.5f,0.0f);
                            rectTransform.anchorMax = new Vector2(0.5f,0.0f);

                            TMP.raycastTarget = false;
                            TMP.fontSize = 10;
                            TMP.color = Color.green;
                            TMP.alignment = TextAlignmentOptions.Center;
                            break;
                        case 6:
                            rectTransform.anchoredPosition = new Vector2(-270.0f, 0.0f);
                            rectTransform.sizeDelta = new Vector2(50.0f, 50.0f);
                            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                            image.sprite = Resources.Load<Sprite>(Sources.TEX_OUTLINE_UI);
                            break;
                    }
                    //image.preserveAspect = true;
                }
                imageTemplate = HHT[4];
                Outline = HHT[6];




                //AIR BUBBLES
                airBubbles = new GameObject[10];
                Vector2[] airBubblesPos = {new Vector2(-90.0f, 0.0f),new Vector2(-70.0f, 0.0f),new Vector2(-50.0f, 0.0f),new Vector2(-30.0f, 0.0f),
                new Vector2(-10.0f, 0.0f),new Vector2(10.0f, 0.0f),new Vector2(30.0f, 0.0f),new Vector2(50.0f, 0.0f),new Vector2(70.0f, 0.0f),
                new Vector2(90.0f, 0.0f)};
                 

                for(int i = 0; i < 10; i++) {
                    int nameExtension = i + 1;
                    
                    //Init Stuff
                    airBubbles[i] = new GameObject();
                    airBubbles[i].transform.name = "Air " + nameExtension;
                    airBubbles[i].transform.tag = "UI";
                    airBubbles[i].transform.SetParent(hotbarAndContainers[1].transform);

                    //Add Components
                    RectTransform rectTransform = airBubbles[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = airBubbles[i].AddComponent<CanvasRenderer>();
                    Image image = airBubbles[i].AddComponent<Image>();

                    //Modify Components
                    rectTransform.anchoredPosition = airBubblesPos[i];
                    rectTransform.sizeDelta = new Vector2(15.0f, 15.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Air Bar/Air");
                    image.preserveAspect = true;

                }

                //EMPTY HEARTS
                emptyHearts = new GameObject[20];

                Vector2[] emptyHeartsPos = {new Vector2(-112.5f,0.0f), new Vector2(-87.5f,0.0f),new Vector2(-62.5f,0.0f),new Vector2(-37.5f,0.0f),
                new Vector2(-12.5f,0.0f),new Vector2(12.5f,0.0f),new Vector2(37.5f,0.0f),new Vector2(62.5f,0.0f),new Vector2(87.5f,0.0f),
                new Vector2(112.5f,0.0f)};

                int heartCounter= -1;

                for(int i = 0; i < 20; i++) {
                    int nameExtension = i + 1;
                    //Init Stuff
                    emptyHearts[i] = new GameObject();
                    emptyHearts[i].transform.name = "Empty Heart " + nameExtension;
                    emptyHearts[i].transform.tag = "UI";
                    emptyHearts[i].transform.SetParent(hotbarAndContainers[7].transform);

                    //Add Components
                    RectTransform rectTransform = emptyHearts[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = emptyHearts[i].AddComponent<CanvasRenderer>();
                    Image image = emptyHearts[i].AddComponent<Image>();

                    //Modify Components
                    if(i == 0 || i % 2 == 0){
                        heartCounter++;
                    }

                    rectTransform.anchoredPosition = emptyHeartsPos[heartCounter];
                    rectTransform.sizeDelta = new Vector2(15.0f, 15.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                 
                    if(i == 0 || i % 2 == 0) {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Health Bar/Empty/Bottom");
                    }
                    else {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Health Bar/Empty/Top");
                        image.useSpriteMesh = true;
                        image.preserveAspect = true;
                    }
                    
                    emptyHearts[i].SetActive(false);
                }

                hotbarAndContainers[7].transform.SetAsLastSibling();





                //Thirst Fillers
                GameObject[] thirstFillers = new GameObject[20];

                Vector2[] thirstFillerPos = {new Vector2(-112.5f,0.0f), new Vector2(-87.5f,0.0f),new Vector2(-62.5f,0.0f),new Vector2(-37.5f,0.0f),
                new Vector2(-12.5f,0.0f),new Vector2(12.5f,0.0f),new Vector2(37.5f,0.0f),new Vector2(62.5f,0.0f),new Vector2(87.5f,0.0f),
                new Vector2(112.5f,0.0f)};

                int thirstCounter= -1;

                for(int i = 0; i < 20; i++) {
                    int nameExtension = i + 1;
                    //Init Stuff
                    thirstFillers[i] = new GameObject();
                    thirstFillers[i].transform.name = "Thirst Filler " + nameExtension;
                    thirstFillers[i].transform.tag = "UI";
                    thirstFillers[i].transform.SetParent(hotbarAndContainers[9].transform);

                    //Add Components
                    RectTransform rectTransform = thirstFillers[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = thirstFillers[i].AddComponent<CanvasRenderer>();
                    Image image = thirstFillers[i].AddComponent<Image>();

                    //Modify Components
                    if(i == 0 || i % 2 == 0){
                        thirstCounter++;
                    }

                    rectTransform.anchoredPosition = thirstFillerPos[thirstCounter];
                    rectTransform.sizeDelta = new Vector2(15.0f, 15.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                    if(i == 0 || i % 2 == 0) {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Thirst Bar/Fill/Top");
                    }
                    else {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Thirst Bar/Fill/Bottom");
                    }

                    image.useSpriteMesh = true;
                    image.preserveAspect = true;
                }


                //Thirst Fillers
                GameObject[] emptyHunger= new GameObject[20];

                Vector2[] emptyHungerPos = {new Vector2(-112.5f,0.0f), new Vector2(-87.5f,0.0f),new Vector2(-62.5f,0.0f),new Vector2(-37.5f,0.0f),
                new Vector2(-12.5f,0.0f),new Vector2(12.5f,0.0f),new Vector2(37.5f,0.0f),new Vector2(62.5f,0.0f),new Vector2(87.5f,0.0f),
                new Vector2(112.5f,0.0f)};

                int hungerCounter= -1;

                for(int i = 0; i < 20; i++) {
                    int nameExtension = i + 1;
                    //Init Stuff
                    emptyHunger[i] = new GameObject();
                    emptyHunger[i].transform.name = "Empty " + nameExtension;
                    emptyHunger[i].transform.tag = "UI";
                    emptyHunger[i].transform.SetParent(hotbarAndContainers[8].transform);

                    //Add Components
                    RectTransform rectTransform = emptyHunger[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = emptyHunger[i].AddComponent<CanvasRenderer>();
                    Image image = emptyHunger[i].AddComponent<Image>();

                    //Modify Components
                    if(i == 0 || i % 2 == 0){
                        hungerCounter++;
                    }

                    rectTransform.anchoredPosition = emptyHungerPos[hungerCounter];
                    rectTransform.sizeDelta = new Vector2(15.0f, 15.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                    if(i == 0 || i % 2 == 0) {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Hunger Bar/Empty/Top");
                    }
                    else {
                        image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Start Panel/Hunger Bar/Empty/Bottom");
                    }

                    image.useSpriteMesh = true;
                    image.preserveAspect = true;
                    emptyHunger[i].SetActive(false);
                }


                hotbarAndContainers[8].transform.SetAsLastSibling();
            }

            void CreateDamagePanel() {
                //Init Stuff
                damagePanel = new GameObject();
                damagePanel.transform.name = "Damage Panel";
                damagePanel.transform.tag = "UI";
                damagePanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = damagePanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = damagePanel.AddComponent<CanvasRenderer>();
                Image image = damagePanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = new Color(1.0f, 0.0f, 0.0f, 0.2f);

                damagePanel.SetActive(false);

                CreateDamageChildren();
            }

            void CreateDamageChildren() {
                damageTris = new GameObject[4];

                Vector2[] damageTrisAnchoredPos = {new Vector2(24.0f, -24.0f), new Vector2(-24.0f, -24.0f), new Vector2(24.0f,24.0f), new Vector2(-24.0f, 24.0f)};
                string[] damageTrisImage = {"/Top_Left_Damage", "/Top_Right_Damage", "/Bottom_Left_Damage", "/Bottom_Right_Damage"};
                Vector2[] damageTrisAnchors = {new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f)};

                for(int i = 0; i < 4; i++) {
                    int nameExtension = i +  1;
                    //Init Stuff
                    damageTris[i] = new GameObject();
                    damageTris[i].transform.name = "Damage Tri " + nameExtension;
                    damageTris[i].transform.tag = "UI";
                    damageTris[i].transform.SetParent(damagePanel.transform);

                    //Add Components
                    RectTransform rectTransform = damageTris[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = damageTris[i].AddComponent<CanvasRenderer>();
                    Image image = damageTris[i].AddComponent<Image>();
                
                    //Modify
                    rectTransform.anchoredPosition = damageTrisAnchoredPos[i];
                    rectTransform.sizeDelta = new Vector2(48.0f, 48.0f);
                    rectTransform.anchorMin = damageTrisAnchors[i];
                    rectTransform.anchorMax = damageTrisAnchors[i];
                    image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Damage Screen" + damageTrisImage[i]);
                }
            }
 
            void CreateBlockNameObjs() {
                blockNames = new GameObject[2];
                Image image = null;
                TextMeshProUGUI TMP = null;

                string[] Names = {"Block Name Panel", "Block Nam Text"};

                for(int i = 0; i < 2; i++) {
                    blockNames[i] = new GameObject();
                    blockNames[i].transform.name = Names[i];
                    blockNames[i].transform.tag = "UI";
                    switch(i) {
                        case 0:
                            blockNames[i].transform.SetParent(gameObject.transform);
                            break;
                        case 1:
                            blockNames[i].transform.SetParent(blockNames[0].transform);
                            break;
                    }

                    //AddComponents
                    RectTransform rectTransform = blockNames[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = blockNames[i].AddComponent<CanvasRenderer>();
                    switch(i) {
                        case 0:
                            image = blockNames[i].AddComponent<Image>();
                            break;
                        case 1:
                            TMP = blockNames[i].AddComponent<TextMeshProUGUI>();
                            break;
                    }

                    //Modify Components
                    switch(i) {
                        case 0:
                            rectTransform.anchoredPosition = new Vector2(-270.0f, 77.0f);
                            rectTransform.sizeDelta = new Vector2(50.0f, 15.0f);
                            rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
                            rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
                            image.sprite = Resources.Load<Sprite>(Sources.TEX_BLOCK_NAME_UI);

                            break;
                        case 1:
                            rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                            rectTransform.sizeDelta = new Vector2(50.0f, 15.0f);
                            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                            TMP.fontSize = 10;
                            TMP.text = "Bedrock";
                            TMP.color = Color.green;
                            TMP.alignment = TextAlignmentOptions.Center;
                            break;
                    }
                }
            }













            void CreateInventory() {
                //Init Stuff
                invPanel = new GameObject();
                invPanel.transform.name = "Inventory Panel";
                invPanel.transform.tag = "UI";
                invPanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = invPanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = invPanel.AddComponent<CanvasRenderer>();
                Image image = invPanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);

                CreateInvChildren();
            }

            void CreateInvChildren() {
                GameObject[] iScreen = new GameObject[13];
                Image image = null;

                Vector2[] anchoredPos = {new Vector2(0.0f, -10.0f), new Vector2(0.0f, 35.0f), new Vector2(0.0f, 105.0f), new Vector2(0.0f, 175.0f), new Vector2(0.0f, 255.0f), new Vector2(0.0f, 0.0f),     new Vector2(0.0f, 0.0f),     new Vector2(-140.0f, -30.0f),new Vector2(-140.0f, 30.0f),new Vector2(-80.0f, -30.0f),new Vector2(-80.0f, 30.0f),     new Vector2(72.0f,0.0f),       new Vector2(214.0f, 0.0f)};
                string[] Names = {"Inventory Screen", "Row 1", "Row 2", "Row 3", "Row 4", "Item Container",     "Invntory Crafting",     "Bottom Left", "Top Left", "Bottom Right", "Top Right",      "Progress Bar",     "Output Slot"};

                for(int i = 0; i < 13; i++) {
                    iScreen[i] = new GameObject();
                    iScreen[i].transform.name = Names[i];
                    iScreen[i].transform.tag = "UI";
                    switch(i) {
                        case 0:
                            iScreen[i].transform.SetParent(invPanel.transform);
                            break;
                        case 1 or 2 or 3 or 4 or 5 or 6:
                            iScreen[i].transform.SetParent(iScreen[0].transform);
                            break;
                        case 7 or 8 or 9 or 10 or 11 or 12:
                            iScreen[i].transform.SetParent(iScreen[6].transform);
                            break;
                    }

                    //Add Components
                    RectTransform rectTransform = iScreen[i].AddComponent<RectTransform>();
                    if(i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11 || i == 12) {
                        CanvasRenderer canvasRenderer = iScreen[i].AddComponent<CanvasRenderer>();
                        image = iScreen[i].AddComponent<Image>();
                    }

                    //Modify Components
                    rectTransform.anchoredPosition = anchoredPos[i];
                    if(i == 0) {
                        rectTransform.sizeDelta = new Vector2(690.0f, 500.0f);
                        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    } else if(i == 1 || i == 2 || i == 3 || i == 4) {
                        rectTransform.sizeDelta = new Vector2(590.0f, 50.0f);
                        rectTransform.anchorMin = new Vector2(0.5f, 0.0f); //0.592
                        rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
                    }
                    else if(i == 6) {
                        rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                        rectTransform.anchorMin = new Vector2(0.0f, 0.568f); //0.592
                        rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    }
                    else if(i == 7 || i == 8 || i == 9 || i == 10 || i == 12) {
                        rectTransform.sizeDelta = new Vector2(50.0f, 50.0f);
                    }
                    else if(i == 11) {
                        rectTransform.sizeDelta = new Vector2(194.0f, 35.0f);
                    }


                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    if(i == 1 || i == 2 || i == 3 || i == 4) {
                        image.sprite = Resources.Load<Sprite>(Sources.TEX_HOTBAR_UI);
                    } else if(i == 7 || i == 8 || i == 9 || i == 10 || i == 12) {
                        image.sprite = Resources.Load<Sprite>(Sources.TEX_HOTBAR_SLOT_UI);
                    } else if(i == 11) {
                        image.sprite = Resources.Load<Sprite>(Sources.TEX_PROGRESS_BAR_UI);
                    }

                    switch(i) {
                        case 0:
                            image.color = new Color(0.0f, 0.0f, 0.0f, 0.4f);
                            break;
                    }
                }

                World.Instance.playerScript.wholeInv.OrganizeInv();          
            }

            void UsingItemPanel() {
                GameObject[] UIP = new GameObject[2];
                TextMeshProUGUI TMP = null;

                string[] Names = {"Using Item Panel",     "MR Txt"};

                for(int i = 0; i < 2; i++) {
                    UIP[i] = new GameObject();
                    UIP[i].transform.name = Names[i];
                    UIP[i].transform.tag = "UI";
                    switch(i) {
                        case 0:
                            UIP[i].transform.SetParent(gameObject.transform);
                            break;
                        case 1:
                            UIP[i].transform.SetParent(UIP[0].transform);
                            break;
                    }

                    RectTransform rectTransform = UIP[i].AddComponent<RectTransform>();
                    if(i == 1) {
                        CanvasRenderer canvasRenderer = UIP[i].AddComponent<CanvasRenderer>();
                        TMP = UIP[i].AddComponent<TextMeshProUGUI>(); 
                    }

                    if(i == 0) {
                        rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                        rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                        rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                        rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                    }
                    else {
                        rectTransform.sizeDelta = new Vector2(290.0f, 22.0f);
                        rectTransform.anchoredPosition = new Vector2(0.0f, 156.0f);
                        rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
                        rectTransform.anchorMax = new Vector2(0.5f, 0.0f);

                        TMP.fontSize = 20;
                        TMP.text = "MODE: Mutate Info";
                        TMP.alignment = TextAlignmentOptions.Center;
                        TMP.color = Color.cyan;

                    }    
                }

                mrTxt = UIP[1];
            }

            public void ChangeMRText() {
                mrTxt.GetComponent<TextMeshProUGUI>().text = mrtxtTXT[mrTextIndex];
            }
            void CreateDebugPanel() {
                //Init Stuff
                debugPanel = new GameObject();
                debugPanel.transform.name = "Debug Panel";
                debugPanel.transform.tag = "UI";
                debugPanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = debugPanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = debugPanel.AddComponent<CanvasRenderer>();
                Image image = debugPanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = Color.clear;
            }

            void CreateDayText() {
                dayText = Instantiate(Resources.Load<GameObject>(Sources.PREFAB_DAY_TEXT));
            }
            void CreatePausePanel() {
                //Init Stuff
                pausePanel = new GameObject();
                pausePanel.transform.name = "Pause Panel";
                pausePanel.transform.tag = "UI";
                pausePanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = pausePanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = pausePanel.AddComponent<CanvasRenderer>();
                Image image = pausePanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = new Color(0.0f, 0.0f, 0.0f, 0.8f);

                SetPauseTitle();
            }

            void SetPauseTitle() {
                //Init Stuff
                GameObject pauseTitle = new GameObject();
                pauseTitle.transform.name = "Title";
                pauseTitle.transform.tag = "UI";
                pauseTitle.transform.SetParent(pausePanel.transform);

                //Add Components
                RectTransform rectTransform = pauseTitle.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = pauseTitle.AddComponent<CanvasRenderer>();
                TextMeshProUGUI TMP = pauseTitle.AddComponent<TextMeshProUGUI>();

                //Modify
                rectTransform.anchoredPosition = new Vector2(0.0f, -75.0f);
                rectTransform.sizeDelta = new Vector2(330.0f, 48.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                TMP.text = "Game Paused";
                TMP.fontSize = 50f;

                CreatePausePanelButtons();
            }

            void CreatePausePanelButtons() {
                GameObject[] Buttons = new GameObject[2];


                UnityEngine.Events.UnityAction[] Listeners = {ResumeGameFromPause};
                Color[] btnColor = {Color.white, Color.red};
                string[] btnNames = {"Resume Game(Btn)", "Main Menu Game(Btn)"};
                Vector2[] btnPos = {new Vector2(0.0f, 60.0f), new Vector2(0.0f, -20.0f)};

                for(int i = 0; i < 2; i++) {
                    //Init Stuff
                    Buttons[i] = new GameObject();
                    Buttons[i].transform.name = btnNames[i];
                    Buttons[i].transform.tag = "UI";
                    Buttons[i].transform.SetParent(pausePanel.transform);

                    //Add Components
                    RectTransform rectTransform = Buttons[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = Buttons[i].AddComponent<CanvasRenderer>();
                    Image image = Buttons[i].AddComponent<Image>();
                    Button button = Buttons[i].AddComponent<Button>();

                    //Modify
                    rectTransform.anchoredPosition = btnPos[i];
                    rectTransform.sizeDelta = new Vector2(200.0f, 25.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Buttons/Button_Regular");
                    image.color = btnColor[i];
                    button.onClick.AddListener(Listeners[0]);
                }

                AddPausePanelBtnText(Buttons);
            }

            void AddPausePanelBtnText(GameObject[] Btns) {
                GameObject[] Texts = new GameObject[2];

                string[] txtNames = {"Resume Game(Txt)", "Back to Main Menu(txt)"};
                string[] txtTxt = {"Resume Game", "Back to Main Menu"};

                for(int i = 0; i < 2; i++) {
                    //Init Stuff
                    Texts[i] = new GameObject();
                    Texts[i].transform.name = txtNames[i];
                    Texts[i].transform.tag = "UI";
                    Texts[i].transform.SetParent(Btns[i].transform);

                    //Add components
                    RectTransform rectTransform = Texts[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = Texts[i].AddComponent<CanvasRenderer>();
                    TextMeshProUGUI TMP = Texts[i].AddComponent<TextMeshProUGUI>();
                    
                    //Modify
                    rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                    rectTransform.sizeDelta = new Vector2(155.0f, 17.0f);
                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    TMP.text = txtTxt[i];
                    TMP.color = Color.black;
                    TMP.fontSize = 18f;
                    TMP.alignment = TextAlignmentOptions.Center;
                }
            }

            void CreateGameOverPanel() {
                //Init Stuff
                gameOverPanel = new GameObject();
                gameOverPanel.transform.name = "Game Over Panel";
                gameOverPanel.transform.tag = "UI";
                gameOverPanel.transform.SetParent(gameObject.transform);

                //Add Components
                RectTransform rectTransform = gameOverPanel.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = gameOverPanel.AddComponent<CanvasRenderer>();
                Image image = gameOverPanel.AddComponent<Image>();

                //Modify Components
                rectTransform.anchoredPosition = new Vector2(0.0f,0.0f);
                rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                image.color = Color.black;

                CreateGameOverText();
                CreateGameOverButtons();
            }


            void CreateGameOverText() {
                //Init Stuff
                GameObject gameoverTitle = new GameObject();
                gameoverTitle.transform.name = "Title";
                gameoverTitle.transform.tag = "UI";
                gameoverTitle.transform.SetParent(gameOverPanel.transform);

                //Add Components
                RectTransform rectTransform = gameoverTitle.AddComponent<RectTransform>();
                CanvasRenderer canvasRenderer = gameoverTitle.AddComponent<CanvasRenderer>();
                TextMeshProUGUI TMP = gameoverTitle.AddComponent<TextMeshProUGUI>();

                //Modify
                rectTransform.anchoredPosition = new Vector2(0.0f, -75.0f);
                rectTransform.sizeDelta = new Vector2(330.0f, 48.0f);
                rectTransform.anchorMin = new Vector2(0.5f, 1.0f);
                rectTransform.anchorMax = new Vector2(0.5f, 1.0f);
                rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                TMP.text = "Game Over";
                TMP.fontSize = 50f;
            }

            void CreateGameOverButtons() {
                GameObject[] Buttons = new GameObject[2];


                UnityEngine.Events.UnityAction[] Listeners = {ResumeGameFromOver};
                Color[] btnColor = {Color.white, Color.red};
                string[] btnNames = {"Respawn(Btn)", "Main Menu Game(Btn)"};
                Vector2[] btnPos = {new Vector2(0.0f, 60.0f), new Vector2(0.0f, -20.0f)};

                for(int i = 0; i < 2; i++) {
                    //Init Stuff
                    Buttons[i] = new GameObject();
                    Buttons[i].transform.name = btnNames[i];
                    Buttons[i].transform.tag = "UI";
                    Buttons[i].transform.SetParent(gameOverPanel.transform);

                    //Add Components
                    RectTransform rectTransform = Buttons[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = Buttons[i].AddComponent<CanvasRenderer>();
                    Image image = Buttons[i].AddComponent<Image>();
                    Button button = Buttons[i].AddComponent<Button>();

                    //Modify
                    rectTransform.anchoredPosition = btnPos[i];
                    rectTransform.sizeDelta = new Vector2(200.0f, 25.0f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    image.sprite = Resources.Load<Sprite>("Visuals/Textures/UI/Buttons/Button_Regular");
                    image.color = btnColor[i];
                    button.onClick.AddListener(Listeners[0]);
                }
            
                AddGameOverBtnText(Buttons);
            }

            void AddGameOverBtnText(GameObject[] Btns) {
                GameObject[] Texts = new GameObject[2];

                string[] txtNames = {"Respawn(Txt)", "Back to Main Menu(txt)"};
                string[] txtTxt = {"Respawn", "Back to Main Menu"};

                for(int i = 0; i < 2; i++) {
                    //Init Stuff
                    Texts[i] = new GameObject();
                    Texts[i].transform.name = txtNames[i];
                    Texts[i].transform.tag = "UI";
                    Texts[i].transform.SetParent(Btns[i].transform);

                    //Add components
                    RectTransform rectTransform = Texts[i].AddComponent<RectTransform>();
                    CanvasRenderer canvasRenderer = Texts[i].AddComponent<CanvasRenderer>();
                    TextMeshProUGUI TMP = Texts[i].AddComponent<TextMeshProUGUI>();
                    
                    //Modify
                    rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
                    rectTransform.sizeDelta = new Vector2(155.0f, 17.0f);
                    rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    TMP.text = txtTxt[i];
                    TMP.color = Color.black;
                    TMP.fontSize = 18f;
                    TMP.alignment = TextAlignmentOptions.Center;
                }
            }

            //Button Listeners
            void ResumeGameFromPause() {
                World.Instance.isGamePaused = false;
            }
            void ResumeGameFromOver() {
                World.Instance.playerScript.Respawn();

                for(int i = 0; i < 20; i++) {
                    World.Instance.uiScript.emptyHearts[i].SetActive(false);
                }
                World.Instance.isGameOver = false;
            }
        }
    }
}
