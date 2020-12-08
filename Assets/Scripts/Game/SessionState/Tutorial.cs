using System.Collections;
using TMPro;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System;

namespace AUSJ
{
    public class Tutorial : IState
    {
        private Session m_session = null;
        private Player m_player = null;

        private SteamVR_Action_Boolean nextAction = null;
        private SteamVR_Input_Sources inputSource;
        
        private TextMeshPro screenTuto = null;
        private GameObject playerScreen = null;

        private Transform snapTurnObj = null;
        private Transform teleporting = null;

        private int nbPiecesGatheredWatch = 0;
        private int nbPieces = 3;

        private GameObject hammer = null;
        private GameObject waterTool = null;
        private GameObject flashLight = null;

        private enum TutorialState
        {
            IndicScreen,
            BuildWatch,
            FindHammer,
            FindWaterTool
        }

        private TutorialState m_state;

        public Tutorial(Player p, Session s)
        {
            m_player = p;
            m_session = s;

            // State tuto screen
            m_state = TutorialState.IndicScreen;

            nextAction = SteamVR_Input.GetBooleanAction("TutoNext"); // Gachette on any controller
            inputSource = SteamVR_Input_Sources.Any; // Any controller

            // Find tuto screen
            try
            {
                screenTuto = GameObject.FindGameObjectsWithTag("tutoScreen")[0].GetComponent<TextMeshPro>();
            } catch (IndexOutOfRangeException)
            {
                throw new Exception("Tuto screen TAG not found");
            }

            try
            {
                hammer = GameObject.FindGameObjectsWithTag("hammer")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Hammer TAG not found");
            }

            try
            {
                waterTool = GameObject.FindGameObjectsWithTag("waterTool")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Water Tool TAG not found");
            }

            // initialize tuto (gather pieces instructions...)
            //Transform flashlight = GameObject.FindGameObjectsWithTag("Player")[0].gameObject.transform.Find("FlashLight"); // chercher dans childs of childs
            flashLight = GameObject.FindGameObjectsWithTag("FlashLight")[0];
            
            // Disable snap turn
            snapTurnObj = GameObject.FindGameObjectsWithTag("Player")[0].gameObject.transform.Find("Snap Turn");
            snapTurnObj.GetComponent<SnapTurn>().CanRotate = false;

            // Disable teleport 
            teleporting = GameObject.FindGameObjectsWithTag("Player")[0].gameObject.transform.Find("Teleporting");
            teleporting.gameObject.SetActive(false);

            // Launch coroutine indications screen
            m_session.StartCoroutine(IndicScreen());
        }

        IEnumerator IndicScreen()
        {
            string txtTuto;
            float speed = 10f;

            //yield on a new YieldInstruction that waits for 5 seconds.
            yield return new WaitForSeconds(2);

            //Print the time of when the function is first called.
            // Debug.Log("START INDIC SCREEN TUTO");

            screenTuto.alignment = TextAlignmentOptions.TopLeft;

            txtTuto = "Bienvenue dans l'univers de A Unique Space Journey !";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, screenTuto, txtTuto));
            yield return new WaitForSeconds(5);

            txtTuto = "Vous venez de vous crasher sur la planète Centora. Vous voila seul face à la nature hostile de cette planète...";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, screenTuto, txtTuto));
            yield return new WaitForSeconds(5);

            txtTuto = "Votre mission est simple, survivre...";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, screenTuto, txtTuto));
            yield return new WaitForSeconds(5);

            txtTuto = "Moi : Ma montre à dû se briser lors du crash... Je devrais peut-être essayer de la réparer !";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, screenTuto, txtTuto));
            yield return new WaitForSeconds(3);

            txtTuto = "\n\nPour démarrer la partie, appuyer sur la gachette d'une des manettes";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, screenTuto, txtTuto, false));

            while (!nextAction.GetState(inputSource))
            {
                yield return new WaitForSeconds(0.1f);
            };

            // Show control hints
            m_session.StartCoroutine(ShowControlHints());

            // Enable snap turn
            snapTurnObj.GetComponent<SnapTurn>().CanRotate = true;

            // Enable teleportation
            teleporting.gameObject.SetActive(true);

            // Tuto screen end => step search watch pieces
            m_state = TutorialState.BuildWatch;
        }

        IEnumerator ShowControlHints()
        {
            // Find left hand
            Hand hand = GameObject.Find("LeftHand").GetComponent<Hand>();
            
            // Teleport action
            SteamVR_Action_Boolean toggleAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

            // Show text hint to help teleport
            ControllerButtonHints.ShowTextHint(hand, toggleAction, "Teleport", true);

            // Wait 10sec
            yield return new WaitForSeconds(10);

            ControllerButtonHints.HideAllTextHints(hand);
        }

        public void BuildWatch()
        {
            // Activate watch => Blue color
            m_player.PlayerWatch.GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing blue"));

            // Enable player screen
            m_player.PlayerScreen.transform.gameObject.SetActive(false);
        }

        // Function called by session every tick
        public IState Update()
        {
            switch (m_state)
            {
                case TutorialState.IndicScreen:
                    //Interactable hammerInteractable = hammer.GetComponent<Interactable>();
                    //Hand hand = GameObject.Find("LeftHand").GetComponent<Hand>();
                    //GrabTypes startingGrabType = hand.GetGrabStarting();
                    //if (hammerInteractable.attachedToHand == null && startingGrabType != GrabTypes.None)
                    //{

                    //}
                    //if (hammerInteractable != null && hammerInteractable.attachedToHand != null)
                    //{
                    //    hammer.transform.position
                    //    // Snap and de-snap hammer, and disable it
                    //    //hammer.gameObject.SetActive(false);
                    //    // Enable hammer selection in tools cycle
                    //    // configure hand skelet
                    //}
                    break;
                case TutorialState.BuildWatch:
                    // Check number of watch pieces gathered by player
                    // If 3/3 => spawn watch on wrist
                    // Return this or next state
                    if (nbPiecesGatheredWatch == nbPieces)
                    {
                        BuildWatch();
                        m_state = TutorialState.FindHammer;
                    }
                    break;
                case TutorialState.FindHammer:
                    //Interactable hammerInteractable = hammer.GetComponent<Interactable>();
                    //if (hammerInteractable != null && hammerInteractable.attachedToHand != null)
                    //{
                    //    hammer.SetActive(false);
                    //    // Enable hammer selection in tools cycle
                    //    // configure hand skelet
                    //}
                    break;
            }
            return this;
        }
    }
}
