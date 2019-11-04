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
    private static float MAX_BANK_ANGLE = 7f;
    private static float MAX_CRAFT_CHANGE_ROT = .2f;

    // Private Variables
    private Rigidbody hcRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float craftRotationX = 0f;
    private float craftRotationY = 0f;
    private float craftRotationZ = 0f;
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
        craftRotationX = initialRotation.x;
        craftRotationY = initialRotation.y;
        craftRotationZ = initialRotation.z;

        // Get Components
        hcRigidbody = GetComponent<Rigidbody>();

        // Move Center of Mass Forward
        hcRigidbody.centerOfMass = new Vector3(0, 0, 2);
    }

    void FixedUpdate() {
        if (isPlayerControlling) {


        	/*--- Determine Unbanked Vectors ---*/

        	Vector3 forward = hcRigidbody.transform.forward +
        		Vector3.Reflect(hcRigidbody.transform.forward, Vector3.up);
        	forward = forward / 2f;
        	Vector3 right = hcRigidbody.transform.right +
        		Vector3.Reflect(hcRigidbody.transform.right, Vector3.up);
        	right = right / 2f;
        	Vector3 up = hcRigidbody.transform.up +
        		Vector3.Reflect(hcRigidbody.transform.up, Vector3.right);
        	up = up / 2f;


            /*--- Update Ship Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");
            float moveUpDown = Input.GetAxis("ControlUpDown");
            float moveRightLeft = Input.GetAxis("ControlRoll");

            // Create Individual Force Vectors
            Vector3 forceForwardBack = forward * 10 * moveForwardBack;
            Vector3 forceUpDown = up * 5 * moveUpDown;
            Vector3 forceRightLeft = right * 10 * moveRightLeft;

            // Combine Force Vectors
            Vector3 combinedForce = forceForwardBack + forceUpDown + forceRightLeft;

            // Apply Force to Rigidbody Component
            hcRigidbody.AddForce(combinedForce);
            //hcRigidbody.velocity = combinedForce;


            /*--- Update Ship Rotation ---*/

            // Calculate Pitch & Yaw Inputs
            float mousePercentXRaw = (Screen.width/2 - (Input.mousePosition[0] + 20)) / (Screen.height * .75f);
            float mousePercentYRaw = (Screen.height/2 - (Input.mousePosition[1] - 20)) / (Screen.height * .75f);
            float mousePercentX = Mathf.Sign(mousePercentXRaw) * Mathf.Pow(mousePercentXRaw, 2);
            float mousePercentY = Mathf.Sign(mousePercentYRaw) * Mathf.Pow(mousePercentYRaw, 2);

            // Calculate Player Rotation
            float shipPitch = 0;
            float shipYaw = -mousePercentX * 5;
            float shipRoll = 0;

            // Setup Bank Variables
            float forwardSpeed = Mathf.Clamp(Vector3.Dot(hcRigidbody.velocity, forward) / MAX_SPEED, -1, 1);
            float rightSpeed = Mathf.Clamp(Vector3.Dot(hcRigidbody.velocity, right) / MAX_SPEED, -1, 1);
            float bankRoll = moveRightLeft * MAX_BANK_ANGLE;
            float bankYaw = moveForwardBack * MAX_BANK_ANGLE;

            // Calculate X Rotation (Bank)
            float targetCraftRotationX = bankYaw;
            if (targetCraftRotationX < craftRotationX) {
                craftRotationX = Mathf.Clamp(craftRotationX - MAX_CRAFT_CHANGE_ROT, targetCraftRotationX, 100);
            } else if (targetCraftRotationX > craftRotationX) {
                craftRotationX = Mathf.Clamp(craftRotationX + MAX_CRAFT_CHANGE_ROT, -100, targetCraftRotationX);
            }

            // Calculate Y Rotation (Turn)
            float targetCraftRotationY = craftRotationY + shipYaw;
            craftRotationY = targetCraftRotationY;

            // Calculate Z Rotation (Bank)
            float targetCraftRotationZ = -bankRoll;
            if (targetCraftRotationZ < craftRotationZ) {
                craftRotationZ = Mathf.Clamp(craftRotationZ - MAX_CRAFT_CHANGE_ROT, targetCraftRotationZ, 100);
            } else if (targetCraftRotationZ > craftRotationZ) {
                craftRotationZ = Mathf.Clamp(craftRotationZ + MAX_CRAFT_CHANGE_ROT, -100, targetCraftRotationZ);
            }

            // Apply Rotation
            transform.rotation = Quaternion.Euler(
                craftRotationX,
             	craftRotationY,
             	craftRotationZ
            );


            /*--- Update Camera ---*/

            // Calculate Speed
            forwardSpeed = Mathf.Clamp01((Vector3.Dot(hcRigidbody.velocity, forward) / MAX_SPEED));
            float speedFactor = Mathf.Clamp01(hcRigidbody.velocity.magnitude / MAX_SPEED) * forwardSpeed;

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

            // Apply Camera Rotation
            cameraEmpty.transform.eulerAngles = new Vector3(
                0,
                transform.eulerAngles.y,
                0
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
