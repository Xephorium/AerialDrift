using System.Collections;
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
    public float fadeTime = .1f;
    public Texture2D textureWhite;
    public Texture2D textureGrey;
	public CanvasGroup menuCanvasGroup;
	public CanvasGroup selectCanvasGroup;
	public CanvasGroup flightCanvasGroup;
	public CanvasGroup pauseCanvasGroup;
    public CanvasGroup biplaneControlCanvasGroup;
    public CanvasGroup helicopterControlCanvasGroup;
    public CanvasGroup viperControlCanvasGroup;

    // Private Variables
    private Texture2D fadeTexture;
    private float time = 0f;
    private bool fading = false;
    private bool fadeIn = true;
    private float fadeStartTime = 0f;
    private float fadeEndTime = 0f;


	/*--- Lifecycle Methods ---*/

    void Start() {
        instance = this;
    }

    void Update() {
        time += Time.deltaTime;
        if (fading
            && ((fadeEndTime - time) / (fadeEndTime - fadeStartTime) > 1.0f
            || (fadeEndTime - time) / (fadeEndTime - fadeStartTime) < 0.0f)) {
            fading = false;
        }
    }

    void OnGUI() {
        if (fading) {

            // Calculate Fade Progress
            float fadeProgress = (fadeEndTime - time) / (fadeEndTime - fadeStartTime);

            // Match Fade Rate to Sin Curve (More visually pleasing transition, but slower.)
            //float normalizedInput = fadeProgress * (Mathf.PI / 3);
            //fadeProgress = .5f * (1 + Mathf.Sin(3 * normalizedInput - Mathf.PI / 2));

            // Calculate Alpha Value
            float currentAlpha = 1 - fadeProgress;
            if (fadeIn) {
                currentAlpha = 1 - currentAlpha;
            }

            // Set Color
            Color newColor = GUI.color;
            newColor.a = currentAlpha;
            GUI.color = newColor;
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);

        } else {

            // Set Default Color
            Color newColor = GUI.color;
            newColor.a = fadeIn ? 0f : 1f;
            GUI.color = newColor;
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
        }
    }


    /*--- Public Methods ---*/

    public void showMenuUI() {
    	menuCanvasGroup.alpha = 1f;
    	menuCanvasGroup.blocksRaycasts = true;
    	hideSelectUI();
    	hideFlightUI();
    	hidePauseUI();
    }

    public void showSelectUI() {
    	selectCanvasGroup.alpha = 1f;
    	selectCanvasGroup.blocksRaycasts = true;
    	hideMenuUI();
    	hideFlightUI();
    	hidePauseUI();
    }

    public void showFlightUI() {
    	flightCanvasGroup.alpha = 1f;
    	flightCanvasGroup.blocksRaycasts = true;
    	hideMenuUI();
    	hideSelectUI();
    	hidePauseUI();
    }

    public void showPauseUI() {
    	pauseCanvasGroup.alpha = 1f;
    	pauseCanvasGroup.blocksRaycasts = true;
    	hideMenuUI();
    	hideSelectUI();
    	hideFlightUI();
    }

    public void showBiplaneControlUI() {
        biplaneControlCanvasGroup.alpha = 1f;
        biplaneControlCanvasGroup.blocksRaycasts = true;
        helicopterControlCanvasGroup.alpha = 0f;
        helicopterControlCanvasGroup.blocksRaycasts = false;
        viperControlCanvasGroup.alpha = 0f;
        viperControlCanvasGroup.blocksRaycasts = false;
    }

    public void showHelicopterControlUI() {
        biplaneControlCanvasGroup.alpha = 0f;
        biplaneControlCanvasGroup.blocksRaycasts = false;
        helicopterControlCanvasGroup.alpha = 1f;
        helicopterControlCanvasGroup.blocksRaycasts = true;
        viperControlCanvasGroup.alpha = 0f;
        viperControlCanvasGroup.blocksRaycasts = false;
    }

    public void showViperControlUI() {
        biplaneControlCanvasGroup.alpha = 0f;
        biplaneControlCanvasGroup.blocksRaycasts = false;
        helicopterControlCanvasGroup.alpha = 0f;
        helicopterControlCanvasGroup.blocksRaycasts = false;
        viperControlCanvasGroup.alpha = 1f;
        viperControlCanvasGroup.blocksRaycasts = true;
    }

    public void fadeToWhite() {
        fadeTexture = textureWhite;
        fading = true;
        fadeStartTime = time;
        fadeEndTime = time + fadeTime;
        fadeIn = false;
    }

    public void fadeFromWhite() {
        fadeTexture = textureWhite;
        fading = true;
        fadeStartTime = time;
        fadeEndTime = time + fadeTime;
        fadeIn = true;
    }

    public void fadeToGrey() {
        fadeTexture = textureGrey;
        fading = true;
        fadeStartTime = time;
        fadeEndTime = time + (fadeTime * 2f);
        fadeIn = false;
    }

    public void fadeFromGrey() {
        fadeTexture = textureGrey;
        fading = true;
        fadeStartTime = time;
        fadeEndTime = time + (fadeTime * 2f);
        fadeIn = true;
    }

    // Windows-Specific Cursor Hackery
    public void centerCursor() {   
        SetCursorPos(Screen.width / 2 - 30,Screen.height / 2 - 30);
    }


    /*--- Private Methods ---*/

    private void hideMenuUI() {
        menuCanvasGroup.alpha = 0f;
        menuCanvasGroup.blocksRaycasts = false;
    }

    private void hideSelectUI() {
        selectCanvasGroup.alpha = 0f;
        selectCanvasGroup.blocksRaycasts = false;
    }

    private void hideFlightUI() {
        flightCanvasGroup.alpha = 0f;
        flightCanvasGroup.blocksRaycasts = false;
    }

    private void hidePauseUI() {
    	pauseCanvasGroup.alpha = 0f;
    	pauseCanvasGroup.blocksRaycasts = false;
    }
}
