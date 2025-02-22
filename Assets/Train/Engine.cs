using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Engine : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 dir = 6500 * transform.forward;
        rb.AddForce(dir);
    }

    private void FixedUpdate()
    {
        // if (Input.GetKey(KeyCode.W))
        // {
        //     Throttle(power);
        // }
        
        // if (Input.GetKey(KeyCode.S))
        // {
        //     Throttle(-power);
        // }
    }

    private void Throttle(float power)
    {
        Vector3 dir = power * transform.forward;
        rb.AddForce(dir);
    }
}