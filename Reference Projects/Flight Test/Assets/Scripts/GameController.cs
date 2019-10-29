using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Data Structures
	public enum GameState {
		menu,
		select,
		flightBiplane,
		flightHelicopter,
		flightViper
	};

	// Public Variables
	public GameState gameState = GameState.menu;

	// Private Variables
	private bool gameStateChanged;
    private bool initialTransitionInitiated = false;
    private bool fadeInQueued = false;
    private bool animatingTransition = true;
    private float time = 0f;
    private float transitionEnd = 0f;


	/*--- Lifecycle Methods ---*/

    void Start() {
        gameStateChanged = true;
    }

    void Update() {

        // Update Realtime Variables
        time += Time.deltaTime;
        animatingTransition = time < transitionEnd;

    	// Flight Logic (First Condition for Optimization)
    	if (gameState == GameState.flightBiplane
    		|| gameState == GameState.flightHelicopter
    		|| gameState == GameState.flightViper) {

            // Handle Queued Transition
            if (!animatingTransition && fadeInQueued) {
                BiplaneController.instance.reset();
                HelicopterController.instance.reset();
                ViperController.instance.reset();
                handleQueuedTransition();
            }

    		// Handle Back Click
            if (!animatingTransition) {
        		if (Input.GetKeyDown("escape")) {
        			updateGameState(GameState.select);
                    beginTransition();
        		}
            }

    	// Menu Logic
    	} else if (gameState == GameState.menu) {

            // Handle Startup Animation
            if (!initialTransitionInitiated) {
                UIController.instance.fadeFromGrey();
                transitionEnd = Time.deltaTime + (UIController.instance.fadeTime * 2);
                initialTransitionInitiated = true;
            }

            // Handle Queued Transition
            if (!animatingTransition && fadeInQueued) {
                handleQueuedTransition();
            }

            // Handle Keypresses
            if (!animatingTransition) {
                if (Input.GetKeyDown("escape")) {
                    Application.Quit();
                } else if (Input.anyKey && !Input.GetKey("escape")) {
                    updateGameState(GameState.select);
                    beginTransition();
                }
            }

    	// Select Logic
    	} else if (gameState == GameState.select) {

            // Handle Queued Transition
            if (!animatingTransition && fadeInQueued) {
                BiplaneController.instance.reset();
                HelicopterController.instance.reset();
                ViperController.instance.reset();
                handleQueuedTransition();
            }

    		// Handle Back Click
            if (!animatingTransition) {
        		if (Input.GetKeyDown("escape")) {
        			updateGameState(GameState.menu);
                    beginTransition();
        		}
            }

    		// Handle Aircraft Click
            if (!animatingTransition) {
        		if (Input.GetMouseButtonDown(0)) {
        			int mousePosition = (int) (Input.mousePosition[0] / (Screen.width / 3));
        			
        			if (mousePosition == 0) {
                        updateGameState(GameState.flightBiplane);
                        beginTransition();
        			} else if (mousePosition == 1) {
        				updateGameState(GameState.flightHelicopter);
                        beginTransition();
        			} else {
        				updateGameState(GameState.flightViper);
                        beginTransition();
        			}
        		}
            }
    	}

    	// Handle Game State Change
    	if (!initialTransitionInitiated || (!animatingTransition && gameStateChanged)) {
        	updateUI();
        	updateCamera();
        	updateShipControls();
        	gameStateChanged = false;
    	}
    }


    /*--- Utility Methods ---*/

    private void updateGameState(GameState state) {
    	gameState = state;
    	gameStateChanged = true;
    }

    private void updateUI() {
    	if (gameState == GameState.menu) {
    		UIController.instance.showMenuUI();
    	} else if (gameState == GameState.select) {
    		UIController.instance.showSelectUI();
    	} else {
    		UIController.instance.showFlightUI();
            UIController.instance.centerCursor();
    	}
    }

    private void updateCamera() {
    	if (gameState == GameState.menu) {
    	    CameraController.instance.showMenuCamera();
    	} else if (gameState == GameState.select) {
    	    CameraController.instance.showSelectCamera();
    	} else if (gameState == GameState.flightBiplane) {
    	    CameraController.instance.showBiplaneCamera();
    	} else if (gameState == GameState.flightHelicopter) {
    	    CameraController.instance.showHelicopterCamera();
    	} else {
    	    CameraController.instance.showViperCamera();
    	}
    }

    private void updateShipControls() {
    	if (gameState == GameState.menu
    		|| gameState == GameState.select) {
    		BiplaneController.instance.isPlayerControlling = false;
    		HelicopterController.instance.isPlayerControlling = false;
    		ViperController.instance.isPlayerControlling = false;
    	} else if (gameState == GameState.flightBiplane) {
    		BiplaneController.instance.isPlayerControlling = true;
    		HelicopterController.instance.isPlayerControlling = false;
    		ViperController.instance.isPlayerControlling = false;
    	} else if (gameState == GameState.flightHelicopter) {
    		BiplaneController.instance.isPlayerControlling = false;
    		HelicopterController.instance.isPlayerControlling = true;
    		ViperController.instance.isPlayerControlling = false;
    	} else {
    		BiplaneController.instance.isPlayerControlling = false;
    		HelicopterController.instance.isPlayerControlling = false;
    		ViperController.instance.isPlayerControlling = true;
    	}
    }

    private void beginTransition() {
        UIController.instance.fadeToWhite();
        transitionEnd = time + UIController.instance.fadeTime;
        animatingTransition = true;
        fadeInQueued = true;
    }

    private void handleQueuedTransition() {
        UIController.instance.fadeFromWhite();
        transitionEnd = time + UIController.instance.fadeTime;
        fadeInQueued = false;
    }

    private void toggleSimulationPause() {
        if (Time.timeScale == 0f) {
            Time.timeScale = 1f;
        } else {
            Time.timeScale = 0f;
        }
    }
}
