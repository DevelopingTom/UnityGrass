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

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;
    private void Update()
    {
        GetPlayerInput();
       
    }

    override public void Start() {
        base.Start();
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
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
                    Acceleration(rayPoints[i]);
                    InverseDrag(rayPoints[i].forward, rayPoints[i], 1f);
                }            
            } 
            base.Movement();
            Turn();

        }
        private void Acceleration(Transform tireTransform)
        {
            float carSpeed = Vector3.Dot(transform.forward, carRBody.linearVelocity);
            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / maxSpeed);
            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * moveInput * 7000;
            
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

}
