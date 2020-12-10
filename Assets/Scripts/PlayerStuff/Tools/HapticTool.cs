using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(Interactable))]
    public class HapticTool : MonoBehaviour
    {
        [SerializeField]
        private bool constantVibration = false;

        [SerializeField]
        private bool onCollisionVibrate = true;

        private Interactable interactable;
        
        private void Start()
        {
            interactable = gameObject.GetComponent<Interactable>();
        }

        // Just pulse vibrate when entering collision
        void OnCollisionEnter(Collision other)
        {
            if (onCollisionVibrate && interactable.attachedToHand)
            {
                // Not custom vibration
                if (other.gameObject.GetComponent<CustomVibrationCollision>() == null)
                {
                    Vibrations.SimplePulse(interactable.attachedToHand);
                }
            }
        }

        // Vibrate constantly when colliding with something
        private void OnCollisionStay(Collision collision)
        {
            if (constantVibration && interactable.attachedToHand)
            {
                // Not custom vibration
                if (collision.gameObject.GetComponent<CustomVibrationCollision>() == null)
                {
                    //interactable.attachedToHand.TriggerHapticPulse(1 / 100000, 160, 0.3f);
                    Vibrations.SimplePulse(interactable.attachedToHand, 0.3f, 100000);
                }
            }
        }
    }
}
