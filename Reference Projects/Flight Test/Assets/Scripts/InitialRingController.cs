using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialRingController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Variables
    public static InitialRingController instance;
	public float temporalOffset = 0f;

	// Private Constants
	private float ROTATION_CYCLE_LENGTH = 20f;
	private float SCALE_CYCLE_LENGTH = 2f;
	private float SCALE_FACTOR = .2f;

	// Private Variables
	private float currentTime = 0f;
    private float restoreTime = 0f;
	private Quaternion initialRingRotation;
	private float initialArrowScale;
	private bool hidden = false;


	/*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;
        
        // Get Initial Values
        initialRingRotation = transform.GetChild(0).transform.localRotation;
        initialArrowScale = transform.GetChild(1).transform.localScale.x;
    }

    void Update() {

    	// Update Time
        currentTime += Time.deltaTime;

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

    	// Update Ring Rotation
    	float rotationFactor = (currentTime % ROTATION_CYCLE_LENGTH) / ROTATION_CYCLE_LENGTH;
        transform.GetChild(0).transform.localRotation = Quaternion.Euler(0f, 0f, rotationFactor * 360f);

        // Calculate Arrows Scale
        float factor = currentTime + temporalOffset;
        float normalizedFactor = factor * (Mathf.PI / (1.5f * SCALE_CYCLE_LENGTH));
        float scaleOffsetFactor = .5f * (1 + Mathf.Sin(3f * normalizedFactor - Mathf.PI / 2f));

        // Update Arrows Scale
        transform.GetChild(1).transform.localScale = new Vector3(
        	initialArrowScale + (SCALE_FACTOR * scaleOffsetFactor),
        	initialArrowScale + (SCALE_FACTOR * scaleOffsetFactor),
        	initialArrowScale
        );
    }

    void OnTriggerEnter(Collider other) {
    	if ((other.name == "Viper"
    		|| other.name == "Biplane"
    		|| other.name == "Helicopter")
            && !BalloonMinigameController.instance.playingGame) {

	    	// Start Race Minigame
	    	RaceMinigameController.instance.startGame();

            // Set Respawn Timer
            restoreTime = currentTime + RaceMinigameController.instance.gameLength;
	    	
            // Hide Ring
            hideRing();
    	}
    }


    /*--- Public Methods ---*/

    public void showRing() {
    	hidden = false;
    }

    public void hideRing() {
    	hidden = true;
    }

    public void reset() {
        hidden = false;
    }


    /*--- Utility Methods ---*/

    private void disableComponents() {

        // Disable Ring
        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        renderer.enabled = false;

        // Disable Arrows
        renderer = transform.GetChild(1).GetComponent<Renderer>();
        renderer.enabled = false;

        // Disable Collisions
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    private void enableComponents() {

        // Disable Ring
        Renderer renderer = transform.GetChild(0).GetComponent<Renderer>();
        renderer.enabled = true;

        // Disable Arrows
        renderer = transform.GetChild(1).GetComponent<Renderer>();
        renderer.enabled = true;

        // Disable Collisions
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }

}
