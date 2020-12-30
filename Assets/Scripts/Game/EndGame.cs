using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private GameObject catPrefab;

    [SerializeField]
    private GameObject catSpawner;

    [SerializeField]
    private AudioClip endMonsterSound;

    private bool endGameTriggered = false;

    public bool EndGameTriggered { get => endGameTriggered; set => endGameTriggered = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerCollider"))
        {
            var cat = Instantiate(catPrefab, catSpawner.transform.position, catSpawner.transform.rotation);
            var source = cat.GetComponent<AudioSource>();
            source.PlayOneShot(endMonsterSound);

            StartCoroutine(FinalizeGameEnd(10));
        }
    }

    public IEnumerator FinalizeGameEnd(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        EndGameTriggered = true;
    }
}
