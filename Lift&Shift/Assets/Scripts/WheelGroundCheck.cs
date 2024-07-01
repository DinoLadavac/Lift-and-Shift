using UnityEngine;

public class WheelGroundCheck : MonoBehaviour
{
    public Transform[] wheels;  // Array to hold the wheel transforms
    public float wheelRadius = 0.5f;  // Radius of the wheels
    private bool isGrounded;
    private CarController carController;
    private CraneController craneController;

    void Start()
    {
        carController = GetComponent<CarController>();
        craneController = GetComponent<CraneController>();
        if (craneController != null)
        {
            craneController.enabled = false;
        }

        if (carController == null)
        {
            Debug.LogError("CarController script not found on the car object.");
        }
    }

    void FixedUpdate()
    {
        isGrounded = false;

        foreach (Transform wheel in wheels)
        {
            RaycastHit hit;
            if (Physics.Raycast(wheel.position, -wheel.up, out hit, wheelRadius + 0.1f))
            {
                if (hit.collider.CompareTag("Road") || hit.collider.CompareTag("Grass") || hit.collider.CompareTag("Soil"))
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (isGrounded)
        {
            EnableCarMovement();
        }
        else
        {
            DisableCarMovement();
        }
    }

    void EnableCarMovement()
    {
        if (carController != null)
        {
            carController.SetInputEnabled(true);
        }
    }

    void DisableCarMovement()
    {
        if (carController != null)
        {
            Debug.Log("Disable car control");
            carController.SetInputEnabled(false);
        }
    }
}
