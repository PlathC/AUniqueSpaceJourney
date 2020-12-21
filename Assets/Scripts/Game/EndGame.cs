using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private bool endGameTriggered = false;

    public bool EndGameTriggered { get => endGameTriggered; set => endGameTriggered = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerCollider")
        {
            EndGameTriggered = true;
        }
    }
}
