using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private float buoyancyForce = 10f;
    [SerializeField] private Transform[] buoyancyPoints;

    private Rigidbody rb;
    private bool inWater = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        if (inWater)
        {
            HandleBuoyancy();
        }
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.AddForce(moveDirection, ForceMode.Acceleration);

        float turn = turnInput * turnSpeed;
        rb.AddTorque(0f, turn, 0f, ForceMode.Acceleration);
    }

    private void HandleBuoyancy()
    {
        foreach (Transform point in buoyancyPoints)
        {
            if (point.position.y < 0f)
            {
                float forceFactor = Mathf.Clamp01(-point.position.y);
                Vector3 uplift = -Physics.gravity * (forceFactor * buoyancyForce);
                rb.AddForceAtPosition(uplift, point.position, ForceMode.Acceleration);
            }
        }
    }

    public void SetInWater(bool isInWater)
    {
        inWater = isInWater;
    }
}