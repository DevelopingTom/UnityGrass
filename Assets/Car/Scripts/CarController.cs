using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : WheelVehicleBehaviour
{

    [Header("References")]
    [SerializeField] private Transform accelerationPoint;
    [SerializeField] public Light[] lights;
    [SerializeField] public Material emissionSourceMaterial;

    
    [Header("Car Settings")]
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private AnimationCurve powerCurve;
    [SerializeField] private float jumpForce = 800f;

    [Header("Fuel Settings")]
    public GameObject fuelBar;
    [SerializeField] private float fuel = 1f;
    [SerializeField] private float fuelCapacity = 1f;
    [SerializeField] private float fuelConsumptionRate = 0.01f;

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;
    private bool isEntered = false;

    override public void Start()
    {
        base.Start();
        if (fuelBar != null)
        {
            fuelCapacity = fuelBar.GetComponent<Renderer>().material.GetFloat("_Level");
            fuel = fuelCapacity;
        }
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

        if (fuelBar != null && isEntered && fuel > -fuelCapacity && moveInput > 0)
        {
            ConsumeFuel();
        }
    }

    private void LightToggle()
    {
        foreach (var light in lights)
        {
            light.enabled = !light.enabled;
        }

        if (emissionSourceMaterial.IsKeywordEnabled("_EMISSION")) 
        {
            emissionSourceMaterial.DisableKeyword("_EMISSION");
        } else 
        {
            emissionSourceMaterial.EnableKeyword("_EMISSION");
        }
    }
    
    #region Input Handling
        private void GetPlayerInput()
        {
            moveInput = Input.GetAxis("Vertical");
            steerInput = Input.GetAxis("Horizontal");
            if (Input.GetKeyDown(KeyCode.R))
            {
                LightToggle();
            }
        }
    #endregion
    
    #region Movement

        private void Jump()
        {
            if (isGrounded) 
            {
                carRBody.AddForceAtPosition(jumpForce * transform.up, accelerationPoint.position, ForceMode.Acceleration);
            }
        }
        
        override protected void Movement() 
        {
            if (wheelsAreGrounded[2] + wheelsAreGrounded[3] > 0) 
            {
                for (int i = 0; i < rayPoints.Length; i++)
                {
                    if (isEntered)
                    {
                        Acceleration(rayPoints[i]);
                    }            
                    InverseDrag(rayPoints[i].forward, rayPoints[i], 1f);
                } 
            }
            base.Movement();
            Turn();

        }
        private void Acceleration(Transform tireTransform)
        {
            if (fuel <= -fuelCapacity) return; // Stop acceleration if out of fuel
            float carSpeed = Vector3.Dot(transform.forward, carRBody.linearVelocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / maxSpeed);
            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * moveInput * carRBody.mass * 5;
            
            if (carSpeed - maxSpeed < 0.1f) 
            {
                carRBody.AddForceAtPosition(tireTransform.forward * availableTorque, tireTransform.position);
            }
        }

        override protected void Turn()
        {
            for (int i = 0; i < 2; i++)
            {
                rayPoints[i].localRotation = Quaternion.Euler(0, Mathf.Clamp(steerInput * steerStrength * turningCurve.Evaluate(carVelocityRatio) , -45, 45), 0);
            }
        }

        
    #endregion

    private void ConsumeFuel()
    {
        fuel -= fuelConsumptionRate * Time.deltaTime;
        fuel = Math.Max(-fuelCapacity, fuel);
        fuelBar.GetComponent<Renderer>().material.SetFloat("_Level", fuel);
    }

}
