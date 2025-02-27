using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boat"))
        {
            BoatController boat = other.GetComponent<BoatController>();
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
            BoatController boat = other.GetComponent<BoatController>();
            if (boat != null)
            {
                boat.SetWater(null);
            }
        }
    }
}