using AUSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class testVibration : MonoBehaviour
{
    float lastAmp = 0.1f;

    private void OnCollisionEnter(Collision collision)
    {
        lastAmp = 0.5f;
        lastAmp = lastAmp % 1f;
        if (collision.gameObject.tag == "hammer" && collision.gameObject.GetComponent<Interactable>().attachedToHand != null)
            Vibrations.SimplePulse(collision.gameObject.transform.GetComponent<Interactable>().attachedToHand, lastAmp);
        //StartCoroutine(Vibrations.PulseAscend(collision.gameObject.transform.GetComponent<Interactable>().attachedToHand, 3, 0.2f, 0.2f));
    }
}
