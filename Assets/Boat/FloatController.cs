using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatController : MonoBehaviour
{   
    public Transform[] Floaters;
    public float UnderWaterDrag = 3f;
    public float UnderWaterAngularDrag = 1f;
    public float AirDrag = 0f;
    public float AirAngularDrag = 0.05f;
    public float FloatingPower = 15f;
    public float WaterHeight = 0f;
    public GameObject water;
    protected Rigidbody Rb;
    private bool Underwater;
    protected Waves Waves;
    private int FloatersUnderWater;

    // Start is called before the first frame update
    protected void Start()
    {
        Rb = GetComponent<Rigidbody>();
        if (Floaters.Length == 0)
        {
            Floaters.Append(transform);
        }
    }

    public void SetWater(GameObject water)
    {
        if (water != null) 
        {
            WaterHeight = water.transform.position.y;
            Waves = water.GetComponent<Waves>();
        }
        else
        {
            Waves = null;
            WaterHeight = 0;
        }
        this.water = water;
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        FloatersUnderWater = 0;
        for(int i = 0; i < Floaters.Length; i++)
        {
            float diff = Floaters[i].position.y - (Waves != null ? Waves.GetAltitude(Floaters[i].position): 0);
            if (diff < 0)
            {
                Rb.AddForceAtPosition(Vector3.up * FloatingPower * Mathf.Abs(diff), Floaters[i].position, ForceMode.Force);
                FloatersUnderWater += 1;
                if (!Underwater)
                {
                    Underwater = true;
                    SwitchState(true);
                }
            }
        }
        if (Underwater && FloatersUnderWater == 0)
        {
            Underwater = false;
            SwitchState(false);
        }
        else if (!Underwater && FloatersUnderWater > 0)
        {
            Underwater = true;
            SwitchState(true);
        }
    }

    void SwitchState(bool isUnderwater)
    {
        if (isUnderwater)
        {
            Rb.linearDamping  = UnderWaterDrag;
            Rb.angularDamping  = UnderWaterAngularDrag;
        }
        else
        {
            Rb.linearDamping  = AirDrag;
            Rb.angularDamping  = AirAngularDrag;
        }
    }
}