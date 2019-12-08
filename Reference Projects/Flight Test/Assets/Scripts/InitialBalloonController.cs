using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialBalloonController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Variables
    public static InitialBalloonController instance;
	public ParticleSystem explosionLarge;
	public ParticleSystem explosionSmall;
	public float temporalOffset = 0f;

	// Private Variables
	private float currentTime = 0f;
    private float restoreTime = 0f;
	private float rotationCycleLength = 20f;
	private float driftCycleLength = 10f;
	private float driftDistance = 3f;
	private Vector3 initialPosition;
	private Vector3 initialRotation;
    private bool hidden = false;


	/*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;
        
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

        // Update Visibility State
        if (hidden && currentTime > restoreTime) {
            hidden = false;
        }

        // Update Visibility
        if (hidden) {
            disableComponents();
        } else {
            enableComponents();
        }

    }

    void OnTriggerEnter(Collider other) {

    	// Start Game
    	BalloonMinigameController.instance.startGame();

        // Hide Balloon
    	hideBalloon();
    }


    /*--- Public Methods ---*/

    public void hideBalloon() {

    	// Hide on Collision
    	hidden = true;
        restoreTime = currentTime + BalloonMinigameController.instance.gameLength;

    	// Create Large Explosion Effect
    	ParticleSystem explosion = Instantiate(explosionLarge, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);

    	// Create Small Explosion Effect
    	explosion = Instantiate(explosionSmall, transform.position, transform.rotation);
    	Destroy(explosion, 1.5f);
    }

    public void reset() {
        hidden = false;
    }


    /*--- Utility Methods ---*/

    private void disableComponents() {

        // Disable Rendering
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        renderer.enabled = false;

        // Disable Collisions
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    private void enableComponents() {

        // Enable Rendering
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = true;
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        renderer.enabled = true;

        // Enable Collisions
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }

}
