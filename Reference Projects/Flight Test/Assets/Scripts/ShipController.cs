using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
	private float DEADZONE_SIZE = .05f;
	private Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
    	myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {


    	/*--- Translation ---*/

        // Get Movement Inputs Since Update
        float moveForwardBack = Input.GetAxis("ControlForwardBack");
        float moveUpDown = Input.GetAxis("ControlUpDown");

        // Create Individual Force Vectors
        Vector3 forceForwardBack = transform.forward * 10 * (moveForwardBack);
        Vector3 forceUpDown = transform.up * 10 * (moveUpDown);

        // Combine Force Vectors
        Vector3 combinedForce = forceForwardBack + forceUpDown;

        // Apply Force to Rigidbody Component
        myRigidBody.AddForce(combinedForce);
        //myRigidBody.velocity = combinedForce;


        /*--- Rotation ---*/

        // Calculate Roll
        float rotateRoll = Input.GetAxis("ControlRoll");

        // Calculate Pitch & Yaw
        float deadZoneRadiusInPixels = (Screen.height * DEADZONE_SIZE) / 2;
        float mousePercentX = (Screen.width/2 - (Input.mousePosition[0] + 20)) / Screen.height;
        float mousePercentY = (Screen.height/2 - (Input.mousePosition[1] - 20)) / Screen.height;

        // Account for Dead Zone
        if (mousePercentX < DEADZONE_SIZE/2 && mousePercentX > -DEADZONE_SIZE/2) {
        	mousePercentX = 0;
        } else {
        	mousePercentX = mousePercentX + (-1 * mousePercentX * (DEADZONE_SIZE/2));
        }
        if (mousePercentY < DEADZONE_SIZE/2 && mousePercentY > -DEADZONE_SIZE/2) {
        	mousePercentY = 0;
        } else {
        	mousePercentY = mousePercentY + (-1 * mousePercentY * (DEADZONE_SIZE/2));
        }

        // Apply Rotation
        transform.Rotate(
        	mousePercentY * 5,
        	-mousePercentX * 5,
        	-rotateRoll,
        	Space.Self
        );
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
