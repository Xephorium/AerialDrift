﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Windows-Specific Cursor Hackery
using System.Runtime.InteropServices;

public class UIController : MonoBehaviour {

    // Windows-Specific Cursor Hackery
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);


	/*--- Variable Declarations ---*/

	// Public Constants
	public static UIController instance;

	// Public Variables
	public CanvasGroup menuCanvasGroup;
	public CanvasGroup selectCanvasGroup;
	public CanvasGroup flightCanvasGroup;


	/*--- Lifecycle Methods ---*/

    void Start() {
        instance = this;
    }


    /*--- Public Methods ---*/

    public void showMenuUI() {
    	menuCanvasGroup.alpha = 1f;
    	menuCanvasGroup.blocksRaycasts = true;
    	hideSelectUI();
    	hideFlightUI();
    }

    private void hideMenuUI() {
    	menuCanvasGroup.alpha = 0f;
    	menuCanvasGroup.blocksRaycasts = false;
    }

    public void showSelectUI() {
    	selectCanvasGroup.alpha = 1f;
    	selectCanvasGroup.blocksRaycasts = true;
    	hideMenuUI();
    	hideFlightUI();
    }

    private void hideSelectUI() {
    	selectCanvasGroup.alpha = 0f;
    	selectCanvasGroup.blocksRaycasts = false;
    }


    public void showFlightUI() {
    	flightCanvasGroup.alpha = 1f;
    	flightCanvasGroup.blocksRaycasts = true;
    	hideMenuUI();
    	hideSelectUI();
    }

    private void hideFlightUI() {
    	flightCanvasGroup.alpha = 0f;
    	flightCanvasGroup.blocksRaycasts = false;
    }

    // Windows-Specific Cursor Hackery
    public void centerCursor() {   
        SetCursorPos(Screen.width / 2 - 30,Screen.height / 2 - 30);
    }
}
