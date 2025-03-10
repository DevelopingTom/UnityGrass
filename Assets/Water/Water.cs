using UnityEngine;
using KinematicCharacterController.Examples;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            FloatController boat = other.GetComponent<FloatController>();
            if (boat != null)
            {
                boat.SetWater(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            FloatController boat = other.GetComponent<FloatController>();
            if (boat != null)
            {
                boat.SetWater(null);
            }
        }
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<ExampleCharacterController>().TransitionToState(CharacterState.Jumping);
        }
    }
}