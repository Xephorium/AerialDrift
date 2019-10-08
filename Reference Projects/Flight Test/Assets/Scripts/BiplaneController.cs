using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiplaneController : MonoBehaviour {


    /*--- Variable Declarations ---*/

    // Public Constants
    public static BiplaneController instance;

    // Public Variables
    public GameObject cameraEmpty;
    public Camera camera;
    public GameObject finBigTop;
    public GameObject finBigBottom;
    public GameObject finSmallTop;
    public GameObject finSmallBottom;
    public GameObject trailEmitterLeft;
    public GameObject trailEmitterRight;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;
    private static float MAX_SPEED = 20f;
    private static float MAX_GRAVITY = -13f;
    private static float GRAVITY_DAMPENING = .75f;
    private static float MAX_FIN_ANGLE = 20f;

    // Private Variables
    private Rigidbody myRigidBody;
    private TrailRenderer trailLeft;
    private TrailRenderer trailRight;
    private Vector3 initialPosition;
    private Quaternion initialRotation;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        myRigidBody = GetComponent<Rigidbody>();
        trailLeft = trailEmitterLeft.GetComponent<TrailRenderer>();
        trailRight = trailEmitterRight.GetComponent<TrailRenderer>();
    }

    void FixedUpdate() {
        if (isPlayerControlling) {
      

            /*--- Update Plane Translation ---*/

            // Get Movement Inputs Since Update
            float moveForwardBack = Input.GetAxis("ControlForwardBack");
            if (moveForwardBack < 0) {
                moveForwardBack *= 0;
            }

            // Create Individual Force Vectors
            Vector3 forceForwardBack = transform.forward * 10 * moveForwardBack;

            // Calculate Force of Gravity
            float speedFactor = 1 - (myRigidBody.velocity.magnitude / MAX_SPEED);
            float gravity = (speedFactor * GRAVITY_DAMPENING) * MAX_GRAVITY;
            if (gravity < MAX_GRAVITY) {
                gravity = MAX_GRAVITY;
            }
            Vector3 forceGravity = new Vector3(0, gravity, 0);

            // Combine Force Vectors
            Vector3 combinedForce = forceForwardBack + forceGravity;

            // Apply Force to Rigidbody Component
            myRigidBody.AddForce(combinedForce);


            /*--- Update Plane Rotation ---*/

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

            // Cap Rotation Based on Speed
            shipPitch -= ((shipPitch * .4f) * (1 - speedFactor));
            shipYaw -= ((shipYaw * .4f) * (1 - speedFactor));
            shipRoll -= ((shipRoll * .4f) * (1 - speedFactor));;

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


            /*--- Update Control Surfaces ---*/

            // Calculate Angles
            float horizontalFinAngle = MAX_FIN_ANGLE * -mousePercentY * 7;
            if (horizontalFinAngle > MAX_FIN_ANGLE) {
                horizontalFinAngle = MAX_FIN_ANGLE;
            } else if (horizontalFinAngle < -MAX_FIN_ANGLE) {
                horizontalFinAngle = -MAX_FIN_ANGLE;
            }
            float verticalFinAngle = (MAX_FIN_ANGLE / 2) * mousePercentX * 7;
            if (verticalFinAngle > MAX_FIN_ANGLE) {
                verticalFinAngle = MAX_FIN_ANGLE;
            } else if (verticalFinAngle < -MAX_FIN_ANGLE) {
                verticalFinAngle = -MAX_FIN_ANGLE;
            }

            // Set Rotations
            finBigTop.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);
            finBigBottom.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);
            finSmallTop.transform.localRotation = Quaternion.Euler(0,verticalFinAngle,0);
            finSmallBottom.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);


            /*--- Update Trails ---*/

            trailLeft.emitting = true;
            trailRight.emitting = true;
            Color trailColor = new Color(1, 1, 1, (1 - (speedFactor * 3f)));
            trailLeft.startColor = trailColor;
            trailLeft.endColor = trailColor;
            trailRight.startColor = trailColor;
            trailRight.endColor = trailColor;
        
        } else {

            // Apply Default Gravity
            Vector3 forceGravity = new Vector3(0, MAX_GRAVITY, 0);
            myRigidBody.AddForce(forceGravity);
        }
    }


    /*--- Public Methods ---*/

    public void reset() {

        // Reset Position, Rotation, Velocity
        trailLeft.emitting = false;
        trailRight.emitting = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        myRigidBody.velocity = new Vector3(0, 0, 0);
    }
}
