using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Intersection
{
    public List<JunctionInfo> junctions;
    public List<float> curves;

    public void AddJunction(int splineIndex, int knotIndex, Spline spline, BezierKnot knot)
    {
        if (junctions == null)
        {
            junctions = new List<JunctionInfo>();
            curves = new List<float>();
        }
        junctions.Add(new JunctionInfo(splineIndex, knotIndex, spline, knot));
        curves.Add(0.3f);
    }

    internal IEnumerable<JunctionInfo> GetJunctions()
    {
        return junctions;
    }

    public bool HasSplineIndex(int splineIndex) {
        foreach(JunctionInfo junction in junctions) 
        {
            if (junction.splineIndex == splineIndex)
            {
                return true;
            }
        }
        return false;
    }
}
