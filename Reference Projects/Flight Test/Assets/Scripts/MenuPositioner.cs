using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPositioner : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Constants
    public static MenuPositioner instance;

	// Public Variables
	public float verticalOffset = 80f;
	public float horizontalOffset = 80f;
	public float verticalTemporalOffsetFactor = 0f;
	public float cycleLength = 50f;

	// Private Variables
	private float currentTime = 0f;
	private Vector3 initialPosition;
	private float verticalTemporalOffset = 0f;


	/*--- Lifecycle Methods ---*/

    void Start() {

    	// Set Instance
        instance = this;
        
        // Get Initial Position/Rotation
        initialPosition = transform.localPosition;

        // Calculate Offsets
        verticalTemporalOffset = verticalTemporalOffsetFactor * cycleLength;
    }

    void Update() {

    	// Update Time
        currentTime += Time.deltaTime;

    	// Calculate Vertical Position
    	float verticalFactor = (((currentTime + verticalTemporalOffset) % cycleLength) / cycleLength) * 2f;
        float normalizedVerticalFactor = verticalFactor * (Mathf.PI / 3f);
        float positionOffsetFactorVertical = .5f * (1 + Mathf.Sin(3f * normalizedVerticalFactor - Mathf.PI / 2f));

        // Calculate Horizontal Position
        float horizontalFactor = Mathf.Abs((((currentTime + verticalTemporalOffset) % cycleLength) / cycleLength) * 2f);
        float normalizedHorizontalFactor = horizontalFactor * (Mathf.PI / 3f);
        float positionOffsetFactorHorizontal = .5f * (1 + Mathf.Sin(3f * normalizedHorizontalFactor - Mathf.PI / 2f));

        // Calculate Horizontal Position 2
        float horizontalFactor2 = (((currentTime + (.50f * cycleLength)) % cycleLength) / cycleLength) * 2f;
        float normalizedHorizontalFactor2 = horizontalFactor2 * Mathf.PI;
        float positionOffsetFactorHorizontal2 = .5f * (1 + Mathf.Sin(normalizedHorizontalFactor2));

        // Update Position
        transform.localPosition = new Vector3(
        	initialPosition.x + (positionOffsetFactorHorizontal * horizontalOffset) + (positionOffsetFactorHorizontal2 * 80),
        	initialPosition.y - (positionOffsetFactorVertical * verticalOffset),
        	initialPosition.z
        );

        // Calculate Rotation
        float angleFactor = (((currentTime + (.6f * cycleLength)) % cycleLength) / cycleLength) * 2f;
        float normalizedAngleFactor = angleFactor * (Mathf.PI / 3f);
        float positionOffsetAngleHorizontal = .5f * (1 + Mathf.Sin(3f * normalizedAngleFactor - Mathf.PI / 2f));

        // Calculate Rotation 2
        float angleFactor2 = (((currentTime + (.25f * cycleLength)) % cycleLength) / cycleLength) * 4f;
        float normalizedAngleFactor2 = angleFactor2 * Mathf.PI;
        float angleOffsetFactor2 = .5f * (1 + Mathf.Sin(normalizedAngleFactor2));
        angleOffsetFactor2 = Mathf.Pow(angleOffsetFactor2, 4);

        // Calculate Rotation 3
        float angleFactor3 = (((currentTime + (-.2f * cycleLength)) % cycleLength) / cycleLength) * 2f;
        float normalizedAngleFactor3 = angleFactor3 * (Mathf.PI / 3f);
        float positionOffsetAngleHorizontal3 = .5f * (1 + Mathf.Sin(3f * normalizedAngleFactor3 - Mathf.PI / 2f));
        positionOffsetAngleHorizontal3 = Mathf.Pow(positionOffsetAngleHorizontal3, 4);

        // Update Rotation
        transform.localEulerAngles = new Vector3(
        	20 - (positionOffsetFactorVertical * 15),
        	83 - (angleOffsetFactor2 * 1.5f) - (positionOffsetFactorVertical * 5f) + (positionOffsetAngleHorizontal3 * 4f),
        	0
        );

    }
}
