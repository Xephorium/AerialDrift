using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour {

	/*--- Variable Declarations ---*/

	// Public Variables
	public float verticalOffset = 5f;
	public float temporalOffset = 0f;
	public float cycleLength = 5f;

	// Private Variables
	private float currentTime = 0f; 
	private Vector3 initialPosition;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
        // Get Initial Position
        initialPosition = transform.position;
    }

    void FixedUpdate() {
        
        // Update Time
        currentTime += Time.deltaTime;

        // Calculate Position
        float factor = currentTime + temporalOffset;
        float normalizedFactor = factor * (Mathf.PI / (1.5f * cycleLength));
        float positionOffsetFactor = .5f * (1 + Mathf.Sin(3f * normalizedFactor - Mathf.PI / 2f));

        // Update Position
        transform.position = new Vector3(
        	initialPosition.x,
        	initialPosition.y + (positionOffsetFactor * verticalOffset),
        	initialPosition.z
        );
    }

}
