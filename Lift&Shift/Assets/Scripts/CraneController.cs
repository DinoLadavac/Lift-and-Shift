using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    public Transform craneArmPivot;
    public Transform movingPart;
    public Transform hook;
    public Transform hookCable;  // Reference to the hook cable

    public float armRotationSpeed = 20f;
    public float movingPartSpeed = 5f;
    public float hookMovementSpeed = 2f;

    public float movingPartMinLimit = -5f;
    public float movingPartMaxLimit = 5f;
    public float hookMinHeight = -10f;  // Minimum height for the hook
    public float hookMaxHeight = 1f;  // Maximum height for the hook

    private Vector3 initialHookLocalPosition;

    void Start()
    {
        initialHookLocalPosition = hook.localPosition;
    }

    void Update()
    {
        HandleInput();
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
        hookCable.localScale = new Vector3(hookCable.localScale.x, distance/2 , hookCable.localScale.z);
        hookCable.localPosition = initialHookLocalPosition + Vector3.down * (distance / 2);
    }
}