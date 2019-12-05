using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViperController : MonoBehaviour
{

    /*--- Variable Declarations ---*/

    // Public Constants
    public static ViperController instance;

    // Public Variables
    public GameObject cameraEmpty;
    public Camera camera;
    public GameObject trailEmitterLeft;
    public GameObject trailEmitterRight;
    public GameObject jetTop;
    public GameObject jetSmokeTop;
    public GameObject jetLeft;
    public GameObject jetSmokeLeft;
    public GameObject jetRight;
    public GameObject jetSmokeRight;
    public GameObject projectileEmitter;
    public Transform projectilePrefab;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;
    private static float CAMERA_DEPTH_FACTOR = .6f;
    private static float CAMERA_VERTICAL_BOOST_OFFSET = .2f;
    private static float MAX_SPEED = 20f;
    private static float MAX_CAMERA_CHANGE_POS = .01f;
    private static float MAX_CAMERA_CHANGE_ROT = .01f;
    private static float FIRE_DELAY = .35f;

    // Private Variables
    private Rigidbody vpRigidbody;
    private TrailRenderer trailLeft;
    private TrailRenderer trailRight;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float cameraPositionX = 0f;
    private float cameraPositionY = 0f;
    private float cameraPositionZ = 0f;
    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;
    private float cameraRotationZ = 0f;
    private float lastFireTime = 0f;

    // Boost Constants
    private static float DEFAULT_FOV = 70f;
    private static float BOOST_FOV = 75f;
    private static float boostAnimationTime = .4f;

    // Boost Variables
    private float boostTimeStart = 0f;
    private float boostTimeEnd = 0f;
    private float boostFOVStart = DEFAULT_FOV;
    private float boostFOVEnd = DEFAULT_FOV;
    private float boostAnimationFactor = 0f;
    private bool boostChange = false;
    private bool boostAnimating = false;
    private bool boost = false;

    // Jet Exhaust Constants
    private float jetLengthFactor = .85f;

    // Jet Exhaust Variables
    private float jetStartSpeed;
    private float jetDefaultStartSpeed;
    private float jetSmokeStartSpeed;
    private float jetSmokeDefaultStartSpeed;
    private ParticleSystem.MainModule jetCurveTop;
    private ParticleSystem.MainModule jetSmokeCurveTop;
    private ParticleSystem.MainModule jetCurveLeft;
    private ParticleSystem.MainModule jetSmokeCurveLeft;
    private ParticleSystem.MainModule jetCurveRight;
    private ParticleSystem.MainModule jetSmokeCurveRight;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        vpRigidbody = GetComponent<Rigidbody>();
        trailLeft = trailEmitterLeft.GetComponent<TrailRenderer>();
        trailRight = trailEmitterRight.GetComponent<TrailRenderer>();

        // Get Jet Exhaust Components
        jetCurveTop = jetTop.GetComponent<ParticleSystem>().main;
        jetSmokeCurveTop = jetSmokeTop.GetComponent<ParticleSystem>().main;
        jetCurveLeft = jetLeft.GetComponent<ParticleSystem>().main;
        jetSmokeCurveLeft = jetSmokeLeft.GetComponent<ParticleSystem>().main;
        jetCurveRight = jetRight.GetComponent<ParticleSystem>().main;
        jetSmokeCurveRight = jetSmokeRight.GetComponent<ParticleSystem>().main;

        // Get Jet Exhaust Variables
        jetDefaultStartSpeed = jetCurveTop.startSpeed.constant;
        jetSmokeDefaultStartSpeed = jetSmokeCurveTop.startSpeed.constant;
        jetStartSpeed = jetDefaultStartSpeed;
        jetSmokeStartSpeed = jetSmokeDefaultStartSpeed;
    }

    void FixedUpdate() {
        if (isPlayerControlling) {


            /*--- Detect Keypresses ---*/

            if (Input.GetKey(KeyCode.LeftShift) != boost
                && (Input.GetAxis("ControlForwardBack") > 0
                    || !Input.GetKey(KeyCode.LeftShift))) {
                boostChange = true;
            } else {
                boostChange = false;
            }
            boost = Input.GetKey(KeyCode.LeftShift);
            float fireInput = Input.GetAxis("Fire1");


            /*--- Fire Projectile ---*/

            // Account for Fire Delay
            bool fire = fireInput == 1f && lastFireTime + FIRE_DELAY < Time.fixedTime;
            
            if (fire) {

            	// Update Last Fire Time
            	lastFireTime = Time.fixedTime;

            	// Create Pojectile
            	Transform projectile = Instantiate(projectilePrefab, projectileEmitter.transform.position, transform.rotation);

            	// Apply Forward Force
            	Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            	projectileRigidbody.AddForce((transform.forward + (transform.up * .05f)) * 7000f);
            }
      

            /*--- Update Ship Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");
            float moveUpDown = Input.GetAxis("ControlUpDown");

            // Create Individual Force Vectors
            Vector3 forceForwardBack = transform.forward * 10 * moveForwardBack;
            Vector3 forceUpDown = transform.up * 10 * moveUpDown;

            // Apply Boost
            if (moveForwardBack > 0 && boost) {
                forceForwardBack *= 2;
            }

            // Combine Force Vectors
            Vector3 combinedForce = forceForwardBack + forceUpDown;

            // Apply Force to Rigidbody Component
            vpRigidbody.AddForce(combinedForce);
            //vpRigidbody.velocity = combinedForce;


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
            float speedFactor = Mathf.Clamp01(vpRigidbody.velocity.magnitude / MAX_SPEED);

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


            /*--- Update Boost Variables ---*/

            // Set Boost Variables on Change
            if (boostChange) {
                boostTimeStart = Time.fixedTime;
                boostTimeEnd = boostTimeStart + boostAnimationTime;
                boostFOVStart = camera.fieldOfView;
                boostAnimating = true;
                if (boost) {
                    boostFOVEnd = BOOST_FOV;
                } else {
                    boostFOVEnd = DEFAULT_FOV;
                }
                trailLeft.emitting = boost;
                trailRight.emitting = boost;
            }

            // Note FOV Animation Completion
            if (Time.fixedTime >= boostTimeEnd) {
                boostAnimating = false;
            }

        } else {

            // Apply Default Gravity
            Vector3 forceGravity = new Vector3(0, -9.81f, 0);
            vpRigidbody.AddForce(forceGravity);
        }
    }

    void Update() {
        if (isPlayerControlling) {

        
            /*--- Update Camera ---*/

            // Update FOV
            if (boostAnimating) {
                
                // Calculate & Set FOV
                boostAnimationFactor = 1 - Mathf.Abs((boostTimeEnd - Time.fixedTime)/(boostTimeEnd - boostTimeStart));
                float normalizedInput = boostAnimationFactor * (Mathf.PI / 3);
                float fovFactor = .5f * (1 + Mathf.Sin(3 * normalizedInput - Mathf.PI / 2));
                float currentFOV = boostFOVStart + (fovFactor * (boostFOVEnd - boostFOVStart));
                camera.fieldOfView = currentFOV;

            }


            /*--- Update Jet Trails ---*/

            // Calculate Exhaust Lengths
            jetStartSpeed = jetDefaultStartSpeed
                + (jetDefaultStartSpeed * jetLengthFactor * (boost ? 1 : 0))
                + (jetDefaultStartSpeed * jetLengthFactor * (Input.GetAxis("ControlForwardBack") > 0 ? 1 : 0));
            jetSmokeStartSpeed = jetSmokeDefaultStartSpeed
                + (jetSmokeDefaultStartSpeed * jetLengthFactor * (boost ? 1 : 0))
                + (jetSmokeDefaultStartSpeed * jetLengthFactor * (Input.GetAxis("ControlForwardBack") > 0 ? 1 : 0));

            // Set Exhaust Lengths
            jetCurveTop.startSpeed = jetStartSpeed;
            jetSmokeCurveTop.startSpeed = jetSmokeStartSpeed;
            jetCurveLeft.startSpeed = jetStartSpeed;
            jetSmokeCurveLeft.startSpeed = jetSmokeStartSpeed;
            jetCurveRight.startSpeed = jetStartSpeed;
            jetSmokeCurveRight.startSpeed = jetSmokeStartSpeed;
        }
    }


    /*--- Public Methods ---*/

    public void reset() {

        // Reset Boost Effects
        boost = false;
        boostAnimating = false;
        camera.fieldOfView = DEFAULT_FOV;
        trailLeft.emitting = false;
        trailRight.emitting = false;

        // Reset Position, Rotation, Velocity
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        vpRigidbody.velocity = new Vector3(0, 0, 0);
    }
}
