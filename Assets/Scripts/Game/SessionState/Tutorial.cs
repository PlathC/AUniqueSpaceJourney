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

        private Transform snapTurnObj = null;
        private Transform teleporting = null;

        public int nbPiecesGatheredWatch = 2;
        private int nbPieces = 3;

        private Hand leftHand = null;
        private GameObject flashLight = null;

        private bool watchTutoCompleted = false;

        private enum TutorialState
        {
            IndicScreen,
            BuildWatch,
            PlayerWatchInstructions,
            WaitingEndTuto,
            Complete
        }

        private TutorialState m_state;

        public Tutorial(Player p, Session s)
        {
            m_player = p;
            m_session = s;

            // State tuto screen
            //m_state = TutorialState.IndicScreen;

            // TESTING
            m_state = TutorialState.BuildWatch;

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
                flashLight = GameObject.FindGameObjectsWithTag("flashlight")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("flashlight TAG not found");
            }

            try
            {
                leftHand = GameObject.FindGameObjectsWithTag("leftHand")[0].GetComponent<Hand>();
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("leftHand TAG not found");
            }

            try
            {
                // Disable cave entry TP
                GameObject.Find("SmallPartGround").GetComponent<TeleportAreaCustom>().SetLocked(true);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Cave entry ground not found");
            }

            try
            {
                // Disable cave tp
                GameObject.Find("BigPartGround").GetComponent<TeleportAreaCustom>().SetLocked(true);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Cave ground TAG not found");
            }

            // initialize tuto (gather pieces instructions...)
            //// Disable snap turn
            //snapTurnObj = GameObject.FindGameObjectsWithTag("Player")[0].gameObject.transform.Find("Snap Turn");
            //snapTurnObj.GetComponent<SnapTurn>().CanRotate = false;

            //// Disable teleport 
            //teleporting = GameObject.FindGameObjectsWithTag("Player")[0].gameObject.transform.Find("Teleporting");
            //teleporting.gameObject.SetActive(false);

            //// Launch coroutine indications screen
            //m_session.StartCoroutine(IndicScreen());
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
            // m_session.StartCoroutine(ShowControlHints()); // Not needed because already done by Teleport script

            // Enable snap turn
            snapTurnObj.GetComponent<SnapTurn>().CanRotate = true;

            // Enable teleportation
            teleporting.gameObject.SetActive(true);

            // Hide tuto screen
            screenTuto.transform.gameObject.SetActive(false);

            // Tuto screen end => step search watch pieces
            m_state = TutorialState.BuildWatch;
        }

        IEnumerator ShowControlHints()
        {           
            // Teleport action
            SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

            // Show text hint to help teleport
            ControllerButtonHints.ShowTextHint(leftHand, teleportAction, "Teleport", true);

            // Wait 10sec
            yield return new WaitForSeconds(10);

            ControllerButtonHints.HideTextHint(leftHand, teleportAction);
        }

        public IEnumerator WatchInstructions()
        {
            string txtTuto;
            float speed = 10f;

            // Show text instructions on player screen
            m_player.PlayerWatch.PlayerScreen.alignment = TextAlignmentOptions.TopLeft;

            txtTuto = "Ma montre est réparée !";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, m_player.PlayerWatch.PlayerScreen, txtTuto));
            yield return new WaitForSeconds(5);

            txtTuto = "Mon système d'éclairage fonctionne également de nouveau !! Je risque d'en avoir besoin pour visiter cette grotte";
            yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, m_player.PlayerWatch.PlayerScreen, txtTuto));
            yield return new WaitForSeconds(5);

            // Unlock flash light
            m_player.FlashLight.Locked = false;

            // Show text hint to help turn on the light
            Hand leftHand = GameObject.Find("LeftHand").GetComponent<Hand>();
            SteamVR_Action_Boolean toggleLight = SteamVR_Input.GetBooleanAction("FlashLightToggle"); // X  button
            ControllerButtonHints.ShowTextHint(leftHand, toggleLight, "Allumer la lampe", true);

            // Wait for light to be toggled
            while (!toggleLight.GetState(inputSource))
            {
                yield return new WaitForSeconds(0.1f);
            };

            // Hide control hint
            ControllerButtonHints.HideTextHint(leftHand, toggleLight);

            watchTutoCompleted = true;
        }

        // Function called by session every tick
        public IState Update()
        {
            switch (m_state)
            {
                case TutorialState.BuildWatch:
                    // Check number of watch pieces gathered by player
                    // If 3/3 => spawn watch on wrist
                    // Return this or next state
                    if (nbPiecesGatheredWatch == nbPieces)
                    {
                        // Lock teleport on exterior cave
                        GameObject.Find("PlayerGround").GetComponent<TeleportAreaCustom>().SetLocked(true);

                        // Lock cave entry teleport
                        GameObject.Find("SmallPartGround").GetComponent<TeleportAreaCustom>().SetLocked(true);

                        // Enable watch and player screen
                        m_player.PlayerWatch.EnableWatch();

                        // Send watch notification
                        m_player.PlayerWatch.SendWatchNotification();

                        // State waiting for instruction reading
                        m_state = TutorialState.PlayerWatchInstructions;
                    } 
                    else if (nbPiecesGatheredWatch == 2)
                    {
                        // Enable cave entry teleport
                        GameObject.Find("SmallPartGround").GetComponent<TeleportAreaCustom>().SetLocked(false);
                    }
                    break;
                case TutorialState.PlayerWatchInstructions:
                    if (m_player.PlayerWatch.LookingWatch)
                    {
                        // Start watch tuto
                        m_session.StartCoroutine(WatchInstructions());
                        
                        // Wait for tuto end
                        m_state = TutorialState.WaitingEndTuto;
                    }
                    break;
                case TutorialState.WaitingEndTuto:
                    if (watchTutoCompleted)
                    {
                        m_state = TutorialState.Complete;
                    }
                    break;
                case TutorialState.Complete:
                    // Go to main story
                    return new MainStory(m_player, m_session);
            }
            return this;
        }
    }
}
