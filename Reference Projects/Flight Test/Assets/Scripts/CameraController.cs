using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Constants
	public static CameraController instance;

	// Public Variables
	public Camera menuCamera;
	public Camera selectCamera;
    public Camera biplaneCamera;
    public Camera helicopterCamera;
	public Camera viperCamera;


	/*--- Lifecycle Methods ---*/

    void Start() {
        instance = this;
    }


    /*--- Utility Methods ---*/

    public void showMenuCamera() {
    	menuCamera.enabled = true;
        menuCamera.GetComponent<AudioListener>().enabled = true;
    	selectCamera.enabled = false;
        selectCamera.GetComponent<AudioListener>().enabled = false;
        biplaneCamera.enabled = false;
        biplaneCamera.GetComponent<AudioListener>().enabled = false;
        helicopterCamera.enabled = false;
        helicopterCamera.GetComponent<AudioListener>().enabled = false;
    	viperCamera.enabled = false;
        viperCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void showSelectCamera() {
    	menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;
        selectCamera.enabled = true;
        selectCamera.GetComponent<AudioListener>().enabled = true;
        biplaneCamera.enabled = false;
        biplaneCamera.GetComponent<AudioListener>().enabled = false;
        helicopterCamera.enabled = false;
        helicopterCamera.GetComponent<AudioListener>().enabled = false;
        viperCamera.enabled = false;
        viperCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void showBiplaneCamera() {
        menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;
        selectCamera.enabled = false;
        selectCamera.GetComponent<AudioListener>().enabled = false;
        biplaneCamera.enabled = true;
        biplaneCamera.GetComponent<AudioListener>().enabled = true;
        helicopterCamera.enabled = false;
        helicopterCamera.GetComponent<AudioListener>().enabled = false;
        viperCamera.enabled = false;
        viperCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void showHelicopterCamera() {
        menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;
        selectCamera.enabled = false;
        selectCamera.GetComponent<AudioListener>().enabled = false;
        biplaneCamera.enabled = false;
        biplaneCamera.GetComponent<AudioListener>().enabled = false;
        helicopterCamera.enabled = true;
        helicopterCamera.GetComponent<AudioListener>().enabled = true;
        viperCamera.enabled = false;
        viperCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void showViperCamera() {
    	menuCamera.enabled = false;
        menuCamera.GetComponent<AudioListener>().enabled = false;
        selectCamera.enabled = false;
        selectCamera.GetComponent<AudioListener>().enabled = false;
        biplaneCamera.enabled = false;
        biplaneCamera.GetComponent<AudioListener>().enabled = false;
        helicopterCamera.enabled = false;
        helicopterCamera.GetComponent<AudioListener>().enabled = false;
        viperCamera.enabled = true;
        viperCamera.GetComponent<AudioListener>().enabled = true;
    }
}
