﻿using System;
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
        [SerializeField]
        private int _SnapPosition;

        [SerializeField]
        private float distanceTrigger = 0.2f;

        [SerializeField]
        private Material defaultMaterial = null;

        [SerializeField]
        private Material greenMaterial = null;

        int ICanHolster.SnapPosition { get => _SnapPosition; set => _SnapPosition = value; }
        private bool isInHolster = false;

        private void OnTriggerStay(Collider other)
        {
            try
            {
                float distanceToHolder = Vector3.Distance(gameObject.transform.position, other.transform.position);
                if (distanceToHolder < distanceTrigger)
                {
                    other.GetComponent<Renderer>().material = greenMaterial;
                    other.GetComponent<Outline>().OutlineColor = new Color(0, 255, 0);
                }
                else
                {
                    other.GetComponent<Renderer>().material = defaultMaterial;
                    other.GetComponent<Outline>().OutlineColor = new Color(255, 255, 0);
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        private void OnTriggerExit(Collider other)
        {
            try
            {
                other.GetComponent<Renderer>().material = defaultMaterial;
                other.GetComponent<Outline>().OutlineColor = new Color(255, 255, 0);
            }
            catch (MissingComponentException)
            {
                // do nothing
            }
        }

        private void OnDetachedFromHand(Hand hand)
        {
            // Find holsters
            GameObject[] holsters = GameObject.FindGameObjectsWithTag("holster");
            foreach (GameObject holster in holsters)
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
    }
}
