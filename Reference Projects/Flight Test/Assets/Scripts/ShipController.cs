using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{

	private Rigidbody myRigidBody;

    // Start is called before the first frame update
    void Start()
    {
    	myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {

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

        // Get Rotation Inputs Since Update
        Vector2 screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        Vector2 mousePosition = Input.mousePosition;
        float rotateRoll = Input.GetAxis("ControlRoll");
        Vector2 screenRotation = new Vector2(
        	(screenCenter[0] - mousePosition[0]) / Screen.width,
        	(screenCenter[1] - mousePosition[1]) / Screen.height
        );


        // Apply Rotation
        transform.Rotate(
        	screenRotation[1] * 5,
        	-screenRotation[0] * 5,
        	-rotateRoll,
        	Space.Self
        );
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
