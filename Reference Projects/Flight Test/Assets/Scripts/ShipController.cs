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
        float rotateRoll = Input.GetAxis("ControlRoll");
        float rotatePitch = Input.GetAxis("Mouse Y") * 2;
        float rotateYaw = Input.GetAxis("Mouse X") * 2;

        // Apply Rotation
        transform.Rotate(-rotatePitch, rotateYaw, -rotateRoll, Space.Self);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
