using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class ReplaceOnCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject ReplacementPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(ReplacementPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
