using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using AUSJ;
using UnityEngine;

namespace AUSJ
{
    public class Tutorial : IState
    {
        private Player m_player;

        private enum TutorialState
        {
            BuildWatch,
            FindFlashLight,
            FindWaterTool
        }

        private TutorialState m_state;

        public Tutorial(Player p)
        {
            this.m_player = p;
            m_state = TutorialState.BuildWatch;
            
            // initialize tuto (gather pieces instructions...)
        }

        public void SpawnWatch()
        {
            Debug.Log("The watch has spawned.");
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
                    break;
                case TutorialState.FindFlashLight:
                    break;
            }
            return this;
        }
    }
}
