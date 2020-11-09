using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideControler : MonoBehaviour
{
    public GameObject toHideControlPrefab;
    public string buttonName = "Action 1";
    
    private GameObject m_instantiatedPrefab;
    private bool m_isHidden = false;
    
        
    void Start()
    {
        m_instantiatedPrefab = Instantiate(toHideControlPrefab, new Vector3(0, 0, 0), toHideControlPrefab.transform.rotation);
        SetEnableChildrensMeshRenderer(m_isHidden);
        m_instantiatedPrefab.transform.SetParent(gameObject.transform);
        m_instantiatedPrefab.transform.localPosition = new Vector3(0, 0.01f, 0);
    }
    
    void Update()
    {
        if (Input.GetButtonDown(buttonName))    
        {
            m_isHidden = !m_isHidden;
            SetEnableChildrensMeshRenderer(m_isHidden);
        }
    }

    void SetEnableChildrensMeshRenderer(bool value)
    {
        m_instantiatedPrefab.GetComponent<Renderer>().enabled = value;
        Renderer[] rs = m_instantiatedPrefab.GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs)
            r.enabled = value;
    }
}
