using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField]
    private GameObject cat;

    [SerializeField]
    private AudioClip endMonsterSound;

    private bool alreadyTriggered = false;

    private bool endGameTriggered = false;

    public bool EndGameTriggered { get => endGameTriggered; set => endGameTriggered = value; }

    void Awake()
    {
        StartCoroutine(PreloadCat());
    }

    public IEnumerator PreloadCat()
    {
        cat.SetActive(true);
        yield return new WaitForSeconds(2);
        cat.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerCollider") && !alreadyTriggered)
        {
            cat.SetActive(true);
            var source = cat.GetComponent<AudioSource>();
            source.PlayOneShot(endMonsterSound);

            Debug.Log("Launch game ending");
            StartCoroutine(FinalizeGameEnd(10));

            alreadyTriggered = true;
        }
    }

    public IEnumerator FinalizeGameEnd(int seconds)
    {
        yield return new WaitForSeconds(seconds);

        Debug.Log("Set property");
        EndGameTriggered = true;
    }
}
