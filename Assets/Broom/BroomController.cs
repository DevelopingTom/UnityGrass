using UnityEngine;

public class BroomController : MonoBehaviour
{
    public float floatHeight = 2.0f; // Height at which the broom floats
    public float floatForce = 10.0f; // Force applied to maintain floating
    public float moveSpeed = 5.0f; // Speed of movement
    public float maxSpeed = 10.0f; // Speed of movement
    public float dragCoeff = 0.2f; // Speed of movement
    public float rotationSpeed = 100.0f; // Speed of rotation
    public float maxPitchAngle = 30.0f; // Maximum angle to rotate up and down

    private float moveInput = 0;
    private float steerInput = 0;
    private Enterable enterable;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure the Rigidbody is set up for floating
        rb.useGravity = false;
        enterable = GetComponent<Enterable>();
    }

    void FixedUpdate()
    {
        HandleFloating();
        HandleMovement();
        RotateTowardsLookDirection(); // Rotate towards look direction
        // RotateTowardsMovement(); // Rotate towards movement direction
        // SlowDownIfNotEntered(); // Slow down when not entered
        LimitMaxSpeed(); // Enforce max speed
        ApplyDragForce(); // Apply drag force
        if (moveInput == 0 && steerInput == 0)
        {
            StabilizeRotation(); // Stabilize rotation
        } else
        {
        }
    }

    void Update()
    {
        
        Enterable enterable = GetComponent<Enterable>();

        if (enterable != null && enterable.Entered)
        {
            GetPlayerInput();
        }
    }
    
    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void HandleFloating()
    {
        // Apply an upward force to maintain the float height
        float heightDifference = floatHeight - transform.position.y;
        if (heightDifference > 0)
        {
            Vector3 floatForceVector = Vector3.up * heightDifference * floatForce;
            rb.AddForce(floatForceVector, ForceMode.Acceleration);
        }
    }

    private void HandleMovement()
    {
        // Get input for movement

        // Move the broomstick forward/backward and sideways
        Vector3 movement = transform.forward * moveInput + transform.right * (steerInput * 0.5f);
        rb.AddForce(movement * moveSpeed, ForceMode.Acceleration);

        // Rotate the broomstick based on input
        float rotation = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up, rotation * rotationSpeed * Time.deltaTime);
    }

    private void StabilizeRotation()
    {
        // Gradually align the broomstick to a stable horizontal orientation
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * (rotationSpeed / 2f));
    }

    private void RotateTowardsLookDirection()
    {
        
        if (enterable != null && enterable.Entered)
        {
            // Get the camera's forward direction
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 lookDirection = mainCamera.transform.forward;
                // lookDirection.y = 0; // Keep the rotation horizontal
                if (lookDirection.sqrMagnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }
        // if (enterable != null && enterable.Entered)
        // {
        //     // Get the camera's forward direction
        //     Camera mainCamera = Camera.main;
        //     if (mainCamera != null)
        //     {
        //         Vector3 lookDirection = mainCamera.transform.forward;
        //         float pitchAngle = Vector3.Angle(Vector3.ProjectOnPlane(lookDirection, Vector3.right), Vector3.forward);

        //         // Clamp the pitch angle
        //         if (lookDirection.y > 0 && pitchAngle > maxPitchAngle)
        //         {
        //             lookDirection.y = Mathf.Tan(Mathf.Deg2Rad * maxPitchAngle);
        //         }
        //         else if (lookDirection.y < 0 && pitchAngle > maxPitchAngle)
        //         {
        //             lookDirection.y = -Mathf.Tan(Mathf.Deg2Rad * maxPitchAngle);
        //         }

        //         if (lookDirection.sqrMagnitude > 0.1f)
        //         {
        //             Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        //             transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        //         }
        //     }
        // }
    }

    private void RotateTowardsMovement()
    {
        if (steerInput != 0 && enterable != null && enterable.Entered)
        {
            // Calculate the direction of the broomstick's velocity
            Vector3 velocity = rb.linearVelocity;
            if (velocity.magnitude > 0.1f) // Only rotate if the broomstick is moving
            {
                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * (rotationSpeed / 2f));
            }
        }
    }

    private void LimitMaxSpeed()
    {
        // Clamp the velocity to the maximum speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void ApplyDragForce()
    {
        // Apply a drag force proportional to the velocity
        Vector3 dragForce = -rb.linearVelocity * dragCoeff;
        rb.AddForce(dragForce, ForceMode.Acceleration);
    }
}
