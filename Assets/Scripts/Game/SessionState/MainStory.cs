using System;
using System.Collections;
using System.Collections.Generic;
using AUSJ;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    public class MainStory : IState
    {
        private Player m_player = null;
        private Session m_session = null;

        private GameObject hammer = null;
        private GameObject vacuum = null;
        private GameObject triggerEndGame = null;
        private int toolsFound = 0;

        private enum MainStoryState
        {
            EnterCave,
            FindTools,
            ExploreCave,
            EndGame
        }

        private MainStoryState m_state;

        public MainStory(Player p, Session s)
        {
            m_player = p;
            m_session = s;

            m_state = MainStoryState.EnterCave;

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
                vacuum = GameObject.FindGameObjectsWithTag("waterTool")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Water Tool TAG not found");
            }

            try
            {
                triggerEndGame = GameObject.FindGameObjectsWithTag("triggerEndGame")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("triggerEndGame TAG not found");
            }
        }

        public IEnumerator WatchIndicStartMainStory()
        {
            yield return new WaitForSeconds(4);
            // Send watch notification
            m_player.PlayerWatch.SendWatchNotification();

            while (!m_player.PlayerWatch.LookingWatch)
            {
                yield return new WaitForSeconds(0.5f);
                
                string txtTuto;
                float speed = 10f;

                // Show text instructions on player screen
                m_player.PlayerWatch.PlayerScreen.alignment = TextAlignmentOptions.TopLeft;

                txtTuto = "Oh noooon, le mur s'est effondré ! Je vais devoir explorer cette grotte pour survivre...";
                yield return m_session.StartCoroutine(TextUtils.FadeInText(speed, m_player.PlayerWatch.PlayerScreen, txtTuto));
                yield return new WaitForSeconds(3);

                // Show player stats
                m_player.StartUpdatePlayerConditions();
            }
        }

        public IState Update()
        {
            switch (m_state)
            {
                case MainStoryState.EnterCave:
                    try
                    {
                        // Enable blocking walls
                        GameObject.Find("BlockingCaveWalls").transform.GetChild(0).gameObject.SetActive(true);
                    }
                    catch (Exception)
                    {
                        throw new Exception("BlockingCaveWalls not found");
                    }

                    // Trigger sound
                    GameObject.Find("SoundRockFall").GetComponent<AudioSource>().Play();

                    try
                    {
                        // Trigger rock falling
                        GameObject.Find("FallingRocks").transform.GetChild(0).gameObject.SetActive(true);
                    }
                    catch (Exception)
                    {
                        throw new Exception("BlockingCaveWalls not found");
                    }

                    // Unlock interior cave teleport
                    GameObject.Find("BigPartGround").GetComponent<TeleportAreaCustom>().SetLocked(false);

                    // Enable holsters
                    m_player.Holsters.SetActive(true);

                    // Main story instructions after 4 sec
                    m_session.StartCoroutine(WatchIndicStartMainStory());

                    m_state = MainStoryState.FindTools;

                    break;
                case MainStoryState.FindTools:
                    Interactable hammerInteractable = hammer.GetComponent<Interactable>();
                    if (hammerInteractable != null && hammerInteractable.attachedToHand != null)
                    {
                        toolsFound++;
                        // Print something on watch screen
                        // NOT IMPLEMENTED YET
                    }

                    Interactable vacuumInteractable = vacuum.GetComponent<Interactable>();
                    if (vacuumInteractable != null && vacuumInteractable.attachedToHand != null)
                    {
                        toolsFound++;
                        // Print something on watch screen
                        // NOT IMPLEMENTED YET
                    }

                    if (toolsFound == 2)
                    {
                        m_state = MainStoryState.ExploreCave;
                    }
                    break;
                case MainStoryState.ExploreCave:
                    // If player in trigger, start end game
                    if (triggerEndGame.GetComponent<EndGame>().EndGameTriggered)
                    {
                        m_state = MainStoryState.EndGame;
                    }
                    break;
                case MainStoryState.EndGame:
                    // Show player score
                    break;
            }
            return this;
        }
    }
}
