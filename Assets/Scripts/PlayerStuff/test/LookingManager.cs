using UnityEngine;

public class LookingManager : MonoBehaviour
{
    public Camera viewCamera;
    private GameObject lastLookedAtObj;

    private void CheckLook()
    {
        if (lastLookedAtObj)
        {
            lastLookedAtObj.SendMessage("NotLookingAt", SendMessageOptions.DontRequireReceiver);
        }

        Ray gazeRay = new Ray(viewCamera.transform.position, viewCamera.transform.rotation * Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(gazeRay, out hit, Mathf.Infinity))
        {
            hit.transform.SendMessage("LookingAt", SendMessageOptions.DontRequireReceiver);
            lastLookedAtObj = hit.transform.gameObject;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckLook();
    }
}
