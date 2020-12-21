using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(AudioSource))]
    public class Watch : MonoBehaviour
    {
        private AudioSource audioSource;
        private Hand attachedHand = null;

        private TextMeshPro playerScreen = null;
        private bool lookingWatch = false;

        public bool LookingWatch { get => lookingWatch; set => lookingWatch = value; }
        public TextMeshPro PlayerScreen { get => playerScreen; set => playerScreen = value; }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            attachedHand = gameObject.transform.parent.GetComponent<Hand>();

            try
            {
                // Get screen in children
                PlayerScreen = GetComponentInChildren<TextMeshPro>();
            }
            catch(Exception)
            {
                Debug.LogError("Player screen not found in Watch class");
                Application.Quit(-1);
            }
        }

        public void EnableWatch()
        {
            // Activate watch => Blue color
            GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing blue"));

            // Enable player screen
            playerScreen.transform.gameObject.SetActive(true);
        }

        public void DisableWatch()
        {
            // Disable watch => Red color
            GetComponent<Renderer>().material = new Material(Shader.Find("Shader Graphs/Glowing red"));

            // Enable player screen
            playerScreen.transform.gameObject.SetActive(false);
        }

        public void SendWatchNotification()
        {
            StartCoroutine(WatchNotification());
        }

        public void PlayWarningSound()
        {
            audioSource.Play();
        }

        private IEnumerator WatchNotification()
        {
            while (!LookingWatch)
            {
                // Play sound on watch
                audioSource.Play();
                
                // Vibrate controller
                Vibrations.SimplePulse(attachedHand, 0.3f);
                
                // Wait for 2 sec and repeat
                yield return new WaitForSeconds(2);
            }
            //yield return new WaitForSeconds(2);
            //LookingWatch = false;
        }
    }
}
