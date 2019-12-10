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
    public GameObject propellerBig;
    public GameObject propellerSpinnerBig;
    public GameObject propellerSmall;
    public GameObject propellerSpinnerSmall;
    public GameObject projectileEmitter;
    public Transform projectilePrefab;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;
    private static float CAMERA_DEPTH_FACTOR = 1f;
    private static float MAX_SPEED = 25f;
    private static float MAX_CAMERA_CHANGE_POS = .01f;
    private static float MAX_CAMERA_CHANGE_ROT = .01f;
    private static float MAX_BANK_ANGLE = 7f;
    private static float MAX_CRAFT_CHANGE_ROT = .2f;
    private static float BANK_TRANSITION_TAPERING = .2f;
    private static float THROTTLE_CHANGE_RATE = 0.3f;
    private static float THROTTLE_DEAD_ZONE = 0.7f;
    private static float FIRE_DELAY = .25f;

    // Private Variables
    private Rigidbody hcRigidbody;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float currentThrottle = 0f;
    private float craftRotationX = 0f;
    private float craftRotationY = 0f;
    private float craftRotationZ = 0f;
    private float cameraPositionX = 0f;
    private float cameraPositionY = 0f;
    private float cameraPositionZ = 0f;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private float cameraRotationZ = 0f;
    private float lastFireTime = 0f;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        craftRotationX = initialRotation.eulerAngles.x;
        craftRotationY = initialRotation.eulerAngles.y;
        craftRotationZ = initialRotation.eulerAngles.z;

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


        	/*--- Fire Projectile ---*/

            // Account for Fire Delay
            float fireInput = Input.GetAxis("Fire1");
            bool fire = fireInput == 1f && lastFireTime + FIRE_DELAY < Time.fixedTime;
            
            if (fire) {

            	// Update Last Fire Time
            	lastFireTime = Time.fixedTime;

            	// Create Pojectile
            	Transform projectile = Instantiate(projectilePrefab, projectileEmitter.transform.position, transform.rotation);

            	// Apply Forward Force
            	Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            	projectileRigidbody.AddForce((forward + (up * .06f)) * 7000f);
            }


            /*--- Update Ship Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");
            float moveUpDown = Input.GetAxis("ControlUpDown");
            float moveRightLeft = Input.GetAxis("ControlRoll");

            // Create Individual Force Vectors
            float throttleFactor = (currentThrottle * (1f - THROTTLE_DEAD_ZONE)) + THROTTLE_DEAD_ZONE;
            if (currentThrottle < THROTTLE_DEAD_ZONE) {
            	throttleFactor = 0f;
            }
            Vector3 forceForwardBack = forward * 14 * moveForwardBack * throttleFactor;
            Vector3 forceUpDown = up * 8 * moveUpDown * throttleFactor;
            Vector3 forceRightLeft = right * 14 * moveRightLeft * throttleFactor;

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
            float shipYaw = -mousePercentX * 3.5f * throttleFactor;
            float shipRoll = 0;

            // Setup Bank Variables
            float forwardSpeed = Mathf.Clamp(Vector3.Dot(hcRigidbody.velocity, forward) / MAX_SPEED, -1, 1);
            float rightSpeed = Mathf.Clamp(Vector3.Dot(hcRigidbody.velocity, right) / MAX_SPEED, -1, 1);
            float bankRoll = moveRightLeft * MAX_BANK_ANGLE * throttleFactor;
            float bankYaw = moveForwardBack * MAX_BANK_ANGLE * throttleFactor;

            // Calculate X Rotation (Bank)
            float targetCraftRotationX = bankYaw;
            float distanceFromMax = (MAX_BANK_ANGLE - Mathf.Abs(craftRotationX)) / MAX_BANK_ANGLE;
            distanceFromMax = distanceFromMax * (1f - BANK_TRANSITION_TAPERING) + BANK_TRANSITION_TAPERING;
            if ((targetCraftRotationX > craftRotationX && craftRotationX < 0f)
            		|| (targetCraftRotationX < craftRotationX && craftRotationX > 0f)) {
            	distanceFromMax = 1f;
            }
            if (targetCraftRotationX < craftRotationX) {
                craftRotationX = Mathf.Clamp(craftRotationX - (MAX_CRAFT_CHANGE_ROT * distanceFromMax), targetCraftRotationX, 100);
            	print("Temp - x3 - " + craftRotationX.ToString());
            } else if (targetCraftRotationX > craftRotationX) {
                craftRotationX = Mathf.Clamp(craftRotationX + (MAX_CRAFT_CHANGE_ROT * distanceFromMax), -100, targetCraftRotationX);
            	print("Temp - x4 - " + craftRotationX.ToString());
            }

            // Calculate Y Rotation (Turn)
            float targetCraftRotationY = craftRotationY + shipYaw;
            craftRotationY = targetCraftRotationY;

            // Calculate Z Rotation (Bank)
            float targetCraftRotationZ = -bankRoll;
            distanceFromMax = (MAX_BANK_ANGLE - Mathf.Abs(craftRotationZ)) / MAX_BANK_ANGLE;
            distanceFromMax = distanceFromMax * (1f - BANK_TRANSITION_TAPERING) + BANK_TRANSITION_TAPERING;
            if ((targetCraftRotationZ > craftRotationZ && craftRotationZ < 0f)
            		|| (targetCraftRotationZ < craftRotationZ && craftRotationZ > 0f)) {
            	distanceFromMax = 1f;
            }
            if (targetCraftRotationZ < craftRotationZ) {
                craftRotationZ = Mathf.Clamp(craftRotationZ - (MAX_CRAFT_CHANGE_ROT * distanceFromMax), targetCraftRotationZ, 100);
            } else if (targetCraftRotationZ > craftRotationZ) {
                craftRotationZ = Mathf.Clamp(craftRotationZ + (MAX_CRAFT_CHANGE_ROT * distanceFromMax), -100, targetCraftRotationZ);
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
            float targetPositionZ = -(Mathf.Clamp(speedFactor * 2f, 0f, 1f) * CAMERA_DEPTH_FACTOR);
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


            /*--- Update Control Surfaces ---*/

            // Update Throttle
            if (currentThrottle != 1f) {
            	currentThrottle = Mathf.Clamp01(currentThrottle + (Time.deltaTime * THROTTLE_CHANGE_RATE));
            }

            // Update Propeller Speed
            propellerBig.transform.Rotate(new Vector3(0, currentThrottle * 360 * 10, 0) * Time.deltaTime);
            propellerSmall.transform.Rotate(new Vector3(currentThrottle * 360 * 10, 0, 0) * Time.deltaTime);

            // Calculate Propeller Visibility
            float visibility = 0f;
            if (currentThrottle > .5f) {
            	visibility = (currentThrottle - .5f) * 2f;
            }

            // Update Big Propeller Visibility
            var spinnerRenderer = propellerBig.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	1 - visibility
            ));

            // Update Small Propeller Visibility
            spinnerRenderer = propellerSmall.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	1 - visibility
            ));

            // Update Big Propeller Spinner Visibility
            spinnerRenderer = propellerSpinnerBig.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	visibility  * .3f
            ));

            // Update SmallPropeller Spinner Visibility
            spinnerRenderer = propellerSpinnerSmall.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	visibility  * .3f
            ));


        } else {

            // Apply Default Gravity
            Vector3 forceGravity = new Vector3(0, -9.81f, 0);
            hcRigidbody.AddForce(forceGravity);
        }
    }


    /*--- Public Methods ---*/

    public void showHighlight() {

        // Get Renderers
        var renderer = gameObject.GetComponent<Renderer>();
        var propellerBigRenderer = propellerBig.GetComponent<Renderer>();
        var propellerSmallRenderer = propellerSmall.GetComponent<Renderer>();

        // Set Emission
        renderer.material.EnableKeyword("_EMISSION");
        propellerBigRenderer.material.EnableKeyword("_EMISSION");
        propellerSmallRenderer.material.EnableKeyword("_EMISSION");
    }

    public void hideHighlight() {

        // Get Renderers
        var renderer = gameObject.GetComponent<Renderer>();
        var propellerBigRenderer = propellerBig.GetComponent<Renderer>();
        var propellerSmallRenderer = propellerSmall.GetComponent<Renderer>();

        // Set Emission
        renderer.material.DisableKeyword("_EMISSION");
        propellerBigRenderer.material.DisableKeyword("_EMISSION");
        propellerSmallRenderer.material.DisableKeyword("_EMISSION");
    }

    public void reset() {

        // Reset Position, Rotation, Velocity, Throttle, Highlight
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        craftRotationX = initialRotation.eulerAngles.x;
        craftRotationY = initialRotation.eulerAngles.y;
        craftRotationZ = initialRotation.eulerAngles.z;
        hcRigidbody.velocity = new Vector3(0, 0, 0);
        currentThrottle = 0f;
        hideHighlight();

        // Reset Propellers
        var spinnerRenderer = propellerBig.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	1f
        ));
        spinnerRenderer = propellerSmall.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	1f
        ));

        // Update Propeller Spinner Visibilities
        spinnerRenderer = propellerSpinnerBig.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	0f
        ));
        spinnerRenderer = propellerSpinnerSmall.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	0f
        ));
    }
}
