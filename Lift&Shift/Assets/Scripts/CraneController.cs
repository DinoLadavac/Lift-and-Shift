using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    public Transform craneArmPivot;
    public Transform movingPart;
    public Transform hook;  // Reference to the current hook/magnet
    public Transform hookCable;  // Reference to the hook cable
    public LayerMask containerLayer; // Layer for detecting containers
    public LayerMask vehicleLayer; // Layer for detecting vehicles
    public Transform hookAttachmentPoint;  // The point where the container should attach, at the bottom of the hook
    public Transform vehicleHookAttachmentPoint;  // The point where the vehicle should attach, at the bottom of the vehicle hook

    public Transform standardHook;  // Reference to the standard hook
    public Transform vehicleHook;  // Reference to the vehicle hook

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

    private bool canMoveDown = true;
    private bool canMoveUp = true;
    private bool canMoveLeft = true;
    private bool canMoveRight = true;
    private bool canRotateLeft = true;
    private bool canRotateRight = true;

    // Offset for detaching the container
    public float detachOffset = 1.0f;  // Adjust this value as needed
    public float detectionRadius = 9f;
    public float collisionDetectionRadius = 2f;  // Radius for the collision detection sphere
    public float collisionDetectionRadiusWithContainer = 3f;  // Radius for the collision detection sphere with a container attached

    private void Start()
    {
        initialHookLocalPosition = hook.localPosition;
    }

    private void Update()
    {
        HandleInput();
        CheckCollisions();
        if (isMagnetOn && attachedContainer != null)
        {
            Transform currentAttachmentPoint = hook == standardHook ? hookAttachmentPoint : vehicleHookAttachmentPoint;
            attachedContainer.position = currentAttachmentPoint.position; // Ensure the container is exactly at the hook's attachment point
            attachedContainer.rotation = currentAttachmentPoint.rotation * containerRotationOffset; // Ensure the container maintains its original rotation
        }
    }

    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.A) && canRotateLeft)
        {
            craneArmPivot.Rotate(Vector3.up, -armRotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D) && canRotateRight)
        {
            craneArmPivot.Rotate(Vector3.up, armRotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) && canMoveRight)
        {
            MoveMovingPart(Vector3.right);
        }
        if (Input.GetKey(KeyCode.S) && canMoveLeft)
        {
            MoveMovingPart(Vector3.left);
        }

        if (Input.GetKey(KeyCode.Q) && canMoveDown)
        {
            MoveHook(Vector3.down);
        }
        if (Input.GetKey(KeyCode.E) && canMoveUp)
        {
            MoveHook(Vector3.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleMagnet();
        }

        if (Input.GetKeyDown(KeyCode.H))  // Key to switch hooks
        {
            SwitchHook();
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

            // Try to attach a nearby container or vehicle
            LayerMask currentLayer = hook == standardHook ? containerLayer : vehicleLayer;
            Collider[] colliders = Physics.OverlapSphere(detectionPosition, detectionRadius, currentLayer);
            Debug.Log("Checking for objects in range. Found: " + colliders.Length);
            foreach (Collider col in colliders)
            {
                Debug.Log("Found object: " + col.name);
            }
            if (colliders.Length > 0)
            {
                attachedContainer = colliders[0].transform;
                attachedContainer.GetComponent<Rigidbody>().isKinematic = true;
                Transform currentAttachmentPoint = hook == standardHook ? hookAttachmentPoint : vehicleHookAttachmentPoint;
                containerRotationOffset = Quaternion.Inverse(currentAttachmentPoint.rotation) * attachedContainer.rotation; // Calculate the offset to maintain the object's original rotation
                attachedContainer.SetParent(currentAttachmentPoint, true); // Make the object a child of the hook attachment point and maintain world space orientation
                attachedContainer.localPosition = Vector3.zero; // Ensure the object is exactly at the hook's attachment point
                Debug.Log("Magnet turned on and object attached.");
            }
            else
            {
                Debug.Log("No object in range to attach.");
            }
        }
        else
        {
            // Detach the object
            if (attachedContainer != null)
            {
                attachedContainer.GetComponent<Rigidbody>().isKinematic = false;
                attachedContainer.SetParent(null); // Unparent the object
                Transform currentAttachmentPoint = hook == standardHook ? hookAttachmentPoint : vehicleHookAttachmentPoint;
                attachedContainer.position = currentAttachmentPoint.position + Vector3.down * detachOffset; // Lower the object upon detaching
                attachedContainer = null;
                Debug.Log("Magnet turned off and object detached.");
            }
        }
    }

    private void SwitchHook()
    {
        if (attachedContainer != null)
        {
            Debug.Log("Cannot switch hook while an object is attached.");
            return; // Do not switch hooks if an object is attached
        }

        if (hook == standardHook)
        {
            SetActiveHook(vehicleHook);
        }
        else
        {
            SetActiveHook(standardHook);
        }
    }

    private void SetActiveHook(Transform newHook)
    {
        newHook.SetParent(hook.parent); // Make sure the new hook has the same parent as the old hook
        newHook.localPosition = hook.localPosition;
        newHook.localRotation = hook.localRotation;
        
        hook.gameObject.SetActive(false);
        hook = newHook;
        hook.gameObject.SetActive(true);
    }

    private void CheckCollisions()
    {
        float currentDetectionRadius = isMagnetOn && attachedContainer != null ? collisionDetectionRadiusWithContainer : collisionDetectionRadius;
        LayerMask currentLayer = hook == standardHook ? containerLayer : vehicleLayer;
        Collider[] colliders = Physics.OverlapSphere(hook.position, currentDetectionRadius, currentLayer);

        canMoveDown = true;
        canMoveUp = true;
        canMoveLeft = true;
        canMoveRight = true;
        canRotateLeft = true;
        canRotateRight = true;

        foreach (Collider col in colliders)
        {
            Vector3 directionToCollider = col.transform.position - hook.position;

            if (directionToCollider.y < 0) // Collider below
            {
                canMoveDown = false;
                Debug.Log("Collision detected below. Cannot move down.");
            }
            if (directionToCollider.x < 0) // Collider to the right
            {
                canRotateRight = false;
                canMoveRight = false;
                Debug.Log("Collision detected to the right. Cannot move right or rotate right.");
            }
            if (directionToCollider.x > 0) // Collider to the left
            {
                canRotateLeft = false;
                canMoveLeft = false;
                Debug.Log("Collision detected to the left. Cannot move left or rotate left.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container") || collision.gameObject.CompareTag("Road"))
        {
            Vector3 collisionDirection = collision.GetContact(0).normal;

            if (collisionDirection.y > 0) // Colliding from above
            {
                canMoveDown = false;
                Debug.Log("Colliding while lowering...");
            }
            if (collisionDirection.y < 0) // Colliding from below
            {
                canMoveUp = false;
                Debug.Log("Colliding while raising...");
            }
            if (collisionDirection.x < 0) // Colliding from the right
            {
                canMoveRight = false;
                Debug.Log("Colliding while moving right...");
            }
            if (collisionDirection.x > 0) // Colliding from the left
            {
                canMoveLeft = false;
                Debug.Log("Colliding while moving left...");
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container") || collision.gameObject.CompareTag("Road"))
        {
            canMoveDown = true;
            canMoveUp = true;
            canMoveLeft = true;
            canMoveRight = true;
            Debug.Log("Collision ended, can move freely.");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Container") || collision.gameObject.CompareTag("Road"))
        {
            Vector3 collisionDirection = collision.GetContact(0).normal;

            if (collisionDirection.y > 0) // Colliding from above
            {
                canMoveDown = false;
                Debug.Log("Still colliding while lowering...");
            }
            if (collisionDirection.y < 0) // Colliding from below
            {
                canMoveUp = false;
                Debug.Log("Still colliding while raising...");
            }
            if (collisionDirection.x < 0) // Colliding from the right
            {
                canMoveRight = false;
                Debug.Log("Still colliding while moving right...");
            }
            if (collisionDirection.x > 0) // Colliding from the left
            {
                canMoveLeft = false;
                Debug.Log("Still colliding while moving left...");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isMagnetOn ? Color.red : Color.green;
        // Draw detection sphere at hook position
        Gizmos.DrawWireSphere(hook.position, detectionRadius);

        float currentDetectionRadius = isMagnetOn && attachedContainer != null ? collisionDetectionRadiusWithContainer : collisionDetectionRadius;
        Gizmos.DrawWireSphere(hook.position, currentDetectionRadius);
    }
}
