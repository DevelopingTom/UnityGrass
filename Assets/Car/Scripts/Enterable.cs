using UnityEngine;
using System.Collections;
using KinematicCharacterController.Examples;
using System;

public class Enterable : MonoBehaviour
{
    public Transform exitPosition;
    public Transform seatPosition;
    public Vector3 seatingOffset;
  
    private GameObject player;
    private Transform mesh;
    public float enterDuration = 1.0f; // Duration for entering the vehicle
  
    private Boolean _isIn = false;
    public Boolean Entered
    {
        get => _isIn;
        set
        {
            _isIn = value;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            Exit();
        }
    }

    IEnumerator Enter(GameObject newDriver)
    {
        Entered = true;
        player = newDriver;
        player.GetComponentInChildren<KinematicCharacterController.Examples.ExampleCharacterController>().TransitionToState(CharacterState.Sitting);
        mesh = player.transform.Find("Root");
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
        player.SetActive(false);
    }

    void Exit()
    {
        player.GetComponentInChildren<KinematicCharacterController.Examples.ExampleCharacterController>().TransitionToState(CharacterState.Default);
        player.SetActive(true);
        player.transform.position = exitPosition.position;
        mesh.parent = player.transform;
        mesh.localPosition = new Vector3(0, 0, 0);
        mesh.localRotation = Quaternion.identity;
        Entered = false;
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
            StartCoroutine(Enter(other.gameObject));
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
