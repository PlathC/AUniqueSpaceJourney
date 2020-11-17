using System.Collections;
using System.Collections.Generic;
using AUSJ;
using UnityEngine;

namespace AUSJ
{
    public class MainStory : IState
    {
        private Player m_player;

        public IState Update()
        {
            return this;
        }
    }
}
