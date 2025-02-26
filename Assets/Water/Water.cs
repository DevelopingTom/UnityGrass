using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter water");
        if (other.CompareTag("Boat"))
        {
            Debug.Log("Boat entered water");
            BoatController boat = other.GetComponent<BoatController>();
            if (boat != null)
            {
                boat.SetInWater(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            BoatController boat = other.GetComponent<BoatController>();
            if (boat != null)
            {
                boat.SetInWater(false);
            }
        }
    }
}