using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    public Transform craneArmPivot;
    public Transform movingPart;
    public Transform hook;  // This should be the visible part of the hook/magnet
    public Transform hookCable;  // Reference to the hook cable
    public LayerMask containerLayer; // Layer for detecting containers
    public Transform hookAttachmentPoint;  // The point where the container should attach, at the bottom of the hook

    public float armRotationSpeed = 20f;
    public float movingPartSpeed = 5f;
    public float hookMovementSpeed = 2f;

    public float movingPartMinLimit = -5f;
    public float movingPartMaxLimit = 5f;
    public float hookMinHeight = -8.7f;  // Minimum height for the hook
    public float hookMaxHeight = 1f;  // Maximum height for the hook

    private Vector3 initialHookLocalPosition;
    private bool isMagnetOn = false;  // State of the magnet
    private Transform attachedContainer = null;  // Reference to the attached container

    // Offset for detaching the container
    public float detachOffset = 1.0f;  // Adjust this value as needed
    public float detectionRadius = 9f;

    void Start()
    {
        initialHookLocalPosition = hook.localPosition;
    }

    void Update()
    {
        HandleInput();
        if (isMagnetOn && attachedContainer != null)
        {
            attachedContainer.position = hookAttachmentPoint.position; // Ensure the container is exactly at the hook's attachment point
        }
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            craneArmPivot.Rotate(Vector3.up, -armRotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            craneArmPivot.Rotate(Vector3.up, armRotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            MoveMovingPart(Vector3.right);
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveMovingPart(Vector3.left);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            MoveHook(Vector3.down);
        }
        if (Input.GetKey(KeyCode.E))
        {
            MoveHook(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleMagnet();
        }
    }

    void MoveMovingPart(Vector3 direction)
    {
        Vector3 newPosition = movingPart.position + craneArmPivot.TransformDirection(direction * movingPartSpeed * Time.deltaTime);
        Vector3 localPosition = craneArmPivot.InverseTransformPoint(newPosition);

        if (localPosition.x >= movingPartMinLimit && localPosition.x <= movingPartMaxLimit)
        {
            movingPart.position = newPosition;
        }
    }

    void MoveHook(Vector3 direction)
    {
        Vector3 newPosition = hook.localPosition + direction * hookMovementSpeed * Time.deltaTime;

        // Clamp the hook position to stay within the height limits
        newPosition.y = Mathf.Clamp(newPosition.y, hookMinHeight, hookMaxHeight);

        hook.localPosition = newPosition;

        // Adjust the cable to maintain the connection between the crane arm and the hook
        float distance = Vector3.Distance(hook.localPosition, initialHookLocalPosition);
        hookCable.localScale = new Vector3(hookCable.localScale.x, distance / 2, hookCable.localScale.z);
        hookCable.localPosition = initialHookLocalPosition + Vector3.down * (distance / 2);
    }

    void ToggleMagnet()
    {
        isMagnetOn = !isMagnetOn;

        if (isMagnetOn)
        {
            // Use the hook position and set the detection radius based on the hook's size

            // Adjust position with offset based on hook's scale
            Vector3 detectionPosition = hook.position;

            // Try to attach a nearby container
            Collider[] colliders = Physics.OverlapSphere(detectionPosition, detectionRadius, containerLayer);
            Debug.Log("Checking for containers in range. Found: " + colliders.Length);
            foreach (Collider col in colliders)
            {
                Debug.Log("Found container: " + col.name);
            }
            if (colliders.Length > 0)
            {
                attachedContainer = colliders[0].transform;
                attachedContainer.GetComponent<Rigidbody>().isKinematic = true;
                attachedContainer.position = hookAttachmentPoint.position; // Ensure the container is exactly at the hook's attachment point
                Debug.Log("Magnet turned on and container attached.");
            }
            else
            {
                Debug.Log("No container in range to attach.");
            }
        }
        else
        {
            // Detach the container
            if (attachedContainer != null)
            {
                attachedContainer.GetComponent<Rigidbody>().isKinematic = false;
                attachedContainer.position = hookAttachmentPoint.position + Vector3.down * detachOffset; // Lower the container upon detaching
                attachedContainer = null;
                Debug.Log("Magnet turned off and container detached.");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isMagnetOn ? Color.red : Color.green;
        // Use the hook position and set the detection radius based on the hook's size
        float detectionRadius = hook.localScale.x / 2;  // Assuming the hook's scale x represents its size
        // Draw detection sphere at hook position
        Gizmos.DrawWireSphere(hook.position, detectionRadius);
    }
}
