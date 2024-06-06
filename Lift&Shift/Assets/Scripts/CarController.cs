using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 20f;
    public float turnSpeed = 50f; // Slow down factor on grass
    private float originalSpeed;
    private Rigidbody rb;
    private bool onGrass = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalSpeed = speed;
    }

    void FixedUpdate()
    {
         float currentSpeed = originalSpeed;
        if(onGrass == true)
        {
            speed = 5f;
        }
        else
        {
            speed = 20f;
        }
       
        float moveForward = Input.GetAxis("Vertical") * currentSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        rb.MovePosition(rb.position + transform.forward * moveForward);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0, turn, 0)));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            onGrass = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            onGrass = false;
        }
    }
}