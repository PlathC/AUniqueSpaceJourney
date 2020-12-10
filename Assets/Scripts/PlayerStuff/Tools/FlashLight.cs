using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace AUSJ
{
    public class FlashLight : MonoBehaviour
    {
        private SteamVR_Action_Boolean toggleFlashLight;
        private SteamVR_Input_Sources inputSource = SteamVR_Input_Sources.Any; //which controller

        private bool locked = true;

        public bool Locked { get => locked; set => locked = value; }

        // Start is called before the first frame update
        void Start()
        {
            toggleFlashLight = SteamVR_Input.GetBooleanAction("FlashLightToggle");
            toggleFlashLight.AddOnChangeListener(ToggleFlashLight, inputSource);
        }

        private void ToggleFlashLight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (!locked)
            {
                GetComponent<Light>().enabled = newState;
            }
        }
    }
}
