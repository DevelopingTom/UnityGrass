using UnityEngine;

public class SetInteractiveShadderEffect : MonoBehaviour
{
    [SerializeField]
    RenderTexture rt;
    [SerializeField]
    Transform target;

    Quaternion fixedRotation;

    void Awake()
    {
        fixedRotation = transform.rotation;
        Shader.SetGlobalTexture("_GlobalEffectRT", rt);
        Shader.SetGlobalFloat("_OrthographicCamSize", GetComponent<Camera>().orthographicSize);
    }
 
    private void Update()
    {
        transform.rotation = fixedRotation;
        transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
