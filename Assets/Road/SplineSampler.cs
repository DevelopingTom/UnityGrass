using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
using Unity.Mathematics;

[ExecuteInEditMode]
[RequireComponent(typeof(SplineContainer))]
public class SplineSampler : MonoBehaviour
{
    public int NumSplines;

    float3 position;
    float3 forward;
    float3 upVector;

    public void Start()
    {
        NumSplines = GetComponent<SplineContainer>().Splines.Count;
    }

    public void SampleSpline(int splineIndex, float t, out Vector3 p) 
    {
        GetComponent<SplineContainer>().Evaluate(splineIndex, t, out position, out forward, out upVector);
        p = position;
    }

    public void SampleSplineWidth(int splineIndex, float t, out Vector3 p1, out Vector3 p2, float width) 
    {
        GetComponent<SplineContainer>().Evaluate(splineIndex, t, out position, out forward, out upVector);
        float3 right = Vector3.Cross(forward, upVector).normalized;
        p1 = position + (right * width);
        p2 = position + (-right * width);
    }
}
