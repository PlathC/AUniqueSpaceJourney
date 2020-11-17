using UnityEngine;

// USAGE: put this on a thing you want the player to look at
// this code will enable that thing to know if it is being looked at!
[RequireComponent(typeof(Collider))] // this thing needs a collider if we should look at it
public class LookingHandScreen : MonoBehaviour
{
	private new Camera camera;
	private bool amIBeingLookedAt = false;
    public GameObject hand;
	public float maximumDistance = 10f;
	public bool debug = false;

    private void showScreen(bool show)
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
        {
            r.enabled = show;
        }
    }

    // Find main camera in the scene
	private void Start()
    {
		if (Camera.main)
        {
			camera = Camera.main;
            showScreen(false);
        } 
		else
        {
			throw new System.Exception("Camera not found");
        }
    }

    void OnStartLook()
	{ // happen the first frame when I start looking at something
		if (debug)
        {
			Debug.Log("user STARTED looking at object " + name);
        }
        showScreen(true);
    }

    void OnStopLook()
	{ // will happen the first frame when I STOP looking at something
		if (debug)
        {
			Debug.Log("user STOPPED looking at object " + name);
        }
        showScreen(false);
    }

    void OnNotLooking()
	{ // will constantly happen every frame, while thing is NOT being looked at
	  // Debug.Log("user is still looking at object " + name );
	}

	void OnLooking()
	{   // will constantly happen every Update, while thing is being looked at
        // Debug.Log("user is still NOT looking at object " + name );
        if (debug)
        {
            Debug.Log("Hand position : " + hand.transform.rotation);
        }

        // showScreen(true);

        //if (hand.transform.rotation.y > 85 && hand.transform.rotation.y < 110
        //    && hand.transform.rotation.z < -85 && hand.transform.rotation.z > -110)
        //{

        //}
    }

    // Update is called once per frame
    void Update()
	{
        // Debug.Log("Hand position : " + hand.transform.rotation);

        // STEP 1: setup a Ray variable before we fire a Raycast
        Vector3 rayOrigin = camera.transform.position;

		// shoot a ray based on the HMD's reported rotation
		Vector3 rayDirection = camera.transform.rotation * Vector3.forward;

		// actually construct the ray
		Ray ray = new Ray(rayOrigin, rayDirection);

		if (debug)
        {
			// visualize all this stuff in Scene View
			Debug.DrawRay(ray.origin, ray.direction * maximumDistance, Color.yellow);
        }

        // STEP 2: get var rayHitInfo to know where we hit something

        // note that a Raycast is an infinitely thin line... which means it only detects things that
        // are in the exact middle of the screen... if you want a "thick Raycast", look up "Physics.Spherecast"

        // STEP 3: actually shoot the raycast now
        if (Physics.Raycast(ray, out RaycastHit rayHitInfo, maximumDistance) && rayHitInfo.collider == GetComponent<Collider>())
        {
            if (debug)
            {
                // visualize the successful raycast as a red line, using the actual distance from impact
                Debug.DrawRay(ray.origin, ray.direction * rayHitInfo.distance, Color.green);
                // visualize the impact point as a small magenta line, pointing based on the surface's curvature
                Debug.DrawRay(rayHitInfo.point, rayHitInfo.normal, Color.magenta);
            }

            if (amIBeingLookedAt == false)
            {
                OnStartLook();
                amIBeingLookedAt = true;
            }
            OnLooking();
        }
        else
        { // what if the user is NOT looking at this thing?...
            if (amIBeingLookedAt == true)
            {
                OnStopLook();
                amIBeingLookedAt = false;
            }
            OnNotLooking();
        }
    }

}
