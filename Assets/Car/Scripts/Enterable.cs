using UnityEngine;
using System.Collections;
using KinematicCharacterController.Examples;
using System;

public class Enterable : MonoBehaviour
{
    public Transform seatPosition;
    public Vector3 seatingOffset;
  
    private Transform player;
    private Transform mesh;
    private Boolean isIn = false;
    
    public float enterDuration = 1.0f; // Duration for entering the vehicle
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Boolean isEntered() {
        return isIn;
    }
    
    IEnumerator Enter(Transform newDriver)
    {
        isIn = true;
        player = newDriver;
        player.GetComponentInChildren<KinematicCharacterController.Examples.ExampleCharacterController>().TransitionToState(CharacterState.Sitting);
        mesh = player.Find("Root");
        if (mesh != null) {
            mesh.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            Vector3 endPos = seatPosition.position;
            Quaternion endRot = seatPosition.rotation;

            float elapsedTime = 0;

            while (elapsedTime < enterDuration)
            {
                mesh.position = Vector3.Lerp(startPos, endPos, elapsedTime / enterDuration);
                mesh.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / enterDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mesh.position = endPos;
            mesh.rotation = endRot;
            mesh.parent = seatPosition;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Display UI or message to indicate that player can enter the vehicle
            Debug.Log("Press 'E' to enter the vehicle");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && player == null)
        {
            StartCoroutine(Enter(other.transform));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide UI or message
            Debug.Log("You left the vehicle area");
        }
    }

}
