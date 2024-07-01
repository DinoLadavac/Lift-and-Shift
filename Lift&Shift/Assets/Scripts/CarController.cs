using UnityEngine;

public class CarController : MonoBehaviour
{
    public float maxMoveSpeed = 25f;
    public float turnSpeed = 50f;
    public float grassSlowdownFactor = 0.5f;
    public float acceleration = 3f;
    public float deceleration = 3f;
    public float brakeStrength = 7f;
    public float minTurnSpeedFactor = 0.7f; // Minimum factor for turning speed

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;
    private float currentSpeed;
    private float targetSpeed;
    private bool onGrass = false;
    private bool inputEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = 0f;
    }

    void Update()
    {
        if (inputEnabled)
        {
            // Get input from the user
            moveInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");

            // Determine target speed based on input and grass slowdown
            float adjustedMoveSpeed = onGrass ? maxMoveSpeed * grassSlowdownFactor : maxMoveSpeed;
            targetSpeed = moveInput * adjustedMoveSpeed;
        }
    }

    void FixedUpdate()
    {
        // Gradually adjust current speed towards target speed
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Apply braking
            if (currentSpeed > 0)
            {
                currentSpeed -= brakeStrength * Time.deltaTime;
                if (currentSpeed < 0)
                {
                    currentSpeed = 0;
                }
            }
            else if (currentSpeed < 0)
            {
                currentSpeed += brakeStrength * Time.deltaTime;
                if (currentSpeed > 0)
                {
                    currentSpeed = 0;
                }
            }
        }
        else if (currentSpeed < targetSpeed)
        {
            currentSpeed += acceleration * Time.deltaTime;
            if (currentSpeed > targetSpeed)
            {
                currentSpeed = targetSpeed;
            }
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed < targetSpeed)
            {
                currentSpeed = targetSpeed;
            }
        }

        // Move the car forward or backward
        Vector3 forwardMovement = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        // Adjust turn speed based on current speed
        float speedFactor = Mathf.Clamp(Mathf.Abs(currentSpeed) / maxMoveSpeed, minTurnSpeedFactor, 1f);
        float adjustedTurnSpeed = turnSpeed * speedFactor;

        // Only allow turning if the car is moving
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turn = turnInput * adjustedTurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the car is on grass
        if (other.CompareTag("Grass"))
        {
            onGrass = true;
            Debug.Log("Car is on grass");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the car has left the grass
        if (other.CompareTag("Grass"))
        {
            onGrass = false;
            Debug.Log("Car has left the grass");
        }
    }
}
