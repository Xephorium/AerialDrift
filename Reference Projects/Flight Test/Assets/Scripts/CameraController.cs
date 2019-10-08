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
	public Camera viperCamera;


	/*--- Lifecycle Methods ---*/

    void Start() {
        instance = this;
    }


    /*--- Utility Methods ---*/

    public void showMenuCamera() {
    	menuCamera.enabled = true;
    	selectCamera.enabled = false;
        biplaneCamera.enabled = false;
    	viperCamera.enabled = false;
    }

    public void showSelectCamera() {
    	menuCamera.enabled = false;
    	selectCamera.enabled = true;
        biplaneCamera.enabled = false;
    	viperCamera.enabled = false;
    }

    public void showBiplaneCamera() {
        menuCamera.enabled = false;
        selectCamera.enabled = false;
        biplaneCamera.enabled = true;
        viperCamera.enabled = false;
    }

    public void showViperCamera() {
    	menuCamera.enabled = false;
    	selectCamera.enabled = false;
        biplaneCamera.enabled = false;
    	viperCamera.enabled = true;
    }
}
