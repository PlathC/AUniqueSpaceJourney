using System;
using System.Collections;
using System.Collections.Generic;
using AUSJ;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    public class MainStory : IState
    {
        private Player m_player;
        private Session m_session = null;

        private GameObject hammer = null;
        private GameObject vacuum = null;

        private enum MainStoryState
        {
            EnterCave,
            FindHammer,
            FindWaterTool,
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
        }

        public IState Update()
        {
            switch (m_state)
            {
                case MainStoryState.EnterCave:
                    try
                    {
                        // Enable blocking walls
                        GameObject.Find("BlockingCaveWalls").SetActive(true);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception("BlockingCaveWalls not found");
                    }

                    // Trigger sound
                    GameObject.Find("SoundRockFall").GetComponent<AudioSource>().Play();

                    try
                    {
                        // Trigger rock falling
                        GameObject.Find("FallingRocks").SetActive(true);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception("BlockingCaveWalls not found");
                    }

                    break;
                case MainStoryState.FindHammer:
                    Interactable hammerInteractable = hammer.GetComponent<Interactable>();
                    if (hammerInteractable != null && hammerInteractable.attachedToHand != null)
                    {
                        // Print something on watch screen


                        // Next state
                        m_state = MainStoryState.FindWaterTool;
                    }
                    break;
                case MainStoryState.FindWaterTool:
                    Interactable vacuumInteractable = vacuum.GetComponent<Interactable>();
                    if (vacuumInteractable != null && vacuumInteractable.attachedToHand != null)
                    {
                        // Print something on watch screen


                        // Game complete
                        m_state = MainStoryState.EndGame;
                    }
                    break;
            }
            return this;
        }
    }
}
