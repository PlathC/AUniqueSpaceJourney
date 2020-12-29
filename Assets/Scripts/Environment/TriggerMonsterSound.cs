using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AUSJ
{
    [RequireComponent(typeof(Collider))]
    public class TriggerMonsterSound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] monsterSounds = null;

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "PlayerCollider")
            {
                AudioSource audioSource = transform.GetChild(0).GetComponent<AudioSource>();

                // Trigger monster sound
                audioSource.PlayOneShot(monsterSounds[Random.Range(0, monsterSounds.Length)]);
            }
        }
    }
}
