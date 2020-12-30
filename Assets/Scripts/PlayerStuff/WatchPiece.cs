using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Interactable))]
    public class WatchPiece : MonoBehaviour
    {
        private GameObject VRCamera;
        private Interactable interactable;

        private void Start()
        {
            interactable = GetComponent<Interactable>();
            try
            {
                VRCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("MainCamera TAG not found");
            }
        }

        public void CatchPiece()
        {
            // Detach object
            interactable.attachedToHand.DetachObject(gameObject);

            // Make piece disapeare
            gameObject.SetActive(false);

            // +1 piece gathered
            Tutorial tuto = VRCamera.GetComponent<Session>().CurrentState as Tutorial;
            tuto.NbPiecesGatheredWatch++;

            // Make sound

            // Print something on screen ?

        }
    }
}
