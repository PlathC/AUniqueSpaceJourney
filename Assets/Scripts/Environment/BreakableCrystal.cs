using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace AUSJ
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(ReplaceOnCollision))]
    [RequireComponent(typeof(AudioSource))]
    public class BreakableCrystal : MonoBehaviour
    {
        [SerializeField]
        private int nbHitsBeforeBreak = 3;
    
        [SerializeField]
        private int hungerRestored = 8; // in %

        [SerializeField]
        private AudioClip[] breakingClips = null;
        
        [SerializeField]
        private AudioClip[] hitClips = null;

        private AudioSource audioSource = null;
        private int nbHitsReceived = 0;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();   
        }

        private void PlayRandomSound(AudioClip[] clips)
        {
            // Play sound random in sound collection
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // If hammer is hitting the crystal
            if (collision.gameObject.tag == "hammer")
            {
                if (nbHitsReceived < nbHitsBeforeBreak)
                {
                    // Receive hit
                    nbHitsReceived++;

                    // Vibrate controller
                    StartCoroutine(Vibrations.PulseAscend(collision.gameObject.transform.GetComponent<Interactable>().attachedToHand, 2, 0.2f, 0.2f));

                    // Make sound
                    PlayRandomSound(hitClips);

                    // TODO : Change crystal color

                }
                // Break crystal
                else
                {
                    // Make sound
                    PlayRandomSound(breakingClips);

                    // Make vibrations
                    Vibrations.SimplePulse(collision.gameObject.transform.GetComponent<Interactable>().attachedToHand, 0.5f);

                    // Restore player hunger 
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>().RestoreHunger(hungerRestored);

                    // Break crystal
                    gameObject.GetComponent<ReplaceOnCollision>().BreakCrystal();
                }
            }
        }
    }
}

