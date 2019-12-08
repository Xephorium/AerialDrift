using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeashellController : MonoBehaviour {

    /*--- Variable Declarations ---*/

	// Public Variables
	public ParticleSystem explosionLarge;
	public ParticleSystem explosionSmall;

	// Private Variables
	private float currentTime = 0f;
	private float cycleLength = 5.5f;
	private Vector3 initialRotation;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
        // Get Initial Rotation
        initialRotation = transform.eulerAngles;
    }

    void Update() {

    	// Update Time
        currentTime += Time.deltaTime;

        // Calculate Rotation
    	float rotationFactor = (currentTime % cycleLength) / cycleLength;

    	// Update Rotation
        transform.eulerAngles = new Vector3(
        	0,
        	initialRotation.y + (rotationFactor * 360f),
        	0
        );
    }

    void OnTriggerEnter(Collider other) {

    	// Record Collection
    	CollectableController.instance.destroyedSeashells += 1f;

    	// Self-Destruct on Collision
    	Destroy(gameObject, 0f);

    	// Create Large Explosion Effect
    	ParticleSystem explosion = Instantiate(explosionLarge, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);

    	// Create Small Explosion Effect
    	explosion = Instantiate(explosionSmall, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);
    }
}
