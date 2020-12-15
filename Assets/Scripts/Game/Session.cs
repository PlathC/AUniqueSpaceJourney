using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUSJ
{
    public class Session : MonoBehaviour
    {
        private IState m_currentState;
        private Player m_player;

        public IState CurrentState { get => m_currentState; set => m_currentState = value; }

        // Start is called before the first frame update
        void Start()
        {
            // Find player instance
            m_player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
            CurrentState = new Tutorial(m_player, this);
        }

        // Update is called once per frame
        void Update()
        {
            CurrentState = CurrentState.Update();
        }
    }
}
