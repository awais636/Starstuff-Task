using UnityEngine;
using Terresquall;

/// <summary>
/// Starstuff Technical challenge
/// Universal arcade style vehicle controller for random meshes.
/// Unity 6.3 / PhysX
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class UniversalVehicleController : MonoBehaviour
{
    Rigidbody rb;
    Collider col;

    // Tunable (global defaults)

    [Header("Movement")]
    public float acceleration = 30f; 
    public float maxSpeed = 18f; 
    public float airControlMultiplier = 0.4f;

    [Header("Steering")]
    public float steeringStrength = 4.5f;
    public float steeringAtLowSpeed = 2f;

    [Header("Grip")]
    public float lateralGrip = 0.2f;

    [Header("Stability")]
    public float uprightTorque = 25f;
    public float angularDamping = 4f;

    [Header("Auto Recovery")]
    public float recoveryTorque = 15f;
    public float recoverySpeedThreshold = 1.5f;

    [Header("Grounding")]
    public float groundRayLength = 1.2f;
    public int groundRayCount = 5;

    // Runtime state
    float throttleInput;
    float steerInput;

    bool grounded;
    Vector3 groundNormal = Vector3.up;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        SetupRigidbody();
    }
    
    void Update()
    {
        // INPUT ONLY (no physics)
        #if UNITY_WEBGL || UNITY_EDITOR
        throttleInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        #endif

     // Virtual Joystick will work on Mobile
     #if UNITY_IOS || UNITY_ANDROID
       throttleInput = VirtualJoystick.GetAxis("Vertical");
       steerInput = VirtualJoystick.GetAxis("Horizontal");
     #endif
        
    }

    void FixedUpdate()
    {
        UpdateGrounding();
        ApplyMovement();
        ApplySteering();
        ApplyGrip();
        ApplyStability();
        ApplyAutoRecovery();
    }

    
    // 1) Ground Detection (shape-agnostic, multi-ray)
    void UpdateGrounding()
    {
        grounded = false;
        groundNormal = Vector3.up;

        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 center = bounds.center;

        int hits = 0;
        Vector3 normalSum = Vector3.zero;

        for (int i = 0; i < groundRayCount; i++)
        {
            Vector3 offset = Random.insideUnitSphere * bounds.extents.x * 0.5f;
            Vector3 origin = center + offset;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundRayLength))
            {
                grounded = true;
                normalSum += hit.normal;
                hits++;
            }
        }

        if (hits > 0)
            groundNormal = normalSum.normalized;
    }

    // 2) Forward / Reverse Movement
    void ApplyMovement()
    {
        if (Mathf.Abs(throttleInput) < 0.01f)
            return;

        Vector3 forwardOnGround = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
        float control = grounded ? 1f : airControlMultiplier;

        Vector3 force = forwardOnGround * throttleInput * acceleration * control;
        rb.AddForce(force, ForceMode.Acceleration);

        // Speed limit
        Vector3 planarVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, groundNormal);
        if (planarVelocity.magnitude > maxSpeed)
        {
            Vector3 limited = planarVelocity.normalized * maxSpeed;
            rb.linearVelocity = limited + Vector3.Project(rb.linearVelocity, groundNormal);
        }
    }

    // 3) Steering (velocity guided yaw)
    void ApplySteering()
    {
    if (!grounded || Mathf.Abs(steerInput) < 0.01f)
        return;

    Vector3 planarVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, groundNormal);
    float speed = planarVelocity.magnitude;

    if (speed < 0.2f)
        return;

    // Speed based steering authority
    float speedFactor = Mathf.Clamp01(speed / maxSpeed);

    // Desired yaw angular velocity (radians/sec)
    float desiredYaw = steerInput * steeringStrength * speedFactor;

    // Current angular velocity projected onto ground normal
    float currentYaw = Vector3.Dot(rb.angularVelocity, groundNormal);

    // Smoothly move toward desired yaw
    float yawDelta = desiredYaw - currentYaw;

    rb.AddTorque(groundNormal * yawDelta, ForceMode.VelocityChange);
    }

    // Mentioned in the task "It should be grippy, snappy" 
    // 4) Arcade Grip System
    void ApplyGrip()
    {
        if (!grounded)
            return;

    Vector3 velocity = rb.linearVelocity;

    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized;
    Vector3 forwardVelocity = Vector3.Project(velocity, forward);
    Vector3 lateralVelocity = velocity - forwardVelocity;

    // Strong arcade grip: kill sideways velocity
    rb.AddForce(-lateralVelocity * lateralGrip, ForceMode.Acceleration);
    }


    // As mentioned in the task "hard to flip over"
    // 5) Stability / Anti-Flip
    void ApplyStability()
    {
        if (!grounded)
            return;

        Vector3 currentUp = transform.up;
        Vector3 targetUp = groundNormal;

        Vector3 torqueAxis = Vector3.Cross(currentUp, targetUp);
        rb.AddTorque(torqueAxis * uprightTorque, ForceMode.Acceleration);
    }

    // As mentioned in the task "easy to flip back when it inevitably does"
    // 6) Auto Upright Recovery
    void ApplyAutoRecovery()
    {
        if (rb.linearVelocity.magnitude > recoverySpeedThreshold)
            return;

        float upsideDown = Vector3.Dot(transform.up, Vector3.up);
        if (upsideDown > 0.3f)
            return;

        Vector3 torqueAxis = Vector3.Cross(transform.up, Vector3.up);
        rb.AddTorque(torqueAxis * recoveryTorque, ForceMode.Acceleration);
    }

    void SetupRigidbody()
    {
    rb.mass = 1f;

    rb.linearDamping = 0.2f;
    rb.angularDamping = 4f;

    rb.useGravity = true;
    rb.isKinematic = false;

    rb.interpolation = RigidbodyInterpolation.Interpolate;
    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

    rb.centerOfMass = Vector3.down * 0.3f;
    }
}