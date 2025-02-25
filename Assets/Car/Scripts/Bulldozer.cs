using UnityEngine;
using System.Collections;

public class Bulldozer : MonoBehaviour
{
    [SerializeField] protected ParticleSystem dustParticleSystems;
    
    [SerializeField] private GameObject laddle; // The GameObject to be moved
    [SerializeField] private Transform pivotPoint; // The pivot point around which the laddle will rotate
    [SerializeField] private float maxRotation = 25f; // Maximum rotation angle in degrees
    [SerializeField] private float minRotation = -25f; // Minimum rotation angle in degrees
    [SerializeField] private float moveSpeed = 20f; // Speed of the movement

    private void Start()
    {
        dustParticleSystems.Stop();
    }

    private void Update()
    {
        Enterable enterable = GetComponent<Enterable>();

        if (enterable != null && enterable.Entered)
        {
            dustParticleSystems.Play();
            GetPlayerInput();
        }
    }

    #region Input Handling
    private void GetPlayerInput()
    {
        if (Input.GetKey(KeyCode.C))
        {
            UpLaddle();
        }
        if (Input.GetKey(KeyCode.V))
        {
            DownLaddle();
        }
    }
    #endregion

    private void UpLaddle()
    {
        float currentRotation = laddle.transform.localEulerAngles.x;
        if (currentRotation > 180) currentRotation -= 360;

        if (currentRotation > minRotation)
        {
            MoveLaddle(Vector3.up);
        }
    }

    private void DownLaddle()
    {
        float currentRotation = laddle.transform.localEulerAngles.x;
        if (currentRotation > 180) currentRotation -= 360;

        if (currentRotation < maxRotation)
        {
            MoveLaddle(Vector3.down);
        }
    }

    private void MoveLaddle(Vector3 direction)
    {
        float step = moveSpeed * Time.deltaTime;
        laddle.transform.RotateAround(pivotPoint.position, transform.right, direction == Vector3.up ? -step : step);
    }
}
