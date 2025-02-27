using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour
{
    public Transform[] Floaters;
    public float UnderWaterDrag = 3f;
    public float UnderWaterAngularDrag = 1f;
    public float AirDrag = 0f;
    public float AirAngularDrag = 0.05f;
    public float FloatingPower = 15f;
    public float WaterHeight = 0f;
    public Vector3 forcePosition;
    public GameObject water;
    public float moveSpeed = 10f;
    public float turnSpeed = 5f;
    private Rigidbody Rb;
    private bool Underwater;
    private int FloatersUnderWater;
    private float moveInput;
    private float steerInput;

    public bool isEntered { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Rb = GetComponent<Rigidbody>();
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
    public void SetWater(GameObject water)
    {
        if (water != null) 
        {
            WaterHeight = water.transform.position.y;
        }
        else
        {
            WaterHeight = 0;
        }
        this.water = water;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FloatersUnderWater = 0;
        for(int i = 0; i < Floaters.Length; i++)
        {
            float diff = Floaters[i].position.y - WaterHeight;
            if (diff < 0)
            {
                Rb.AddForceAtPosition(Vector3.up * FloatingPower * Mathf.Abs(diff), Floaters[i].position, ForceMode.Force);
                FloatersUnderWater += 1;
                if (!Underwater)
                {
                    Underwater = true;
                    SwitchState(true);
                }
            }
        }
        if (Underwater && FloatersUnderWater == 0)
        {
            Underwater = false;
            SwitchState(false);
        }

        // Handle boat movement
        HandleMovement();
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            Rb.linearDamping  = UnderWaterDrag;
            Rb.angularDamping  = UnderWaterAngularDrag;
        }
        else
        {
            Rb.linearDamping  = AirDrag;
            Rb.angularDamping  = AirAngularDrag;
        }
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