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
    private Quaternion containerRotationOffset;  // Offset to maintain the container's original rotation
    private bool isHookColliding = false;  // Flag to indicate if the hook is colliding

    // Offset for detaching the container
    public float detachOffset = 1.0f;  // Adjust this value as needed
    public float detectionRadius = 9f;

    private void Start()
    {
        initialHookLocalPosition = hook.localPosition;
    }

    private void Update()
    {
        HandleInput();
        if (isMagnetOn && attachedContainer != null)
        {
            attachedContainer.position = hookAttachmentPoint.position; // Ensure the container is exactly at the hook's attachment point
            attachedContainer.rotation = hookAttachmentPoint.rotation * containerRotationOffset; // Ensure the container maintains its original rotation
        }
    }

    private void HandleInput()
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

        if (Input.GetKey(KeyCode.Q) && !isHookColliding)
        {
            MoveHook(Vector3.down);
        }
        if (Input.GetKey(KeyCode.E) && !isHookColliding)
        {
            MoveHook(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleMagnet();
        }
    }

    private void MoveMovingPart(Vector3 direction)
    {
        Vector3 newPosition = movingPart.position + craneArmPivot.TransformDirection(direction * movingPartSpeed * Time.deltaTime);
        Vector3 localPosition = craneArmPivot.InverseTransformPoint(newPosition);

        if (localPosition.x >= movingPartMinLimit && localPosition.x <= movingPartMaxLimit)
        {
            movingPart.position = newPosition;
        }
    }

    private void MoveHook(Vector3 direction)
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

    private void ToggleMagnet()
    {
        isMagnetOn = !isMagnetOn;

        if (isMagnetOn)
        {
            // Use the hook position and set the detection radius based on the hook's size
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
                containerRotationOffset = Quaternion.Inverse(hookAttachmentPoint.rotation) * attachedContainer.rotation; // Calculate the offset to maintain the container's original rotation
                attachedContainer.SetParent(hookAttachmentPoint, true); // Make the container a child of the hook attachment point and maintain world space orientation
                attachedContainer.localPosition = Vector3.zero; // Ensure the container is exactly at the hook's attachment point
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
                attachedContainer.SetParent(null); // Unparent the container
                attachedContainer.position = hookAttachmentPoint.position + Vector3.down * detachOffset; // Lower the container upon detaching
                attachedContainer = null;
                Debug.Log("Magnet turned off and container detached.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container") || collision.gameObject.CompareTag("Road"))
        {
            isHookColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container") || collision.gameObject.CompareTag("Road"))
        {
            isHookColliding = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isMagnetOn ? Color.red : Color.green;
        // Draw detection sphere at hook position
        Gizmos.DrawWireSphere(hook.position, detectionRadius);
    }
}
