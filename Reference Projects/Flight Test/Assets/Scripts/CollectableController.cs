using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableController: MonoBehaviour {


	/*--- Variable Declarations ---*/

	// Public Constants
    public static CollectableController instance;

    // Public Variables
    public Text seashellCount;
    public Image seashellImage;
    public float destroyedSeashells = 0f;

    // Private Constants
    private float TOTAL_SEASHELLS = 4f;


    /*--- Lifecycle Methods ---*/

    void Start() {
        
        // Set Instance
        instance = this;
    }

    void Update() {

    	// Update UI
    	seashellCount.text = destroyedSeashells.ToString() + "/" + TOTAL_SEASHELLS.ToString();

    	if (destroyedSeashells >= TOTAL_SEASHELLS) {
    		seashellCount.color = new Color(.95f, .95f, 0.4f);
    		seashellImage.color = new Color(.95f, .95f, 0.4f);
    	}

    }
}
