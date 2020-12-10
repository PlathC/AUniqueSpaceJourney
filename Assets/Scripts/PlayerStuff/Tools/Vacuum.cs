using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(AudioSource))]
    public class Vacuum : MonoBehaviour
    {
        private Interactable interactable;
        private SteamVR_Action_Single toggleToolOn;
        private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any; //which controller
        private GameObject VRCamera;
        private GameObject waterSourceInVacuumFOV = null;
        private GameObject lastWaterSource = null;
        private GameObject vacuumBag;
        private Material defaultMaterial;
        private bool suckingWater = false;
        private float suckingValue = 0; // Percentage trigger

        [SerializeField]
        private float minimumTriggerGachette = 0.1f;

        [SerializeField]
        private float maximumDistance = 20f;

        [SerializeField]
        private bool debug = false;

        [SerializeField]
        private AudioClip aspirationSound = null;

        private AudioSource audioSource = null;

        // Start is called before the first frame update
        void Start()
        {
            interactable = GetComponent<Interactable>();
            toggleToolOn = SteamVR_Input.GetSingleAction("EnableTool");
            toggleToolOn.AddOnAxisListener(ComputeSuckWater, inputSource);
            audioSource = GetComponent<AudioSource>();

            // Configuration audio source
            audioSource.loop = true;
            audioSource.clip = aspirationSound;

            try
            {
                vacuumBag = GameObject.FindGameObjectsWithTag("vacuumBag")[0];
                
                // Store default material
                defaultMaterial = vacuumBag.GetComponent<Renderer>().material;
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("MainCamera TAG not found");
            }

            try
            {
                VRCamera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("MainCamera TAG not found");
            }
        }

        private IEnumerator SuckWater()
        {
            float sizeDecreaseStep = 0.1f;
            while (suckingWater)
            {
                // Add water to profile bar
                // ... using suckingValue
                // Decrease water source scale
                Vector3 currentScale = waterSourceInVacuumFOV.gameObject.transform.localScale;
                // If too small => disable render
                if (currentScale.x < 0.1f)
                {
                    waterSourceInVacuumFOV.gameObject.SetActive(false);
                    // Animation disappearance
                } else
                {
                    // Update scale
                    Vector3 newScale = new Vector3(currentScale.x - sizeDecreaseStep, currentScale.y - sizeDecreaseStep, currentScale.z - sizeDecreaseStep);
                    waterSourceInVacuumFOV.gameObject.transform.localScale.Scale(newScale);

                    // Update speed
                }
                yield return new WaitForSeconds(2f);
            }
            yield return null;
        }

        private IEnumerator PulseWave(Hand hand)
        {
            float step = 0.1f;
            float amplitude = 0.5f;
            float duration = 100000;
            float seconds = (float)duration / 1000000f;

            while (suckingWater)
            {
                hand.TriggerHapticPulse(seconds, 1f / seconds, amplitude);
                amplitude += step;
                amplitude = Mathf.Abs(Mathf.Cos(amplitude));
                Debug.Log("VIBRATE : " + amplitude);
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ResetWaterSucking()
        {
            suckingWater = false;
            suckingValue = 0;

            // Stop sucking
            StopCoroutine(SuckWater()); // Not necessary ??

            // Stop vibration
            StopCoroutine(PulseWave(interactable.attachedToHand));

            // Stop animation
            if (lastWaterSource)
            {
                lastWaterSource.transform.GetChild(0).gameObject.SetActive(false);
            }

            // Stop sound
            audioSource.Stop();
        }

        public void ComputeSuckWater(SteamVR_Action_Single fromAction, SteamVR_Input_Sources fromSource, float newAxis, float newDelta)
        {
            if (interactable.attachedToHand)
            {
                // Start sucking water
                if (newAxis > minimumTriggerGachette && waterSourceInVacuumFOV)
                {
                    suckingValue = newAxis * 100;
                    if (!suckingWater)
                    {
                        suckingWater = true;

                        // Start sucking water loop
                        StartCoroutine(SuckWater());

                        // Start vibration
                        StartCoroutine(PulseWave(interactable.attachedToHand));

                        // Start animation
                        waterSourceInVacuumFOV.transform.GetChild(0).gameObject.SetActive(true);
                        lastWaterSource = waterSourceInVacuumFOV;

                        // Make sound
                        audioSource.Play();
                    }
                }
                else if (lastWaterSource && suckingWater)
                {
                    ResetWaterSucking();
                }
            } 
            else
            {
                ResetWaterSucking();
            }
        }

        void OnStartLook()
        { // happen the first frame when I start looking at something
            if (debug)
            {
                Debug.Log("user STARTED looking at object " + name);
            }
            // Vacuum status to green
            vacuumBag.GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing green"));
        }

        void OnStopLook()
        { // will happen the first frame when I STOP looking at something
            if (debug)
            {
                Debug.Log("user STOPPED looking at object " + name);
            }
            vacuumBag.GetComponent<Renderer>().material = defaultMaterial;
        }

        // Update is called once per frame
        void Update()
        {
            // Debug.Log("Hand position : " + hand.transform.rotation);

            Vector3 rayOrigin = vacuumBag.transform.position;
            Vector3 rayDirection = vacuumBag.transform.rotation * Vector3.back;

            Ray ray = new Ray(rayOrigin, rayDirection);

            if (debug)
            {
                // visualize all this stuff in Scene View
                Debug.DrawRay(ray.origin, ray.direction * maximumDistance, Color.yellow);
            }

            // shoot the raycast now
            if (Physics.Raycast(ray, out RaycastHit rayHitInfo, maximumDistance) && rayHitInfo.collider.tag == "waterSource")
            {
                if (debug)
                {
                    // visualize the successful raycast as a red line, using the actual distance from impact
                    Debug.DrawRay(ray.origin, ray.direction * rayHitInfo.distance, Color.green);
                    // visualize the impact point as a small magenta line, pointing based on the surface's curvature
                    Debug.DrawRay(rayHitInfo.point, rayHitInfo.normal, Color.magenta);
                }

                if (waterSourceInVacuumFOV == null)
                {
                    OnStartLook();
                    waterSourceInVacuumFOV = rayHitInfo.transform.gameObject;
                }
            }
            else
            { // what if the user is NOT looking at this thing?...
                if (waterSourceInVacuumFOV != null)
                {
                    OnStopLook();
                    waterSourceInVacuumFOV = null;
                }
            }
        }
    }
}
