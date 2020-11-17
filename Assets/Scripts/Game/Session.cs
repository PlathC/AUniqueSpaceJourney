using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUSJ
{
    public class Session : MonoBehaviour
    {
        private IState m_currentState;
        private Player player;

        // Start is called before the first frame update
        void Start()
        {
            // Find player instance
            player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
            m_currentState = new Tutorial(player);
        }

        // Update is called once per frame
        void Update()
        {
            m_currentState = m_currentState.Update();
        }
    }
}
