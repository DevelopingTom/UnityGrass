using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using KinematicCharacterController.Examples;
using System;
using KinematicCharacterController;

public class Enterable : MonoBehaviour
{
    public Transform seatPosition;

    private GameObject player;
    private Transform mesh;
    public float enterDuration = 1.0f; // Duration for entering the vehicle
    private bool isPlayerInTrigger = false;
    private Boolean _isIn = false;
    public UnityEvent enterEvent;
    public UnityEvent exitEvent;

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
        player.GetComponentInChildren<ExampleCharacterController>().TransitionToState(CharacterState.Sitting);
        mesh = player.transform.Find("Root");
        if (mesh != null) {
            mesh.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

            float elapsedTime = 0;

            while (elapsedTime < enterDuration)
            {         
                Vector3 endPos = seatPosition.position;
                Quaternion endRot = seatPosition.rotation;
                mesh.position = Vector3.Lerp(startPos, endPos, elapsedTime / enterDuration);
                mesh.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime / enterDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            mesh.position = seatPosition.position;
            mesh.rotation = seatPosition.rotation;
            mesh.parent = seatPosition;
        }
        player.SetActive(false);
        enterEvent.Invoke();
    }

    void Exit()
    {
        if (player != null)
        {
            player.SetActive(true);
            var characterMotor = player.GetComponentInChildren<KinematicCharacterMotor>();
            characterMotor.SetPosition(seatPosition.position + Vector3.up / 2);
            characterMotor.SetRotation(seatPosition.rotation);
            mesh.position = player.transform.position;
            mesh.parent = player.transform;
            mesh.localPosition = Vector3.zero;
            mesh.localRotation = Quaternion.identity;
            player.GetComponentInChildren<ExampleCharacterController>().TransitionToState(CharacterState.Jumping);
            player = null;
            Entered = false;
            exitEvent.Invoke();
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
        if (isPlayerInTrigger && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E) && player == null)
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
