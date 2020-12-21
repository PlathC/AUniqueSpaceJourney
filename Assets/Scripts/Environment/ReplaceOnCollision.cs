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

    public void DisparitionEffect()
    {
        if (replacementPrefab)
        {
            GameObject newPrefab = Instantiate(replacementPrefab, transform.position, transform.rotation);
            newPrefab.transform.localScale = transform.localScale;
        }

        if (disparitionEffect)
        {
            Instantiate(disparitionEffect, transform.position, transform.rotation, gameObject.transform);
            var visualEffect = disparitionEffect.GetComponent<VisualEffect>();
            
            if (visualEffect)
            {
                visualEffect.Play();
            }
        }
        Destroy(gameObject);
    }
}
