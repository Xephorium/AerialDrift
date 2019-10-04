using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRotator : MonoBehaviour {


	/*--- Variable Declarations ---*/


	/*--- Lifecycle Methods ---*/

    void Start() {
        
    }

    void Update() {
        transform.Rotate(new Vector3(0, 7, 0) * Time.deltaTime);
    }
}
