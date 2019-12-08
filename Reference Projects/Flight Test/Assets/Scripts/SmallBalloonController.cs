using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBalloonController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Variables
	public ParticleSystem explosionLarge;
	public ParticleSystem explosionSmall;
    public float driftCycleLength = 10f;
	public float temporalOffset = 0f;

	// Private Variables
	private float currentTime = 0f;
	private float rotationCycleLength = 20f;
	private float driftDistance = 3f;
	private Vector3 initialPosition;
	private Vector3 initialRotation;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
        // Get Initial Values
        initialPosition = transform.position;
        initialRotation = transform.eulerAngles;
    }

    void Update() {

    	// Update Time
        currentTime += Time.deltaTime;

    	// Update Rotation
    	float rotationFactor = (currentTime % rotationCycleLength) / rotationCycleLength;
        transform.eulerAngles = new Vector3(
        	0,
        	initialRotation.y + (rotationFactor * 360f),
        	0
        );

        // Calculate Position
        float factor = currentTime + temporalOffset;
        float normalizedFactor = factor * (Mathf.PI / (1.5f * driftCycleLength));
        float positionOffsetFactor = .5f * (1 + Mathf.Sin(3f * normalizedFactor - Mathf.PI / 2f));

        // Update Position
        transform.position = new Vector3(
        	initialPosition.x,
        	initialPosition.y + (positionOffsetFactor * driftDistance),
        	initialPosition.z
        );
    }

    void OnTriggerEnter(Collider other) {

    	// Record Pop
    	BalloonMinigameController.instance.recordDestroyedBalloon();

    	selfDestruct();
    }


    /*--- Public Methods ---*/

    public void selfDestruct() {

    	// Self-Destruct on Collision
    	Destroy(gameObject, .1f);

    	// Create Large Explosion Effect
    	ParticleSystem explosion = Instantiate(explosionLarge, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);

    	// Create Small Explosion Effect
    	explosion = Instantiate(explosionSmall, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);
    }

}
