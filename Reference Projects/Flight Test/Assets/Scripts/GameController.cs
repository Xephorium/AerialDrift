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


	/*--- Lifecycle Methods ---*/

    void Start() {
        gameStateChanged = true;
    }

    void Update() {

    	// Handle Flight Keypresses (First Condition for Optimization)
    	if (gameState == GameState.flightBiplane
    		|| gameState == GameState.flightHelicopter
    		|| gameState == GameState.flightViper) {

    		// Back Click
    		if (Input.GetKeyDown("escape")) {
    			updateGameState(GameState.select);
    			// TODO - Reset Biplane
    			// TODO - Reset Helicopter
    			ViperController.instance.reset();
    		}

    	// Handle Menu Keypresses
    	} else if (gameState == GameState.menu) {
    		if (Input.GetKeyDown("escape")) {
    			Application.Quit();
    		} else if (Input.anyKey && !Input.GetKey("escape")) {
    			updateGameState(GameState.select);
    		}

    	// Handle Select Keypress
    	} else if (gameState == GameState.select) {

    		// Back Click
    		if (Input.GetKeyDown("escape")) {
    			updateGameState(GameState.menu);
    		}

    		// Aircraft Click
    		if (Input.GetMouseButtonUp(0)) {
    			int mousePosition = (int) (Input.mousePosition[0] / (Screen.width / 3));
    			
    			if (mousePosition == 0) {
    				// TODO - Select Biplane
    			} else if (mousePosition == 1) {
    				// TODO - Select Helicopter
    			} else {
    				ViperController.instance.reset();
    				updateGameState(GameState.flightViper);
    			}
    		}
    	}

    	// Handle Game State Change
    	if (gameStateChanged) {
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
    		// TODO - Enable Biplane Camera
    	} else if (gameState == GameState.flightHelicopter) {
    		// TODO - Enable Helicopter Camera
    	} else {
    		CameraController.instance.showViperCamera();
    	}
    }

    private void updateShipControls() {
    	if (gameState == GameState.menu
    		|| gameState == GameState.select) {
    		// TODO - Set Biplane Controls Inactive
    		// TODO - Set Helicopter Controls Inactive
    		ViperController.instance.isPlayerControlling = false;
    	} else if (gameState == GameState.flightBiplane) {
    		// TODO - Set Biplane Controls Active
    		// TODO - Set Helicopter Controls Inactive
    		ViperController.instance.isPlayerControlling = false;
    	} else if (gameState == GameState.flightHelicopter) {
    		// TODO - Set Biplane Controls Inactive
    		// TODO - Set Helicopter Controls Active
    		ViperController.instance.isPlayerControlling = false;
    	} else {
    		// TODO - Set Biplane Controls Inactive
    		// TODO - Set Helicopter Controls Inactive
    		ViperController.instance.isPlayerControlling = true;
    	}
    }
}
