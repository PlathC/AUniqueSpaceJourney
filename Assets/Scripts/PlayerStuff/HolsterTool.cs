using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Interactable))]
    public class HolsterTool : MonoBehaviour, ICanHolster
    {
        public int _SnapPosition;
        public float distanceTrigger = 0.25f;
        public float impactMagnifier = 100f;
     
        int ICanHolster.SnapPosition { get => _SnapPosition; set => _SnapPosition = value; }
        private bool isInHolster = false;
        private Interactable interactable;

        private void Start()
        {
            interactable = GetComponent<Interactable>();
        }

        private void OnDetachedFromHand(Hand hand)
        {
            // Find holsters
            GameObject[] holsters = GameObject.FindGameObjectsWithTag("holster");
            foreach(GameObject holster in holsters)
            {
                // Calculate distance between holster and tool detached from hand
                float distanceToHolder = Vector3.Distance(gameObject.transform.position, holster.transform.position);

                // Check if holster has already a tool inside
                ICanHolster toolInHolster = holster.GetComponentInChildren<ICanHolster>();

                // If holster is free and tool is close to holster (less than 0.25m)
                if (toolInHolster == null && distanceToHolder < distanceTrigger)
                {
                    gameObject.transform.rotation = holster.transform.GetChild(this._SnapPosition).rotation;
                    gameObject.transform.position = holster.transform.GetChild(this._SnapPosition).position;

                    // Set object to kinematic
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;

                    // Hide holster placeholder
                    holster.transform.GetChild(holster.transform.childCount - 1).GetComponent<MeshRenderer>().enabled = false;

                    // Make tool child of the holster
                    gameObject.transform.parent = holster.transform;

                    isInHolster = true;
                }
            }
        }
        
        private void OnAttachedToHand(Hand hand)
        {           
            // If tool is in holster
            if (isInHolster)
            {
                // Disable tool kinematic (activated for holster)
                gameObject.transform.GetComponent<Rigidbody>().isKinematic = false;
                // Show holster placeholder
                gameObject.transform.parent.transform.GetChild(gameObject.transform.parent.childCount - 2).GetComponent<MeshRenderer>().enabled = true;
                // Reset tool parent
                gameObject.transform.parent = null;
                
                isInHolster = false;
            }
        }

        // Just pulse vibrate when entering collision
        //void OnCollisionEnter(Collision other)
        //{
        //    if (interactable.attachedToHand)
        //    {
        //        interactable.attachedToHand.TriggerHapticPulse(3500);
        //    }
        //}

        // Vibrate constantly when colliding with something
        private void OnCollisionStay(Collision collision)
        {
            if (interactable.attachedToHand)
            {
                SteamVR_Input_Sources hand = interactable.attachedToHand.handType;
                interactable.attachedToHand.TriggerHapticPulse(1/100000, 160, 0.3f);
            }
        }
    }
}
