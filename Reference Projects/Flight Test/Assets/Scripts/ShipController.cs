using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

	/*--- Variable Declarations ---*/

	public GameObject cameraEmpty;
	public Camera camera;
	public GameObject trailEmitterLeft;
	public GameObject trailEmitterRight;

	private static float DEFAULT_FOV = 70f;
	private static float BOOST_FOV = 80f;
	private static float boostAnimationTime = .2f;

	private Rigidbody myRigidBody;
	private TrailRenderer trailLeft;
	private TrailRenderer trailRight;

	private float boostTimeStart = 0f;
	private float boostTimeEnd = 0f;
	private float boostFOVStart = DEFAULT_FOV;
	private float boostFOVEnd = DEFAULT_FOV;
	private bool boostChange = false;
	private bool boostAnimating = false;
	private bool boost = false;


	/*--- Lifecycle Methods ---*/

    // Called Before First Frame Update
    void Start() {
    	myRigidBody = GetComponent<Rigidbody>();
    	trailLeft = trailEmitterLeft.GetComponent<TrailRenderer>();
    	trailRight = trailEmitterRight.GetComponent<TrailRenderer>();
    }

    // Called Regularly (Regardless Of Framerate)
    void FixedUpdate() {

    	/*--- Detect Keypresses ---*/

    	if (Input.GetKey(KeyCode.LeftShift) != boost
    		&& (Input.GetAxis("ControlForwardBack") > 0
    			|| !Input.GetKey(KeyCode.LeftShift))) {
    		boostChange = true;
    	} else {
    		boostChange = false;
    	}
    	boost = Input.GetKey(KeyCode.LeftShift);
  

    	/*--- Update Ship Translation ---*/

        // Get Movement Inputs Since Update
        float moveForwardBack = Input.GetAxis("ControlForwardBack");
        float moveUpDown = Input.GetAxis("ControlUpDown");

        // Create Individual Force Vectors
        Vector3 forceForwardBack = transform.forward * 10 * (moveForwardBack);
        Vector3 forceUpDown = transform.up * 10 * (moveUpDown);

        // Apply Boost
        if (moveForwardBack > 0 && boost) {
        	forceForwardBack *= 2;
        }

        // Combine Force Vectors
        Vector3 combinedForce = forceForwardBack + forceUpDown;

        // Apply Force to Rigidbody Component
        myRigidBody.AddForce(combinedForce);
        //myRigidBody.velocity = combinedForce;


        /*--- Update Ship Rotation ---*/

        // Calculate Roll
        float rotateRoll = Input.GetAxis("ControlRoll");

        // Calculate Pitch & Yaw
        float mousePercentXRaw = (Screen.width/2 - (Input.mousePosition[0] + 20)) / (Screen.height * .75f);
        float mousePercentYRaw = (Screen.height/2 - (Input.mousePosition[1] - 20)) / (Screen.height * .75f);
        float mousePercentX = Mathf.Sign(mousePercentXRaw) * Mathf.Pow(mousePercentXRaw, 2);
        float mousePercentY = Mathf.Sign(mousePercentYRaw) * Mathf.Pow(mousePercentYRaw, 2);

        // Apply Transformations
        float shipPitch = mousePercentY * 5;
        float shipYaw = -mousePercentX * 5;
        float shipRoll = -rotateRoll;

        // Apply Rotation
        transform.Rotate(
        	shipPitch,
        	shipYaw,
        	shipRoll,
        	Space.Self
        );


        /*--- Update Camera ---*/

        // Calculate Camera Shift
        float cameraShiftX = -mousePercentY * 3;
        float cameraShiftY = mousePercentX * 3;
        float cameraShiftZ = 0;

        // Apply Camera Rotation
        cameraEmpty.transform.eulerAngles = new Vector3(
        	transform.eulerAngles.x + cameraShiftX,
        	transform.eulerAngles.y + cameraShiftY,
        	transform.eulerAngles.z + cameraShiftZ
    	);

    	// Apply Camera Movement
    	cameraEmpty.transform.localPosition = new Vector3(
    		-mousePercentX * .3f,
    		-mousePercentY * .3f,
    		0
    	);


    	/*--- Update Boost Variables ---*/

		// Set Boost Variables on Change
    	if (boostChange) {
    		boostTimeStart = Time.fixedTime;
    		boostTimeEnd = boostTimeStart + boostAnimationTime;
    		boostFOVStart = camera.fieldOfView;
    		boostAnimating = true;
    		if (boost) {
    			boostFOVEnd = BOOST_FOV;
    		} else {
    			boostFOVEnd = DEFAULT_FOV;
    		}
    		trailLeft.emitting = boost;
    		trailRight.emitting = boost;
    	}

    	// Note FOV Animation Completion
    	if (Time.fixedTime >= boostTimeEnd) {
    		boostAnimating = false;
    	}
    }


    // Called Once Per Frame
    void Update() {

    	
    	/*--- Update FOV ---*/

    	// If FOV Animation Incomplete
    	if (boostAnimating) {
    		
    		// Calculate & Set FOV
    		float timeElapseFactor = 1 - Mathf.Abs((boostTimeEnd - Time.fixedTime)/(boostTimeEnd - boostTimeStart));
			float normalizedInput = timeElapseFactor * (Mathf.PI / 3);
			float fovFactor = .5f * (1 + Mathf.Sin(3 * normalizedInput - Mathf.PI / 2));
			float currentFOV = boostFOVStart + (fovFactor * (boostFOVEnd - boostFOVStart));
			camera.fieldOfView = currentFOV;
    	}
    }
}
