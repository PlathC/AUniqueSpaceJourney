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
        private bool suckingBlocked = false;

        // Water source sizes and scale changes
        private int timeBlockNextSource = 5;  // in seconds
        private float timeBeforeNextSuckingStep = 1.0f; // in seconds
        private int waterSourceDecreaseStep = 0;
        private float sizeDecreaseStep = 0.05f;
        private float radiusIncreaseStep = 0.2f;
        private float waterSourceRestoreThirst = 0.08f;
        
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

        private IEnumerator WaitBeforeNextSucking()
        {
            suckingBlocked = true;
            for (int i = 0; i < timeBlockNextSource; i++)
            {
                vacuumBag.GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing red"));
                yield return new WaitForSeconds(0.5f);
                vacuumBag.GetComponent<Renderer>().material = defaultMaterial;
                yield return new WaitForSeconds(0.5f);
            }
            suckingBlocked = false;
        }

        private IEnumerator SuckWater()
        {
            while (suckingWater)
            {
                yield return new WaitForSeconds(timeBeforeNextSuckingStep);

                if (waterSourceInVacuumFOV)
                {
                    // Add water to profile bar
                    // ... using suckingValue
                    int restoreThirst = (int)(suckingValue * waterSourceRestoreThirst);
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>().RestoreThirst(restoreThirst);

                    // Decrease water source scale
                    Vector3 currentScale = waterSourceInVacuumFOV.gameObject.transform.localScale;
                    waterSourceDecreaseStep++;
                    Debug.Log(currentScale);
                    // If too small => disable render
                    if (currentScale.x <= 0.2f)
                    {
                        // Animation disappearance
                        waterSourceInVacuumFOV.gameObject.GetComponent<ReplaceOnCollision>().DisparitionEffect();

                        // Hide gameobject
                        //waterSourceInVacuumFOV.gameObject.SetActive(false);
                    }
                    else
                    {
                        // Update scale
                        Vector3 newScale = new Vector3(currentScale.x - sizeDecreaseStep, currentScale.y - sizeDecreaseStep, currentScale.z - sizeDecreaseStep);
                        waterSourceInVacuumFOV.gameObject.transform.localScale = newScale;

                        // Increase collider size
                        float currentRadius = waterSourceInVacuumFOV.gameObject.GetComponent<SphereCollider>().radius;
                        waterSourceInVacuumFOV.gameObject.GetComponent<SphereCollider>().radius = currentRadius + radiusIncreaseStep * waterSourceDecreaseStep;

                        // Update speed
                        waterSourceInVacuumFOV.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_Speed", 3);
                    }
                }
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
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ResetWaterSucking()
        {
            suckingWater = false;
            suckingValue = 0;
            waterSourceDecreaseStep = 0;

            // Stop sucking
            StopCoroutine(SuckWater()); // Not necessary ??

            // Stop vibration
            StopCoroutine(PulseWave(interactable.attachedToHand));

            // Stop animation
            if (lastWaterSource && lastWaterSource.gameObject.activeSelf)
            {
                lastWaterSource.transform.GetChild(0).gameObject.SetActive(false);
                lastWaterSource.gameObject.GetComponent<Renderer>().sharedMaterial.SetFloat("_Speed", 1);

                if (lastWaterSource.gameObject.transform.localScale.x <= 0.2f)
                {
                    Destroy(lastWaterSource.gameObject);
                }
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
                    if (!suckingWater && !suckingBlocked)
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

                    // Block new sucking attempts for x seconds to avoid sucking water too fast
                    StartCoroutine(WaitBeforeNextSucking());
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
