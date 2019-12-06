using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Variables
	public float temporalOffsetFactor = 0f;

	// Private Constants
	private float CYCLE_LENGTH = 13f;

	// Private Variables
	private float currentTime = 0f;
	private Vector3 initialPosition;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
        // Get Initial Position
        initialPosition = transform.localPosition;
    }

    void Update() {

    	// Update Time
        currentTime += Time.deltaTime;

    	// Calculate Y Position Offset
        float factor = currentTime + (temporalOffsetFactor * CYCLE_LENGTH);
        float normalizedFactor = factor * (Mathf.PI / (1.5f * CYCLE_LENGTH));
        float positionOffsetFactorY = (.5f * (1 + Mathf.Sin(3f * normalizedFactor - Mathf.PI / 2f))) - 1;

        // Calculate X Position Offset
        factor = currentTime + (temporalOffsetFactor * CYCLE_LENGTH) + 3f;
        normalizedFactor = factor * (Mathf.PI / (1.5f * CYCLE_LENGTH));
        float positionOffsetFactorX = (.5f * (1 + Mathf.Sin(3f * normalizedFactor - Mathf.PI / 2f))) - 1;

    	// Update Rotation
        transform.localPosition = new Vector3(
        	initialPosition.x + (positionOffsetFactorX * .75f),
        	initialPosition.y + (positionOffsetFactorY * .2f),
        	initialPosition.z
        );
    }
}
