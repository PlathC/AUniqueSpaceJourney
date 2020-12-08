using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUSJ
{
    public class HolsterFollowMovement : MonoBehaviour
    {
        private GameObject VRCamera;
        private float rotationSpeed = 100;
        public bool followHeadPosition = true;
        public bool followHeadRotation = false;

        private float playerSize = 0;

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                //VRCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
                VRCamera = Camera.main.gameObject;
                playerSize = VRCamera.transform.position.y / 2f;
                // transform.parent = VRCamera.transform;
                //transform.position = new Vector3(0.07f, - VRCamera.transform.position.y / 2, -0.2f);
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("MainCamera TAG not found");
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (followHeadPosition)
            {
                transform.position = new Vector3(VRCamera.transform.position.x + 0.07f, VRCamera.transform.position.y - playerSize, VRCamera.transform.position.z - 0.2f);
                // Debug.Log("transform ==> " + transform.position);
                //transform.position = new Vector3(VRCamera.transform.position.x + 0.07f, VRCamera.transform.position.y / 2, VRCamera.transform.position.z - 0.2f);
                //Vector3 newPosition = VRCamera.transform.forward;
                //newPosition.y = VRCamera.transform.position.y / 2;
                //transform.position = VRCamera.transform.forward;
            }


            if (followHeadRotation)
            {
                float rotationDiff = Math.Abs(VRCamera.transform.eulerAngles.y - transform.eulerAngles.y);
                float finalRotationSpeed = rotationSpeed;

                // Make rotation speed faster if holster rotation is further away from the VR Camera
                if (rotationDiff > 60)
                {
                    finalRotationSpeed = rotationSpeed * 2;
                }
                else if (rotationDiff > 40 && rotationDiff < 60)
                {
                    finalRotationSpeed = rotationSpeed;
                }
                else if (rotationDiff < 40 && rotationDiff > 20)
                {
                    finalRotationSpeed = rotationSpeed / 2;
                }
                else if (rotationDiff < 20 && rotationDiff > 0)
                {
                    finalRotationSpeed = rotationSpeed / 4;
                }

                // The step size is equal to speed times frame time
                float step = finalRotationSpeed * Time.deltaTime;

                // Rotation our transform a step closer to the target's
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, VRCamera.transform.eulerAngles.y, 0), step);
            }
        }
    }
}
