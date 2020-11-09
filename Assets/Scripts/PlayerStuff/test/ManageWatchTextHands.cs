
using System;
using TMPro;
using UnityEngine;

public class ManageWatchTextHands : MonoBehaviour, ILookingReceiver
{
    public TextMeshPro txt;
    private bool isLookingAt;

    public void LookingAt()
    {
        isLookingAt = true;
    }

    public void NotLookingAt()
    {
        isLookingAt = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        txt.gameObject.SetActive(false);
        Debug.Log("INIT TEXT");
    }

    // Update is called once per frame
    void Update()
    {
        // Show textmesh
        if (isLookingAt)
        {
            Debug.Log("LOOK AT");
            txt.gameObject.SetActive(true);
        } 
        // Hide textmesh
        else
        {
            Debug.Log("NOT LOOK AT");
            txt.gameObject.SetActive(false);
        }
    }
}
