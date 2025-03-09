using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : FloatController
{   
    private float moveInput;
    private float steerInput;
    public float moveSpeed = 10f;
    public float turnSpeed = 5f;
    public bool isEntered { get; private set; }

    protected new void Start()
    {
        base.Start();
    }
    
    private void Update()
    {
        Enterable enterable = GetComponent<Enterable>();

        if (enterable != null && enterable.Entered)
        {
            GetPlayerInput();
            isEntered = true;
        }
        else
        {
            isEntered = false;
        }
    }
    
    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        // Handle boat movement
        HandleMovement();
    }


    #region Input Handling
        private void GetPlayerInput()
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
        }
    #endregion
    
    void HandleMovement()
    {
        if (isEntered)
        {
            Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
            Rb.AddForce(moveDirection, ForceMode.Acceleration);

            float turn = steerInput * turnSpeed;
            Rb.AddTorque(0f, turn, 0f, ForceMode.Acceleration);
        }
    }
}