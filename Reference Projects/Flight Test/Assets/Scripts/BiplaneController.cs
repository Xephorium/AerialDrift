using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneController : MonoBehaviour
{

    /*--- Variable Declarations ---*/

    // Public Constants
    public static BiplaneController instance;

    // Public Variables
    public GameObject cameraEmpty;
    public Camera camera;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;

    // Private Variables
    private Rigidbody myRigidBody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Boost Constants
    // private static float DEFAULT_FOV = 70f;
    // private static float BOOST_FOV = 80f;
    // private static float boostAnimationTime = .4f;

    // Boost Variables
    // private float boostTimeStart = 0f;
    // private float boostTimeEnd = 0f;
    // private float boostFOVStart = DEFAULT_FOV;
    // private float boostFOVEnd = DEFAULT_FOV;
    // private float boostAnimationFactor = 0f;
    // private bool boostChange = false;
    // private bool boostAnimating = false;
    // private bool boost = false;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        myRigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (isPlayerControlling) {


            /*--- Detect Keypresses ---*/

            // if (Input.GetKey(KeyCode.LeftShift) != boost
            //     && (Input.GetAxis("ControlForwardBack") > 0
            //         || !Input.GetKey(KeyCode.LeftShift))) {
            //     boostChange = true;
            // } else {
            //     boostChange = false;
            // }
            // boost = Input.GetKey(KeyCode.LeftShift);
      

            /*--- Update Ship Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");

            // Create Individual Force Vectors
            Vector3 forceForwardBack = transform.forward * 10 * moveForwardBack;

            // // Apply Boost
            // if (moveForwardBack > 0 && boost) {
            //     forceForwardBack *= 2;
            // }

            // Combine Force Vectors
            Vector3 combinedForce = forceForwardBack;

            // Apply Force to Rigidbody Component
            myRigidBody.AddForce(combinedForce);
            //myRigidBody.velocity = combinedForce;


            /*--- Update Ship Rotation ---*/

            // Calculate Roll
            float rotateRoll = Input.GetAxis("ControlRoll");

            // Calculate Pitch & Yaw
            float mousePercentXRaw = (Screen.width/2 - (Input.mousePosition[0] + 20)) / (Screen.height * .75f);
            float mousePercentYRaw = (Screen.height/2 - (Input.mousePosition[1] - 20)) / (Screen.height * .75f);
            float mousePercentX = Mathf.Sign(mousePercentXRaw) * Mathf.Pow(mousePercentXRaw, 2);
            float mousePercentY = Mathf.Sign(mousePercentYRaw) * Mathf.Pow(mousePercentYRaw, 2);

            // Apply Transformations
            float shipPitch = mousePercentY * 5;
            float shipYaw = -mousePercentX * 5;
            float shipRoll = -rotateRoll;

            // Apply Rotation
            transform.Rotate(
                shipPitch,
                shipYaw,
                shipRoll,
                Space.Self
            );


            /*--- Update Camera ---*/

            // Calculate Camera Shift
            float cameraShiftX = -mousePercentY * 3;
            float cameraShiftY = mousePercentX * 3;
            float cameraShiftZ = 0;

            // Apply Camera Rotation
            cameraEmpty.transform.eulerAngles = new Vector3(
                transform.eulerAngles.x + cameraShiftX,
                transform.eulerAngles.y + cameraShiftY,
                transform.eulerAngles.z + cameraShiftZ
            );

            // Apply Camera Movement
            cameraEmpty.transform.localPosition = new Vector3(
                -mousePercentX * (CAMERA_MOVEMENT_FACTOR / 2),
                -mousePercentY * CAMERA_MOVEMENT_FACTOR,
                0
            );


            /*--- Update Boost Variables ---*/

            // // Set Boost Variables on Change
            // if (boostChange) {
            //     boostTimeStart = Time.fixedTime;
            //     boostTimeEnd = boostTimeStart + boostAnimationTime;
            //     boostFOVStart = camera.fieldOfView;
            //     boostAnimating = true;
            //     if (boost) {
            //         boostFOVEnd = BOOST_FOV;
            //     } else {
            //         boostFOVEnd = DEFAULT_FOV;
            //     }
            //     trailLeft.emitting = boost;
            //     trailRight.emitting = boost;
            // }

            // // Note FOV Animation Completion
            // if (Time.fixedTime >= boostTimeEnd) {
            //     boostAnimating = false;
            // }
        }
    }

    void Update() {
        if (isPlayerControlling) {

        
            /*--- Update FOV ---*/

            // // If FOV Animation Incomplete
            // if (boostAnimating) {
                
            //     // Calculate & Set FOV
            //     boostAnimationFactor = 1 - Mathf.Abs((boostTimeEnd - Time.fixedTime)/(boostTimeEnd - boostTimeStart));
            //     float normalizedInput = boostAnimationFactor * (Mathf.PI / 3);
            //     float fovFactor = .5f * (1 + Mathf.Sin(3 * normalizedInput - Mathf.PI / 2));
            //     float currentFOV = boostFOVStart + (fovFactor * (boostFOVEnd - boostFOVStart));
            //     camera.fieldOfView = currentFOV;
            // }
        }
    }


    /*--- Public Methods ---*/

    public void reset() {

        // // Reset Boost Effects
        // boost = false;
        // boostAnimating = false;
        // camera.fieldOfView = DEFAULT_FOV;

        // Reset Position, Rotation, Velocity
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        myRigidBody.velocity = new Vector3(0, 0, 0);
    }
}
