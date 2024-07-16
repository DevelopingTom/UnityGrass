using UnityEngine;
using KinematicCharacterController.Examples;

public class Enterable : MonoBehaviour
{
    public Transform player;
    public Transform seatPosition;
    public Vector3 seatingOffset;
    public float transitionSpeed;
  
    private Transform mesh;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Enter(Transform newDriver)
    {
        player = newDriver;
        player.GetComponentInChildren<KinematicCharacterController.Examples.ExampleCharacterController>().TransitionToState(CharacterState.Sitting);
        mesh = player.Find("Root"); 
        mesh.position = Vector3.Lerp(player.position, seatPosition.position + seatingOffset, transitionSpeed);
        mesh.rotation = Quaternion.Slerp(player.rotation, seatPosition.rotation, transitionSpeed);
        mesh.parent = seatPosition;
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
            Enter(other.transform);
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
