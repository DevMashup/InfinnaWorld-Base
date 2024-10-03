using UnityEngine;

namespace DevMashup {
    namespace GamePlay {
        public class AlligatorBrain : MonoBehaviour {
            
            [HideInInspector]public int Health = 10;
            
            
            CharacterController cc;
            AudioSource audioSource;
            Animator anim;



            Transform player {get { return World.Instance.playerObj.GetComponent<Transform>(); } } //Gets the player's transform
            Vector2 Destination {get {return new Vector2(player.position.x, player.position.z); } } //Gets the player's position
            Vector2 Pos {get {return new Vector2(transform.position.x, transform.position.z); } } //Gets this object's position

            bool isHitting = false; //Responsible for seeing if the zombie is within arms range
            float secondCounter = 0; //We dontwant the zombie to hit every frame
            int emptyHeartsCounter = 0; //Responsible for decreasing player health
            


            public void CreateAlligator() {
                //Init Stuff
                transform.name = "Alligator";
                transform.tag = "Alligator";
                transform.SetParent(World.Instance.Containers[3].transform);
            
                //Add components
                cc = gameObject.AddComponent<CharacterController>();
                audioSource = gameObject.GetComponent<AudioSource>();
                anim = gameObject.GetComponent<Animator>();
            
                //Modify
                transform.position = new Vector3((World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE, World.Instance.worldConfig.surfaceLevel + 50.0f, (World.Instance.debugTest.testSize / 2) * Constants.CHUNK_SIDE);
                cc.center = new Vector3(0.0f, 0.8f, 0.0f);
            }


            //
            void Update() {
                if(!World.Instance.isGamePaused && !World.Instance.isGameOver) { //The game is not frozen
                    secondCounter += Time.deltaTime;

                    cc.enabled = true;
                    audioSource.enabled = true;
                    anim.enabled = true;


                    transform.LookAt(new Vector3(Destination.x, transform.position.y, Destination.y)); //Look at the victim(I mean player).....MWAHAHAHAHAHAH
                    if(Vector2.Distance(Pos, Destination) > 2f && Vector2.Distance(Pos, Destination) < 17f) {
                        gameObject.GetComponent<AudioSource>().Play();
                    }

                    if(Vector2.Distance(Pos, Destination) > 2f) { //If the alligatoor is more than 2 units away
                    
                        cc.Move(transform.forward * Time.deltaTime * 3f); //1st value is whatever 'Forward'is for the alligator, 2nd is time between frames, 3rd is speed 
                    }
                    else {
                        isHitting = true;
                    }


                    if(secondCounter < 1) { //In between the active states
                        isHitting = false;
                    }

                    if(isHitting) {
                        MobHit();
                    }

                    void MobHit() {
                        gameObject.GetComponent<AudioSource>().Play();
                        World.Instance.uiScript.emptyHearts[19 - emptyHeartsCounter].SetActive(true);
                        World.Instance.uiScript.damagePanel.SetActive(true);

                        if (secondCounter > 1.5f) {
                            //RESET
                            World.Instance.uiScript.damagePanel.SetActive(false);
                            isHitting = false;
                            secondCounter = 0;
                            emptyHeartsCounter++;
                            World.Instance.playerScript.Health -= 1;

                            if (World.Instance.playerScript.Health == 0) {
                                //World.Instance.uiScript.gameOverPnl.SetActive(true);
                                World.Instance.isGameOver = true;
                                emptyHeartsCounter = 0;
                            }
                        }
                    }
                }
                else {
                    cc.enabled = false;
                    audioSource.enabled = false;
                    anim.enabled = false;
                }
            }
        }
    }
}
