using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceMinigameController : MonoBehaviour {


    /*--- Variable Declarations ---*/

	// Public Constants
    public static RaceMinigameController instance;
    public float gameLength = 40f;

    // Public Variables
    public Text ringCount;
    public Image ringImage;
    public Text raceTitle;
    public Text raceTimer;
    public GameObject ringContainer;
    public GameObject finalRing;
    public GameObject victoryFireworkContainer1;
    public GameObject victoryFireworkContainer2;
    public GameObject victoryFireworkContainer3;
    public GameObject victoryFireworkContainer4;
    public GameObject victoryFireworkContainer5;
    public AudioSource jukeboxVictoryFirework;
    public bool playingGame = false;

    // Private Constants
    private float RING_COUNT = 18f;

    // Private Variables
    private float currentTime = 0f;
    private float gameStartTime = 0f;
    private float checkpointsPassed = 0f;
    private float maxCheckpointsPassed = 0f;


    /*--- Lifecycle Methods ---*/

    void Start() {
        
        // Set Instance
        instance = this;

    }

    void Update() {

    	// Update Time
    	currentTime += Time.deltaTime;

    	// Update Game State
    	if (playingGame && currentTime > gameStartTime + gameLength) {
    		endGame();
    	}

    	// Update UI
    	if (playingGame) {
    		ringCount.text = checkpointsPassed.ToString();
    		raceTimer.text = ((int) (gameStartTime + gameLength - currentTime + 1)).ToString();
    	} else {
    		ringCount.text = maxCheckpointsPassed.ToString();
    	}

        // Show Final Ring
        if (playingGame && checkpointsPassed > RING_COUNT - 3) {
            var script = finalRing.GetComponent(typeof(RingController)) as RingController;
            script.showRing();
        }
    }


    /*--- Public Methods ---*/

    public void startGame() {

    	// Update State
    	playingGame = true;
    	gameStartTime = currentTime;

    	// Update UI
    	ringCount.color = new Color(.4f, .95f, 0.4f);
    	ringImage.color = new Color(.4f, .95f, 0.4f);
    	raceTitle.enabled = true;
    	raceTimer.enabled = true;

        // Update Audio
        AudioController.instance.setBattleVolumeNormal();
        AudioController.instance.playGameWhistle();

    	// Show Rings
        showRings();

    }

    public void endGame() {

    	// Update State
    	playingGame = false;
    	checkpointsPassed = 0f;

        // Update Audio
        if (AudioController.instance != null) {
            AudioController.instance.playGameWhistle();
            AudioController.instance.setBattleVolumeSilent();
        }

    	// Update UI
        if (maxCheckpointsPassed < RING_COUNT) {
    	    ringCount.color = new Color(1f, 1f, 1f);
    	    ringImage.color = new Color(1f, 1f, 1f);
        } else {
            ringCount.color = new Color(.95f, .95f, 0.4f);
            ringImage.color = new Color(.95f, .95f, 0.4f);
        }
    	raceTitle.enabled = false;
    	raceTimer.enabled = false;

    	// Hide Rings
    	hideRings();

    }

    public void recordCheckpointPassed() {

    	// Update Balloon Count
    	if (playingGame) {

            // Record Checkpoint
    		checkpointsPassed++;
    		if (checkpointsPassed > maxCheckpointsPassed) {
    			maxCheckpointsPassed++;
    		}

            // Handle Final Ring
            if (maxCheckpointsPassed >= RING_COUNT) {
                celebrate();
                reset();
            }
    	}
    }

    public void reset() {
    	endGame();

        // Reset Initial Ring
        if (InitialRingController.instance != null) {
    	   InitialRingController.instance.reset();
        }
    }


    /*--- Utility Methods ---*/

    private void showRings() {

        // Show Intermediate Rings
        for (int x = 0; x < RING_COUNT - 1; x++) {
            var script = ringContainer.transform.GetChild(x).GetComponent(typeof(RingController)) as RingController;
            script.showRing();
        }
    }

    private void hideRings() {

        // Hide Intermediate Rings
        for (int x = 0; x < RING_COUNT - 1; x++) {
            var script = ringContainer.transform.GetChild(x).GetComponent(typeof(RingController)) as RingController;
            script.hideRing();
        }

        // Hide Final Ring
        var finalRingScript = finalRing.GetComponent(typeof(RingController)) as RingController;
        finalRingScript.hideRing();
    }

    private void celebrate() {

        // Play All Particle Systems
        for(int x = 0; x < 5; x++) {
            var particleSystem = victoryFireworkContainer1.transform.GetChild(x).GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        for(int x = 0; x < 5; x++) {
            var particleSystem = victoryFireworkContainer2.transform.GetChild(x).GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        for(int x = 0; x < 5; x++) {
            var particleSystem = victoryFireworkContainer3.transform.GetChild(x).GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        for(int x = 0; x < 5; x++) {
            var particleSystem = victoryFireworkContainer4.transform.GetChild(x).GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        for(int x = 0; x < 5; x++) {
            var particleSystem = victoryFireworkContainer5.transform.GetChild(x).GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        // Play Firework Audio
        jukeboxVictoryFirework.Play();
    }

}
