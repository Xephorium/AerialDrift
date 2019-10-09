﻿using System.Collections;
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
    private static float MAX_SPEED = 40f;
    private static float TRAIL_SPEED = 30f;
    private static float TRAIL_FADE = 5f;
    private static float MAX_GRAVITY = -13f;
    private static float GRAVITY_DAMPENING = .75f;
    private static float MAX_FIN_ANGLE = 20f;

    // Private Variables
    private Rigidbody myRigidBody;
    private TrailRenderer trailLeft;
    private TrailRenderer trailRight;
    private Vector3 initialPosition;
    private Quaternion initialRotation;



    ///////////////////////////////////////////////////////////////////////////////////////////


    [SerializeField] private float m_MaxEnginePower = 40f;        // The maximum output of the engine.
    [SerializeField] private float m_Lift = 0.5f;                // The amount of lift generated by the aeroplane moving forwards.
    [SerializeField] private float m_ZeroLiftSpeed = 300;         // The speed at which lift is no longer applied.
    [SerializeField] private float m_RollEffect = 1f;             // The strength of effect for roll input.
    [SerializeField] private float m_PitchEffect = 4f;            // The strength of effect for pitch input.
    [SerializeField] private float m_YawEffect = 1000f;           // The strength of effect for yaw input.
    [SerializeField] private float m_BankedTurnEffect = 0.1f;     // The amount of turn from doing a banked turn.
    [SerializeField] private float m_AerodynamicEffect = 0.02f;   // How much aerodynamics affect the speed of the aeroplane.
    [SerializeField] private float m_AutoTurnPitch = 0.1f;        // How much the aeroplane automatically pitches when in a banked turn.
    [SerializeField] private float m_AutoRollLevel = 0.1f;        // How much the aeroplane tries to level when not rolling.
    [SerializeField] private float m_AutoPitchLevel = 0.1f;       // How much the aeroplane tries to level when not pitching.
    [SerializeField] private float m_AirBrakesEffect = 3f;        // How much the air brakes effect the drag.
    [SerializeField] private float m_ThrottleChangeSpeed = 0.3f;  // The speed with which the throttle changes.
    [SerializeField] private float m_DragIncreaseFactor = 0.1f; // how much drag should increase with speed.

    public float Throttle { get; private set; }                     // The amount of throttle being used.
    public bool AirBrakes { get; private set; }                     // Whether or not the air brakes are being applied.
    public float ForwardSpeed { get; private set; }                 // How fast the aeroplane is traveling in it's forward direction.
    public float EnginePower { get; private set; }                  // How much power the engine is being given.
    public float MaxEnginePower{ get { return m_MaxEnginePower; }}    // The maximum output of the engine.
    public float RollAngle { get; private set; }
    public float PitchAngle { get; private set; }
    public float RollInput { get; private set; }
    public float PitchInput { get; private set; }
    public float YawInput { get; private set; }
    public float ThrottleInput { get; private set; }

    private float m_OriginalDrag;         // The drag when the scene starts.
    private float m_OriginalAngularDrag;  // The angular drag when the scene starts.
    private float m_AeroFactor;
    private float m_BankedTurnAmount;
    private Rigidbody m_Rigidbody;
    WheelCollider[] m_WheelColliders;




    ////////////////////////////////////////////////////////////////////////////////////////////


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        myRigidBody = GetComponent<Rigidbody>();
        m_OriginalDrag = myRigidBody.drag;
        m_OriginalAngularDrag = myRigidBody.angularDrag;
        trailLeft = trailEmitterLeft.GetComponent<TrailRenderer>();
        trailRight = trailEmitterRight.GetComponent<TrailRenderer>();
    }

    void FixedUpdate() {
        if (isPlayerControlling) {
      
            /*--- Do The Flying ---*/

            // Calculate Roll
            float rotateRoll = Input.GetAxis("ControlRoll");

            // Calculate Pitch & Yaw
            float mousePercentXRaw = (Screen.width/2 - (Input.mousePosition[0] + 20)) / (Screen.height * .75f);
            float mousePercentYRaw = (Screen.height/2 - (Input.mousePosition[1] - 20)) / (Screen.height * .75f);
            float mousePercentX = Mathf.Sign(mousePercentXRaw) * Mathf.Pow(mousePercentXRaw, 2);
            float mousePercentY = Mathf.Sign(mousePercentYRaw) * Mathf.Pow(mousePercentYRaw, 2);

            // Apply Transformations
            float shipPitch = mousePercentY * 10;
            float shipYaw = -mousePercentX * 10;
            float shipRoll = rotateRoll;

            // Read input for the pitch, yaw, roll and throttle of the aeroplane.
            float roll = shipRoll;
            float pitch = shipPitch;
            float yaw = shipYaw;
            float throttle = Input.GetAxis("ControlForwardBack");

            // Pass the input to the aeroplane
            move(roll, pitch, yaw, throttle);

            print("Temp - " + myRigidBody.velocity.magnitude.ToString());


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
            float verticalFinAngle = (MAX_FIN_ANGLE / 3) * mousePercentX * 7;
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

            // Compute Trail Visibility
            float trailVisibility = Mathf.Clamp01(
                (myRigidBody.velocity.magnitude - (TRAIL_SPEED - TRAIL_FADE))
                / (TRAIL_SPEED - (TRAIL_SPEED - TRAIL_FADE))
            );

            // Set Trail Visibility
            trailLeft.emitting = true;
            trailRight.emitting = true;
            Color trailColor = new Color(1, 1, 1, trailVisibility);
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

        // Reset State Variables
        Throttle = 0;
        trailLeft.emitting = false;
        trailRight.emitting = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        myRigidBody.velocity = new Vector3(0, 0, 0);
    }


    /*--- Aeroplane Controller Methods ---*/

    private void move(float rollInput, float pitchInput, float yawInput, float throttleInput) {
            // transfer input parameters into properties.s
            RollInput = rollInput;
            PitchInput = pitchInput;
            YawInput = yawInput;
            ThrottleInput = throttleInput;

            clampInputs();

            calculateRollAndPitchAngles();

            //AutoLevel();

            CalculateForwardSpeed();

            ControlThrottle();

            CalculateDrag();

            CaluclateAerodynamicEffect();

            CalculateLinearForces();

            CalculateTorque();
    }

    private void clampInputs() {
        // clamp the inputs to -1 to 1 range
        RollInput = Mathf.Clamp(RollInput, -1, 1);
        PitchInput = Mathf.Clamp(PitchInput, -1, 1);
        //YawInput = Mathf.Clamp(YawInput, -1, 1);
        ThrottleInput = Mathf.Clamp(ThrottleInput, -1, 1);
    }

    private void calculateRollAndPitchAngles() {
        // Calculate roll & pitch angles
        // Calculate the flat forward direction (with no y component).
        var flatForward = transform.forward;
        flatForward.y = 0;
        // If the flat forward vector is non-zero (which would only happen if the plane was pointing exactly straight upwards)
        if (flatForward.sqrMagnitude > 0)
        {
            flatForward.Normalize();
            // calculate current pitch angle
            var localFlatForward = transform.InverseTransformDirection(flatForward);
            PitchAngle = Mathf.Atan2(localFlatForward.y, localFlatForward.z);
            // calculate current roll angle
            var flatRight = Vector3.Cross(Vector3.up, flatForward);
            var localFlatRight = transform.InverseTransformDirection(flatRight);
            RollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
        }
    }

    private void AutoLevel() {
        // The banked turn amount (between -1 and 1) is the sine of the roll angle.
        // this is an amount applied to elevator input if the user is only using the banking controls,
        // because that's what people expect to happen in games!
        m_BankedTurnAmount = Mathf.Sin(RollAngle);
        // auto level roll, if there's no roll input:
        if (RollInput == 0f)
        {
            RollInput = -RollAngle*m_AutoRollLevel;
        }
        // auto correct pitch, if no pitch input (but also apply the banked turn amount)
        if (PitchInput == 0f)
        {
            PitchInput = -PitchAngle*m_AutoPitchLevel;
            PitchInput -= Mathf.Abs(m_BankedTurnAmount*m_BankedTurnAmount*m_AutoTurnPitch);
        }
    }


    private void CalculateForwardSpeed() {
        // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
        var localVelocity = transform.InverseTransformDirection(myRigidBody.velocity);
        ForwardSpeed = Mathf.Max(0, localVelocity.z);
    }


    private void ControlThrottle() {

        if (ThrottleInput < 1) {
            ThrottleInput = -1;
        }

        // Adjust throttle based on throttle input
        Throttle = Mathf.Clamp01(Throttle + ThrottleInput*Time.deltaTime*m_ThrottleChangeSpeed);

        // current engine power is just:
        EnginePower = Throttle*m_MaxEnginePower;
    }


    private void CalculateDrag() {
        // increase the drag based on speed, since a constant drag doesn't seem "Real" (tm) enough
        float extraDrag = myRigidBody.velocity.magnitude*m_DragIncreaseFactor;
        // Air brakes work by directly modifying drag. This part is actually pretty realistic!
        myRigidBody.drag = (AirBrakes ? (m_OriginalDrag + extraDrag)*m_AirBrakesEffect : m_OriginalDrag + extraDrag);
        // Forward speed affects angular drag - at high forward speed, it's much harder for the plane to spin
        myRigidBody.angularDrag = m_OriginalAngularDrag*ForwardSpeed;
    }


    private void CaluclateAerodynamicEffect() {
        // "Aerodynamic" calculations. This is a very simple approximation of the effect that a plane
        // will naturally try to align itself in the direction that it's facing when moving at speed.
        // Without this, the plane would behave a bit like the asteroids spaceship!
        if (myRigidBody.velocity.magnitude > 0)
        {
            // compare the direction we're pointing with the direction we're moving:
            m_AeroFactor = Vector3.Dot(transform.forward, myRigidBody.velocity.normalized);
            // multipled by itself results in a desirable rolloff curve of the effect
            m_AeroFactor *= m_AeroFactor;
            // Finally we calculate a new velocity by bending the current velocity direction towards
            // the the direction the plane is facing, by an amount based on this aeroFactor
            var newVelocity = Vector3.Lerp(myRigidBody.velocity, transform.forward*ForwardSpeed,
                                           m_AeroFactor*ForwardSpeed*m_AerodynamicEffect*Time.deltaTime);
            myRigidBody.velocity = newVelocity;

            // also rotate the plane towards the direction of movement - this should be a very small effect, but means the plane ends up
            // pointing downwards in a stall
            myRigidBody.rotation = Quaternion.Slerp(myRigidBody.rotation,
                                                  Quaternion.LookRotation(myRigidBody.velocity, transform.up),
                                                  m_AerodynamicEffect*Time.deltaTime);
        }
    }


    private void CalculateLinearForces() {
        // Now calculate forces acting on the aeroplane:
        // we accumulate forces into this variable:
        var forces = Vector3.zero;
        // Add the engine power in the forward direction
        forces += EnginePower*transform.forward;
        // The direction that the lift force is applied is at right angles to the plane's velocity (usually, this is 'up'!)
        var liftDirection = Vector3.Cross(myRigidBody.velocity, transform.right).normalized;
        // Calculate and add the lift power
        var liftPower = ForwardSpeed*ForwardSpeed*m_Lift*m_AeroFactor;
        forces += liftPower*liftDirection*5;
        // Apply the calculated forces to the the Rigidbody
        myRigidBody.AddForce(forces);
    }


    private void CalculateTorque() {
        // We accumulate torque forces into this variable:
        var torque = Vector3.zero;
        // Add torque for the pitch based on the pitch input.
        torque += PitchInput*m_PitchEffect*transform.right;
        // Add torque for the yaw based on the yaw input.
        torque += YawInput*m_YawEffect*(transform.up*2);
        // Add torque for the roll based on the roll input.
        torque += -RollInput*m_RollEffect*(transform.forward*2);
        // Add torque for banked turning.
        torque += m_BankedTurnAmount*m_BankedTurnEffect*transform.up;
        // The total torque is multiplied by the forward speed, so the controls have more effect at high speed,
        // and little effect at low speed, or when not moving in the direction of the nose of the plane
        // (i.e. falling while stalled)
        myRigidBody.AddTorque(torque*ForwardSpeed*m_AeroFactor);
    }
}
