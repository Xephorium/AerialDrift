using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalloonMinigameController : MonoBehaviour {


    /*--- Variable Declarations ---*/

	// Public Constants
    public static BalloonMinigameController instance;
    public float gameLength = 30f;

    // Public Variables
    public Text balloonCount;
    public Image balloonImage;
    public Text balloonTitle;
    public Text balloonTimer;
    public GameObject balloonEmitter;
    public Transform balloonTemplate;
    public bool playingGame = false;

    // Private Constants
    private float BALLOON_COUNT = 40f;
    private float BALLOON_HORIZONTAL_DISTANCE = 110f;
    private float BALLOON_VERTICAL_DISTANCE = 20f;
    private List<Color> COLORS = new List<Color>();

    // Private Variables
    private float currentTime = 0f;
    private float gameStartTime = 0f;
    private float destroyedBalloons = 0f;
    private float maxDestroyedBalloons = 0f;
    private List<Transform> balloonList = new List<Transform>();


    /*--- Lifecycle Methods ---*/

    void Start() {
        
        // Set Instance
        instance = this;

        // Set Colors
        COLORS.Add(new Color(.24f, .77f, .33f)); // Green
        COLORS.Add(new Color(.28f, .34f, 1f));   // Blue
        COLORS.Add(new Color(.91f, .40f, 1f));   // Purple
        COLORS.Add(new Color(.83f, .51f, .12f)); // Orange
        COLORS.Add(new Color(1f, 1f, 1f));       // White
        COLORS.Add(new Color(1f, .30f, .36f));   // Red
        COLORS.Add(new Color(.9f, 0f, .07f));    // Yellow

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
    		balloonCount.text = destroyedBalloons.ToString();
    		balloonTimer.text = ((int) (gameStartTime + gameLength - currentTime + 1)).ToString();
    	} else {
    		balloonCount.text = maxDestroyedBalloons.ToString();
    	}
    }


    /*--- Public Methods ---*/

    public void startGame() {

    	// Update State
    	playingGame = true;
    	gameStartTime = currentTime;

    	// Update UI
    	balloonCount.color = new Color(.4f, .95f, 0.4f);
    	balloonImage.color = new Color(.4f, .95f, 0.4f);
    	balloonTitle.enabled = true;
    	balloonTimer.enabled = true;

        // Update Audio
        AudioController.instance.setBattleVolumeNormal();
        AudioController.instance.playGameWhistle();

    	// Spawn Balloon
        spawnBalloons();

    }

    public void endGame() {

    	// Update State
    	playingGame = false;
    	destroyedBalloons = 0f;

        // Update Audio
        if (AudioController.instance != null) {
            AudioController.instance.playGameWhistle();
            AudioController.instance.setBattleVolumeSilent();
        }

    	// Update UI
    	balloonCount.color = new Color(1f, 1f, 1f);
    	balloonImage.color = new Color(1f, 1f, 1f);
    	balloonTitle.enabled = false;
    	balloonTimer.enabled = false;

    	// Destroy Remaining Balloons
    	for (int x = 0; x < balloonList.Count; x++) {
    		if (balloonList[x] != null) {
	    		var script = balloonList[x].GetComponent(typeof(SmallBalloonController)) as SmallBalloonController;
	    		script.selfDestruct();
	    	}
    	}

    	// Clear Balloon List
	    balloonList.Clear();

    }

    public void recordDestroyedBalloon() {

    	// Update Balloon Count
    	if (playingGame) {
    		destroyedBalloons++;
    		if (destroyedBalloons > maxDestroyedBalloons) {
    			maxDestroyedBalloons++;
    		}
    	}
    }

    public void reset() {
    	endGame();

        if (InitialBalloonController.instance != null) {
    	   InitialBalloonController.instance.reset();
        }
    }


    /*--- Utility Methods ---*/

    private void spawnBalloons() {

    	for (int x = 0; x < BALLOON_COUNT; x++) {

    		// Determine Balloon Position
    		//float balloonPositionX = balloonEmitter.transform.position.x + (x * 5f);
    		float posX = Random.Range(balloonEmitter.transform.position.x - BALLOON_HORIZONTAL_DISTANCE,
    			balloonEmitter.transform.position.x + BALLOON_HORIZONTAL_DISTANCE);
    		float posY = Random.Range(balloonEmitter.transform.position.y - BALLOON_VERTICAL_DISTANCE,
    			balloonEmitter.transform.position.y + BALLOON_VERTICAL_DISTANCE);
    		float posZ = Random.Range(balloonEmitter.transform.position.z - BALLOON_HORIZONTAL_DISTANCE,
    			balloonEmitter.transform.position.z + BALLOON_HORIZONTAL_DISTANCE);

	    	// Create New Balloon
	    	Transform newBalloon = Instantiate(
	    		balloonTemplate, 
	    		new Vector3(
	    			posX,
	    			posY,
	    			posZ
	    		), 
	    		transform.rotation
	    	);

	    	// Set Temporal Offset
	    	var script = newBalloon.GetComponent(typeof(SmallBalloonController)) as SmallBalloonController;
	    	script.temporalOffset = Random.Range(0, script.driftCycleLength);

	    	// Set Balloon Color
	    	newBalloon.GetComponent<Renderer>().material.SetColor("_Color", getRandomColor());

	    	// Add Balloon to List
	    	balloonList.Add(newBalloon);

    	}
    }

    private Color getRandomColor() {
		int index = Random.Range(0, COLORS.Count - 1);
		return COLORS[index];
    }
}
