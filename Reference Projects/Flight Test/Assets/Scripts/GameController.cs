using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Data Structures
	public enum GameState {
        none,
		menu,
		select,
		flightBiplane,
		flightHelicopter,
		flightViper,
        pauseBiplane,
        pauseHelicopter,
        pauseViper
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
    private GameState prePauseState = GameState.none;


	/*--- Lifecycle Methods ---*/

    void Start() {
        gameState = GameState.menu;
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
            if (!animatingTransition && fadeInQueued && prePauseState == GameState.none) {
                BiplaneController.instance.reset();
                HelicopterController.instance.reset();
                ViperController.instance.reset();
                handleQueuedTransition();
            } else if (!animatingTransition && fadeInQueued && prePauseState != GameState.none) {
                handleQueuedTransition();
                prePauseState = GameState.none;
            }

    		// Handle Back Click
            if (!animatingTransition) {
        		if (Input.GetKeyDown("escape")) {
                    prePauseState = gameState;
                    pauseSumulation();

                    // Determine Pause State
                    GameState pauseState = GameState.pauseBiplane;
                    if (gameState == GameState.flightBiplane) {
                        pauseState = GameState.pauseBiplane;
                    }
                    if (gameState == GameState.flightHelicopter) {
                        pauseState = GameState.pauseHelicopter;
                    }
                    if (gameState == GameState.flightViper) {
                        pauseState = GameState.pauseViper;
                    }

                    // Set Pause State
        			updateGameState(pauseState);
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

            // Reset Pause Flag
            if (prePauseState != GameState.none) {
                prePauseState = GameState.none;
            }

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
                    BiplaneController.instance.hideHighlight();
                    HelicopterController.instance.hideHighlight();
                    ViperController.instance.hideHighlight();
        			updateGameState(GameState.menu);
                    beginTransition();
        		}
            }

            // Handle Aircraft Hover
            if (!animatingTransition) {
                int mousePosition = (int) (Input.mousePosition[0] / (Screen.width / 3));
                if (mousePosition == 0) {
                    BiplaneController.instance.showHighlight();
                    HelicopterController.instance.hideHighlight();
                    ViperController.instance.hideHighlight();
                } else if (mousePosition == 1) {
                    BiplaneController.instance.hideHighlight();
                    HelicopterController.instance.showHighlight();
                    ViperController.instance.hideHighlight();
                } else {
                    BiplaneController.instance.hideHighlight();
                    HelicopterController.instance.hideHighlight();
                    ViperController.instance.showHighlight();
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
    	

        // Pause Logic
        } else if (
                gameState == GameState.pauseBiplane
                || gameState == GameState.pauseHelicopter
                || gameState == GameState.pauseViper
            ) {

            // Handle Queued Transition
            if (!animatingTransition && fadeInQueued) {
                handleQueuedTransition();
            }

            // Handle Back Click
            if (!animatingTransition) {
                if (Input.GetKeyDown("escape")) {
                    resumeSumulation();
                    updateGameState(GameState.select);
                    beginTransition();
                } else if (Input.anyKey && !Input.GetKey("escape")) {
                    updateGameState(prePauseState);
                    resumeSumulation();
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
    	} else if (gameState == GameState.flightBiplane
                || gameState == GameState.flightHelicopter
                || gameState == GameState.flightViper) {
    		UIController.instance.showFlightUI();
            UIController.instance.centerCursor();
    	} else if (gameState == GameState.pauseBiplane
                || gameState == GameState.pauseHelicopter
                || gameState == GameState.pauseViper) {
            UIController.instance.showPauseUI();
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
    	} else if (gameState == GameState.flightViper || gameState == GameState.pauseViper) {
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
    	} else if (gameState == GameState.flightViper) {
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

    private void pauseSumulation() {
        Time.timeScale = 0f;
    }

    private void resumeSumulation() {
        Time.timeScale = 1f;
    }
}
