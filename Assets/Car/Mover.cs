using UnityEngine;
using KinematicCharacterController;

public class Mover : MonoBehaviour, IMoverController
{
    public PhysicsMover physicsMover;
    private Transform _transform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _transform = this.transform;
        physicsMover.MoverController = this;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        // Remember pose before animation
        Vector3 _positionBefore = _transform.position;
        Quaternion _rotationBefore = _transform.rotation;

        // Update animation
        //EvaluateAtTime(Time.time);

        // Set our platform's goal pose to the animation's
        goalPosition = _transform.position;
        goalRotation = _transform.rotation;

        // Reset the actual transform pose to where it was before evaluating. 
        // This is so that the real movement can be handled by the physics mover; not the animation
        _transform.position = _positionBefore;
        _transform.rotation = _rotationBefore;
    }
}