using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Rigidbody targetRigidbody;

    [Header("Position")]
    public Vector3 offset = new Vector3(0f, 3.5f, -9f);
    public float positionSmoothTime = 0.12f;

    [Header("Rotation")]
    public float rotationSmoothSpeed = 7f;

    [Header("Zoom")]
    public float minZoom = 6f;
    public float maxZoom = 10f;
    public float zoomSmoothSpeed = 3f;
    public float maxSpeedForZoom = 40f;

    private Vector3 positionVelocity;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        AutoAssignTarget();
    }
    public void AutoAssignTarget()
    {
        //if (target != null && targetRigidbody != null)
          //  return;

        UniversalVehicleController vehicle = Object.FindAnyObjectByType<UniversalVehicleController>();

        if (vehicle == null)
        {
            Debug.LogWarning( "[CameraFollow] No UniversalVehicleController found in scene.");
            return;
        }

        target = vehicle.transform;

        //if (!targetRigidbody)
            targetRigidbody = vehicle.GetComponent<Rigidbody>();

        Debug.Log( $"[CameraFollow] Auto-assigned target: {vehicle.name}");    
    }



    void LateUpdate()
    {
        if (!target) return;

        FollowPosition();
        FollowRotation();
        HandleZoom();
    }

    void FollowPosition()
    {
        Vector3 desiredPosition = target.TransformPoint(offset);

        transform.position = Vector3.SmoothDamp(transform.position,desiredPosition,ref positionVelocity,positionSmoothTime );
    }

    void FollowRotation()
    {
        Vector3 forward = target.forward;
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothSpeed * Time.deltaTime );
    }

    void HandleZoom()
    {
        if (!cam || !targetRigidbody) return;

        float speed = targetRigidbody.linearVelocity.magnitude;
        float t = Mathf.InverseLerp(0f, maxSpeedForZoom, speed);

        float desiredFOV = Mathf.Lerp(minZoom * 10f, maxZoom * 10f, t);

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, desiredFOV, zoomSmoothSpeed * Time.deltaTime );
    }
}