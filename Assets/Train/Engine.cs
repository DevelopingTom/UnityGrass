using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float initSpeed = 0;
    [SerializeField] private float dragForce = 0.1f; // A


    [Header("Input")]
    private float moveInput = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 dir = initSpeed * transform.forward;
        rb.AddForce(dir);
    }

    private void Update()
    {
        
        Enterable enterable = GetComponent<Enterable>();

        if (enterable != null && enterable.Entered)
        {
            GetPlayerInput();
        }
    }
    
    #region Input Handling
        private void GetPlayerInput()
        {
            moveInput = Input.GetAxis("Vertical");
        }
    #endregion

    private void FixedUpdate()
    {
        Throttle(moveInput * power);
        
        if (moveInput == 0)
        {
            rb.linearVelocity *= 1 - dragForce * Time.fixedDeltaTime;
        }
    }

    private void Throttle(float power)
    {
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            Vector3 dir = power * transform.forward;
            rb.AddForce(dir);
        }
    }
}