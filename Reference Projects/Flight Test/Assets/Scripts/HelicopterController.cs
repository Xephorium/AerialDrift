using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{

    /*--- Variable Declarations ---*/

    // Public Constants
    public static HelicopterController instance;

    // Public Variables
    public GameObject cameraEmpty;
    public Camera camera;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;
    private static float CAMERA_DEPTH_FACTOR = .6f;
    private static float MAX_SPEED = 20f;
    private static float MAX_CAMERA_CHANGE_POS = .01f;
    private static float MAX_CAMERA_CHANGE_ROT = .01f;

    // Private Variables
    private Rigidbody hcRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float cameraPositionX = 0f;
    private float cameraPositionY = 0f;
    private float cameraPositionZ = 0f;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private float cameraRotationZ = 0f;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        hcRigidbody = GetComponent<Rigidbody>();

        // Move Center of Mass Forward
        hcRigidbody.centerOfMass = new Vector3(0, 0, 2);
    }

    void FixedUpdate() {
        if (isPlayerControlling) {
      

            /*--- Update Ship Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");
            float moveUpDown = Input.GetAxis("ControlUpDown");

            // Create Individual Force Vectors
            Vector3 forceForwardBack = transform.forward * 10 * moveForwardBack;
            Vector3 forceUpDown = transform.up * 10 * moveUpDown;

            // Combine Force Vectors
            Vector3 combinedForce = forceForwardBack + forceUpDown;

            // Apply Force to Rigidbody Component
            hcRigidbody.AddForce(combinedForce);
            //hcRigidbody.velocity = combinedForce;


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

            // Calculate Speed
            float speedFactor = Mathf.Clamp01(hcRigidbody.velocity.magnitude / MAX_SPEED);

            // Calculate Camera X Position
            float targetPositionX = -mousePercentX * (CAMERA_MOVEMENT_FACTOR / 2) * speedFactor;
            if (targetPositionX < cameraPositionX) {
                cameraPositionX = Mathf.Clamp(cameraPositionX - MAX_CAMERA_CHANGE_POS, targetPositionX, 5);
            } else if (targetPositionX > cameraPositionX) {
                cameraPositionX = Mathf.Clamp(cameraPositionX + MAX_CAMERA_CHANGE_POS, -5, targetPositionX);
            }

            // Calculate Camera Y Position
            float targetPositionY = -mousePercentY * CAMERA_MOVEMENT_FACTOR * speedFactor;
            if (targetPositionY < cameraPositionY) {
                cameraPositionY = Mathf.Clamp(cameraPositionY - MAX_CAMERA_CHANGE_POS, targetPositionY, 5);
            } else if (targetPositionY > cameraPositionY) {
                cameraPositionY = Mathf.Clamp(cameraPositionY + MAX_CAMERA_CHANGE_POS, -5, targetPositionY);
            }

            // Calculate Camera Z Position
            float targetPositionZ = -(speedFactor * CAMERA_DEPTH_FACTOR);
            if (targetPositionZ < cameraPositionZ) {
                cameraPositionZ = Mathf.Clamp(cameraPositionZ - MAX_CAMERA_CHANGE_POS, targetPositionZ, 0);
            } else if (targetPositionZ > cameraPositionZ) {
                cameraPositionZ = Mathf.Clamp(cameraPositionZ + MAX_CAMERA_CHANGE_POS, -1, targetPositionZ);
            }

            // Apply Camera Movement
            cameraEmpty.transform.localPosition = new Vector3(
                cameraPositionX,
                cameraPositionY,
                cameraPositionZ
            );

            // Calculate Camera X Rotation
            float targetRotationX = -mousePercentY * 1.5f * speedFactor;
            if (targetRotationX < cameraRotationX) {
                cameraRotationX = Mathf.Clamp(cameraRotationX - MAX_CAMERA_CHANGE_ROT, targetRotationX, 180);
            } else if (targetRotationX > cameraRotationX) {
                cameraRotationX = Mathf.Clamp(cameraRotationX + MAX_CAMERA_CHANGE_ROT, -180, targetRotationX);
            }

            // Calculate Camera Y Rotation
            float targetRotationY = mousePercentX * 3f * speedFactor;
            if (targetRotationY < cameraRotationY) {
                cameraRotationY = Mathf.Clamp(cameraRotationY - MAX_CAMERA_CHANGE_ROT, targetRotationY, 180);
            } else if (targetRotationY > cameraRotationY) {
                cameraRotationY = Mathf.Clamp(cameraRotationY + MAX_CAMERA_CHANGE_ROT, -180, targetRotationY);
            }

            // Apply Camera Rotation
            cameraEmpty.transform.localRotation = Quaternion.Euler(
                cameraRotationX,
                cameraRotationY,
                cameraRotationZ
            );


        } else {

            // Apply Default Gravity
            Vector3 forceGravity = new Vector3(0, -9.81f, 0);
            hcRigidbody.AddForce(forceGravity);
        }
    }


    /*--- Public Methods ---*/

    public void reset() {

        // Reset Position, Rotation, Velocity
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        hcRigidbody.velocity = new Vector3(0, 0, 0);
    }
}
