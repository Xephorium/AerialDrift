using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	/*--- Variable Declarations ---*/

	// Private Variables
	private float DESTRUCTION_DELAY = 2f;


	/*--- Lifecycle Methods ---*/

    void Start() {
        
    }

    void Update() {
        
    }

    void OnCollisionEnter(Collision collision) {

    	// Self-Destruct on Collision
    	Destroy(gameObject, DESTRUCTION_DELAY);
    }
}
