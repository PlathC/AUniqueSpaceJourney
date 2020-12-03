using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Collider))]
public class ReplaceOnCollision : MonoBehaviour
{
    [SerializeField]
    private GameObject replacementPrefab = null;

    [SerializeField]
    private GameObject disparitionEffect = null;

    private void OnCollisionEnter(Collision collision)
    {
        if(replacementPrefab)
        {
            Instantiate(replacementPrefab, transform.position, transform.rotation);
        }

        if (disparitionEffect)
        {
            Instantiate(disparitionEffect, transform.position, transform.rotation);
            var visualEffect = disparitionEffect.GetComponent<VisualEffect>();
            if (visualEffect)
            {
                visualEffect.Play();
            }
        }
        Instantiate(disparitionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
