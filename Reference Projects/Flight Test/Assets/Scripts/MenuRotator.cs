using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotator : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Variables
	public float temporalOffsetFactor = 0f;

	// Private Variables
	private float currentTime = 0f;
	private float cycleLength = 0f;
	private Vector3 initialRotation;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
        // Get Initial Rotation
        initialRotation = transform.eulerAngles;
    }

    void Update() {

    	// Get Cycle Length
    	cycleLength = MenuPositioner.instance.cycleLength;

    	// Update Time
        currentTime += Time.deltaTime;

        // Calculate Rotation
    	float rotationFactor =
    		((currentTime + (temporalOffsetFactor * cycleLength)) % cycleLength) / cycleLength;

    	// Update Rotation
        transform.eulerAngles = new Vector3(
        	0,
        	initialRotation.y + (rotationFactor * 360f),
        	0
        );
    }
}
