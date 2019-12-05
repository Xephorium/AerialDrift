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
    public GameObject propeller;
    public GameObject propellerSpinner;
    public GameObject trailEmitterLeft;
    public GameObject trailEmitterRight;
    public GameObject projectileEmitter;
    public Transform projectilePrefab;
    public bool isPlayerControlling = false;

    // Private Constants
    private static float CAMERA_MOVEMENT_FACTOR = .8f;
    private static float MAX_SPEED = 23f;
    private static float TRAIL_SPEED = 22f;
    private static float TRAIL_FADE = 5f;
    private static float MAX_GRAVITY = -13f;
    private static float MAX_FIN_ANGLE = 20f;
    private static float MAX_CAMERA_CHANGE_POS = .01f;
    private static float MAX_CAMERA_CHANGE_ROT = .01f;
    private static float FIRE_DELAY = .35f;

    // Private Variables
    private Rigidbody bpRigidbody;
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

    // Flight Simulation Constants
    private static float MAX_ENGINE_POWER = 33f;       // The maximum output of the engine.
    private static float LIFT_FACTOR = 0.007f;         // The amount of lift generated by the aeroplane moving forwards.
    private static float ROLL_FACTOR = 1f;             // The strength of effect for roll input.
    private static float PITCH_FACTOR = 1f;            // The strength of effect for pitch input.
    private static float YAW_FACTOR = 0.2f;            // The strength of effect for yaw input.
    private static float BANK_TURN_FACTOR = 0.5f;      // The amount of turn from doing a banked turn.
    private static float AERODYNAMICS_FACTOR = 0.04f;  // How much aerodynamics affect the speed of the aeroplane.
    private static float THROTTLE_CHANGE_RATE = 0.3f;  // The speed with which the throttle changes.
    private static float DRAG_FALLOFF_FACTOR = 0.006f; // How much drag should increase with speed.

    // Flight Simulation Variables
    private float initialDrag;
    private float initialAngularDrag;
    private float yawInput;
    private float pitchInput;
    private float rollInput;
    private float throttleInput;
    private float currentThrottle;
    private float forwardSpeed;
    private float enginePower;
    private float aeroFactor;
    private float bankedTurnAmount;


    /*--- Lifecycle Methods ---*/

    void Start() {

        // Set Instance
        instance = this;

        // Get Initial State
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get Components
        bpRigidbody = GetComponent<Rigidbody>();
        initialDrag = bpRigidbody.drag;
        initialAngularDrag = bpRigidbody.angularDrag;
        trailLeft = trailEmitterLeft.GetComponent<TrailRenderer>();
        trailRight = trailEmitterRight.GetComponent<TrailRenderer>();
    }

    void FixedUpdate() {
        if (isPlayerControlling) {


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
                projectileRigidbody.AddForce((transform.forward + (transform.up * .05f)) * 7000f);
            }


            /*--- Simulate Flight Mechanics ---*/

            // Calculate Pitch & Yaw
            float mousePercentXRaw = (Screen.width/2 - (Input.mousePosition[0] + 20)) / (Screen.height * .75f);
            float mousePercentYRaw = (Screen.height/2 - (Input.mousePosition[1] - 20)) / (Screen.height * .75f);
            float mousePercentX = Mathf.Sign(mousePercentXRaw) * Mathf.Pow(mousePercentXRaw, 2);
            float mousePercentY = Mathf.Sign(mousePercentYRaw) * Mathf.Pow(mousePercentYRaw, 2);

            // Finalize Movement Inputs
            pitchInput = mousePercentY * 10;
            yawInput = -mousePercentX * 10;
            rollInput = Input.GetAxis("ControlRoll");
            throttleInput = Input.GetAxis("ControlForwardBack");

            // Determine Position & Rotation Forces From Inputs
            simulateFlightMechanics();


            /*--- Update Camera ---*/

            // Calculate Speed
            float speedFactor = Mathf.Clamp01(bpRigidbody.velocity.magnitude / MAX_SPEED);

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
            if (-speedFactor < cameraPositionZ) {
                cameraPositionZ = Mathf.Clamp(cameraPositionZ - MAX_CAMERA_CHANGE_POS, -speedFactor, 0);
            } else if (-speedFactor > cameraPositionZ) {
                cameraPositionZ = Mathf.Clamp(cameraPositionZ + MAX_CAMERA_CHANGE_POS, -1, -speedFactor);
            }

            // Apply Camera Movement
            cameraEmpty.transform.localPosition = new Vector3(
                cameraPositionX,
                cameraPositionY,
                cameraPositionZ
            );

            // Calculate Camera X Rotation
            float targetRotationX = -mousePercentY * 3f * speedFactor;
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


            /*--- Update Control Surfaces ---*/

            // Calculate Angles
            float controlSurfaceFactor = (speedFactor * .5f) + .5f;
            float horizontalFactor = Mathf.Clamp(-mousePercentY * 4f, -1, 1);
            float horizontalFinAngle = MAX_FIN_ANGLE * horizontalFactor * controlSurfaceFactor;
            float verticalFactor = Mathf.Clamp(mousePercentX * 2f, -1, 1);
            float verticalFinAngle = MAX_FIN_ANGLE * verticalFactor * controlSurfaceFactor;

            // Set Rotations
            finBigTop.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);
            finBigBottom.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);
            finSmallTop.transform.localRotation = Quaternion.Euler(0,verticalFinAngle,0);
            finSmallBottom.transform.localRotation = Quaternion.Euler(horizontalFinAngle,0,0);

            // Update Propeller Speed
            propeller.transform.Rotate(new Vector3(0, 0, currentThrottle * 360 * 10) * Time.deltaTime);

            // Calculate Propeller Visibility
            float visibility = 0f;
            if (currentThrottle > .5) {
            	visibility = (currentThrottle - .5f) * 2f;
            }

            // Update Propeller Visibility
            var spinnerRenderer = propeller.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	1 - visibility
            ));

            // Update Propeller Spinner Visibility
            spinnerRenderer = propellerSpinner.GetComponent<Renderer>();
            spinnerRenderer.material.SetColor("_Color", new Color(
            	spinnerRenderer.material.color.r,
            	spinnerRenderer.material.color.g,
            	spinnerRenderer.material.color.b,
            	visibility  * .3f
            ));


            /*--- Update Trails ---*/

            // Compute Trail Visibility
            float trailVisibility = Mathf.Clamp01(
                (bpRigidbody.velocity.magnitude - (TRAIL_SPEED - TRAIL_FADE))
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
            bpRigidbody.AddForce(forceGravity);
        }
    }


    /*--- Public Methods ---*/

    public void reset() {

        // Reset State Variables
        currentThrottle = 0;
        trailLeft.emitting = false;
        trailRight.emitting = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        bpRigidbody.velocity = new Vector3(0, 0, 0);

        // Reset Propellers
        var spinnerRenderer = propeller.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	1f
        ));

        // Update Propeller Spinner Visibility
        spinnerRenderer = propellerSpinner.GetComponent<Renderer>();
        spinnerRenderer.material.SetColor("_Color", new Color(
        	spinnerRenderer.material.color.r,
        	spinnerRenderer.material.color.g,
        	spinnerRenderer.material.color.b,
        	0f
        ));
    }


    /*--- Flight Simulation Methods ---*/

    // Note: The following code was reverse-engineered from Unity's sample Aeroplane
    //       project and stripped down to suit this project's goals. I initially coded a
    //       flight model from scratch, but without a deep dive into the physics of lift
    //       and aerodynamics, the controls just weren't turning out to be as satisfying
    //       as I hoped. All code above this point was written from scratch for this project.
    // Link: https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-32351

    private void simulateFlightMechanics() {
            clampInputs();
            calculateForwardSpeed();
            calculateThrottle();
            calculateDrag();
            caluclateAerodynamicEffect();
            calculateLinearForces();
            calculateBankedTurnAmount();
            CalculateTorque();
    }

    private void clampInputs() {
        rollInput = Mathf.Clamp(rollInput, -1, 1);
        pitchInput = Mathf.Clamp(pitchInput, -1, 1);
        throttleInput = Mathf.Clamp(throttleInput, -1, 1);
    }


    private void calculateForwardSpeed() {
        var localVelocity = transform.InverseTransformDirection(bpRigidbody.velocity);
        forwardSpeed = Mathf.Max(0, localVelocity.z);
    }


    private void calculateThrottle() {
        if (throttleInput < 1) {
            if (currentThrottle > 0.01f) {
                throttleInput = -1;
            } else {
                throttleInput = 0;
                currentThrottle = 0;
            }
        }
        currentThrottle = Mathf.Clamp01(currentThrottle + throttleInput*Time.deltaTime*THROTTLE_CHANGE_RATE);
        enginePower = currentThrottle*MAX_ENGINE_POWER;
    }


    private void calculateDrag() {
        float extraDrag = bpRigidbody.velocity.magnitude*DRAG_FALLOFF_FACTOR;
        bpRigidbody.drag = initialDrag + extraDrag;
        bpRigidbody.angularDrag = initialAngularDrag*forwardSpeed;
    }


    private void caluclateAerodynamicEffect() {
        if (bpRigidbody.velocity.magnitude > .01) {
            aeroFactor = Vector3.Dot(transform.forward, bpRigidbody.velocity.normalized);
            aeroFactor *= aeroFactor;
            var newVelocity = Vector3.Lerp(
                bpRigidbody.velocity,
                transform.forward*forwardSpeed,
                aeroFactor*forwardSpeed*AERODYNAMICS_FACTOR*Time.deltaTime
            );
            bpRigidbody.velocity = newVelocity;
            bpRigidbody.rotation = Quaternion.Slerp(
                bpRigidbody.rotation,
                Quaternion.LookRotation(
                    bpRigidbody.velocity, 
                    transform.up
                ),
                AERODYNAMICS_FACTOR*Time.deltaTime
            );
        }
    }

    private void calculateLinearForces() {
        var forces = Vector3.zero;
        forces += enginePower*transform.forward;
        var liftDirection = Vector3.Cross(bpRigidbody.velocity, transform.right).normalized;
        var liftPower = forwardSpeed*forwardSpeed*LIFT_FACTOR*aeroFactor;
        forces += liftPower*liftDirection*5;
        bpRigidbody.AddForce(forces);
    }

    private void calculateBankedTurnAmount() {
        var flatForward = transform.forward;
        flatForward.y = 0;
        if (flatForward.sqrMagnitude > 0){
            flatForward.Normalize();
            var localFlatForward = transform.InverseTransformDirection(flatForward);
            var flatRight = Vector3.Cross(Vector3.up, flatForward);
            var localFlatRight = transform.InverseTransformDirection(flatRight);
            float rollAngle = Mathf.Atan2(localFlatRight.y, localFlatRight.x);
            bankedTurnAmount = Mathf.Sin(rollAngle);
        }
    }

    private void CalculateTorque() {
        var torque = Vector3.zero;
        torque += pitchInput*PITCH_FACTOR*transform.right;
        torque += yawInput*YAW_FACTOR*(transform.up*2);
        torque += -rollInput*ROLL_FACTOR*(transform.forward*2);
        torque += bankedTurnAmount*BANK_TURN_FACTOR*transform.up;
        bpRigidbody.AddTorque(torque*forwardSpeed*aeroFactor);
    }
}
