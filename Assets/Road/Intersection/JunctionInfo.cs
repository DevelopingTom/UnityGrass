using UnityEngine;
using UnityEngine.Splines;

[System.Serializable]
public struct JunctionInfo
{
    public int splineIndex;
    public int knotIndex;
    public Spline spline;
    public BezierKnot knot;

    public JunctionInfo(int splineIndex, int knotIndex, Spline spline, BezierKnot knot)
    {
        this.splineIndex = splineIndex;
        this.knotIndex = knotIndex;
        this.spline = spline;
        this.knot = knot;
    }
}

struct JunctionEdge
{
    public Vector3 left;
    public Vector3 right;
    public Vector3 Center => (left + right) / 2;
    public JunctionEdge (Vector3 p1, Vector3 p2)
    {
        left = p1;
        right = p2;
    }
}