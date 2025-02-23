using UnityEngine;
using System.Collections;
using KinematicCharacterController.Examples;
using System;
using KinematicCharacterController;

public class Enterable : MonoBehaviour
{
    public Transform seatPosition;
    public Vector3 seatingOffset;

    private GameObject player;
    private Transform mesh;
    public float enterDuration = 1.0f; // Duration for entering the vehicle
    private bool isPlayerInTrigger = false;
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
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && player == null)
        {
            StartCoroutine(Enter(GameObject.FindWithTag("Player")));
        }

        if (Input.GetKeyDown(KeyCode.Space) && player != null)
        {
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
        if (player != null)
        {
            player.SetActive(true);
            var characterMotor = player.GetComponentInChildren<KinematicCharacterMotor>();
            characterMotor.SetPosition(seatPosition.position + Vector3.up / 2);
            characterMotor.SetRotation(seatPosition.rotation);
            mesh.localPosition = Vector3.zero;
            mesh.localRotation = Quaternion.identity;
            mesh.position = player.transform.position;
            mesh.parent = player.transform;
            player.GetComponentInChildren<KinematicCharacterController.Examples.ExampleCharacterController>().TransitionToState(CharacterState.Jumping);
            player = null;
            Entered = false;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
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
            isPlayerInTrigger = false;
            // Hide UI or message
            Debug.Log("You left the vehicle area");
        }
    }

}
