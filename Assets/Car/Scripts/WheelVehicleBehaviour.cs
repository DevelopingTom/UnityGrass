using Unity.VisualScripting;
using UnityEngine;

public abstract class WheelVehicleBehaviour : MonoBehaviour
{

    [Header("References")]
    [SerializeField] protected LayerMask drivable;
    [SerializeField] protected Transform[] rayPoints;   
    [SerializeField] protected Transform[] wheels;
    [SerializeField] protected Transform[] tires;
    [SerializeField] protected ParticleSystem[] dustParticleSystems;
    
    [Header("Suspension Settings")]
    [SerializeField] protected float springStiffness;
    [SerializeField] protected float restLength;
    [SerializeField] protected float springTravel;
    [SerializeField] protected float wheelRadius;
    [SerializeField] protected float damperStiffness;

    
    [Header("Car Settings")]
    [SerializeField] protected float maxSpeed = 100f;
    [SerializeField] protected float dragCoefficient = 1f;
    [SerializeField] protected float wheelWeight = 50f;
    
    protected Rigidbody carRBody;
    protected int[] wheelsAreGrounded;
    protected bool isGrounded = false;
    protected Vector3 currentCarLocalVelocity = Vector3.zero;
    protected float carVelocityRatio = 0;
    
    public virtual void Start()
    {
        carRBody = GetComponent<Rigidbody>();
        wheelsAreGrounded = new int[rayPoints.Length];
    }

    private void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    public virtual void Suspension() 
    {
        for (int i = 0; i < rayPoints.Length; i++) 
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;


            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable)) 
            {
                tires[i].position = GetPositionAlongLine(hit.point, rayPoints[i].position, wheelRadius);
                wheelsAreGrounded[i] = 1;
                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRBody.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;
                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                
                carRBody.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);


                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
                Debug.DrawLine(tires[i].position, hit.point, Color.yellow);

            } else 
            {
                wheelsAreGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }

    protected virtual Vector3 InverseDrag(Vector3 steeringDir, Transform tireTransform, float ratio)
    {            
        Vector3 tireWorldVel = carRBody.GetPointVelocity(tireTransform.position);
        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
        float desiredVelChange = -steeringVel * dragCoefficient;
        float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
        Vector3 dragForce = steeringDir * ratio * desiredAccel;
        carRBody.AddForceAtPosition(dragForce, tireTransform.position);
        return dragForce;
    }


    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRBody.linearVelocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }

    private void HandleDust(ParticleSystem dustParticleSystem, Vector3 dragForce)
    {
        var emissionL = dustParticleSystem.emission;
        if (dragForce.magnitude > 25000f && carRBody.linearVelocity.magnitude > 0.1f) // Emit dust when the car is moving
        {
            emissionL.rateOverTime = (dragForce.magnitude - 25000f) / 200; // Adjust this value as needed
        }
        else
        {
            emissionL.rateOverTime = 0;
        }
    }

    private Vector3 GetPositionAlongLine(Vector3 start, Vector3 end, float distance)
    {
        Vector3 direction = (end - start).normalized;
        return start + direction * distance;
    }

    protected virtual void GroundCheck()
    {
        int tempGroundedWheels = 0;
        for (int i = 0; i < wheelsAreGrounded.Length; i++)
        {
            tempGroundedWheels += wheelsAreGrounded[i];
        }

        isGrounded = tempGroundedWheels > 1;
    }


    protected abstract void Turn();
    protected virtual void Movement() 
    {

        for (int i = 0; i < rayPoints.Length; i++)
        {
            if (wheelsAreGrounded[i] > 0) {
                Vector3 dragForce = InverseDrag(rayPoints[i].right, rayPoints[i], wheelWeight);
                if (dustParticleSystems.Length > i) 
                {
                    HandleDust(dustParticleSystems[i], dragForce);
                }
            }
        }
        RotateWheels();
    }
    
    protected virtual void RotateWheels()
    {
        
        float speed = carRBody.linearVelocity.magnitude;        
        float direction = Vector3.Dot(carRBody.linearVelocity, transform.forward) >= 0 ? 1 : -1; // Forward or backward
        float rotationAngle = direction * speed * (360 / (2 * Mathf.PI * wheelRadius)) * Time.deltaTime;

        for (int i = 0; i < wheels.Length; i++)
        {
            if (i % 2 == 0) // Left wheels (assuming left wheels are at even indices)
            {
                wheels[i].Rotate(Vector3.forward, -rotationAngle);
            }
            else // Right wheels
            {
                wheels[i].Rotate(Vector3.forward, rotationAngle);
            }
        }
    }
}
