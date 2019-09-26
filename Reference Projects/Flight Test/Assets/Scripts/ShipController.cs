using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	public GameObject cameraEmpty;
	public Camera camera;

	private float DEFAULT_FOV = 60f;
	private float BOOST_FOV = 70f;

	private Rigidbody myRigidBody;

	private float boostTimeStart = 0f;
	private float boostTimeEnd = 0f;
	private float boostFOVStart = 0f;
	private float boostFOVEnd = 0f;
	private float boostAnimationTime = .5f;
	private bool boostChange = false;
	private bool boost = false;

    // Start is called before the first frame update
    void Start()
    {
    	myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {

    	/*--- Detect Keypresses ---*/

    	if (Input.GetKey(KeyCode.LeftShift) != boost) {
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


    	/*--- Update Camera FOV ---*/

    	if (boostChange) {

    		// Get Boost Variables
    		boostTimeStart = Time.fixedTime;
    		boostTimeEnd = boostTimeStart + boostAnimationTime;
    		boostFOVStart = camera.fieldOfView;
    		if (boost) {
    			boostFOVEnd = BOOST_FOV;
    		} else {
    			boostFOVEnd = DEFAULT_FOV;
    		}

    		// Calculate & Set FOV
    		if (camera.fieldOfView < boostFOVEnd) {
    			camera.fieldOfView = BOOST_FOV;
    		} else {
    			camera.fieldOfView = DEFAULT_FOV;
    		}
    	}
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
