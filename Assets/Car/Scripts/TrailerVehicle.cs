using UnityEngine;

public class TrailerVehicle : MonoBehaviour
{
    [SerializeField] Transform carAttachmentPoint; 
    private ConfigurableJoint joint;
    private float jointLinearLimit = 0.07f;
    private GameObject lastDroppedTrailer;
    private bool isTrailerDropped = false;
    public float connectionDistance = 2f;
    public float cooldownDistance = 5.0f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            DropTrailer();
        }
        if (isTrailerDropped && lastDroppedTrailer != null)
        {
            float dropDistance = Vector3.Distance(carAttachmentPoint.position, lastDroppedTrailer.transform.position);
            if (dropDistance > cooldownDistance)
            {
                isTrailerDropped = false;
                lastDroppedTrailer = null;
            }
            else
            {
                return; // Skip the attachment process if the trailer is still within the cooldown distance
            }
        }
        GameObject closestTrailer = FindClosestTrailer();
        if (closestTrailer != null)
        {        
            Transform trailerAttachmentPoint = closestTrailer.transform.Find("TrailerAttachmentPoint");
            if (trailerAttachmentPoint == null)
            {
                Debug.LogError("No TrailerAttachmentPoint found on " + closestTrailer.name);
                return;
            }
            // Check the distance between the car and the closest trailer
            float distance = Vector3.Distance(trailerAttachmentPoint.position, carAttachmentPoint.position);
    
            // If the trailer is close enough, connect it to the car
            if (distance <= connectionDistance)
            {        
                if (joint == null)
                {
                    GrabTrailer(closestTrailer, trailerAttachmentPoint);
                }
            }
        }
        else
        {
            // No trailers found, ensure joint is disconnected
            if (joint != null && joint.connectedBody != null)
            {
                DropTrailer();
            }
        }
        
    }

    private void DropTrailer()
    {
        if (joint != null && joint.connectedBody  != null)
        {
            lastDroppedTrailer = joint.connectedBody.gameObject;
            isTrailerDropped = true;
            Destroy(joint);
        }
    }
    GameObject FindClosestTrailer()
    {
        GameObject[] trailers = GameObject.FindGameObjectsWithTag("Trailer");
        GameObject closestTrailer = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject trailer in trailers)
        {
            float distance = Vector3.Distance(transform.position, trailer.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTrailer = trailer;
            }
        }

        return closestTrailer;
    }
    private void GrabTrailer(GameObject trailer, Transform trailerAttachmentPoint)
    {

        if (!joint)
        {
            SoftJointLimit linearLimit = new SoftJointLimit
            {
                limit = jointLinearLimit
            };
            joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = trailer.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = carAttachmentPoint.localPosition;
            joint.connectedAnchor = trailerAttachmentPoint.localPosition;
            joint.linearLimit =  linearLimit;
            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;
            joint.angularXMotion = ConfigurableJointMotion.Limited;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Limited;
        }
    }

}
