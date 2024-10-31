using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(SplineSampler))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SplineRectangleExtruder : MonoBehaviour
{
    List<Vector3> m_vertsP1;
    List<Vector3> m_vertsP1Top;
    List<Vector3> m_vertsP2;
    List<Vector3> m_vertsP2Top;
    
    [SerializeField]
    private float m_width = 1;
    [SerializeField]
    private float m_height = 1;
    [SerializeField]
    private List<Intersection> m_intersections = new List<Intersection>();

    [SerializeField]
    private int resolution = 10;

    [SerializeField]
    private float m_curveStep = 0.2f;

    public float Width {
        get { return m_width; }
        set { m_width = value; }
    }

    public float Height {
        get { return m_height; }
        set { m_height = value; }
    }

    private void Awake()
    {
        GetVerts();
        BuildMesh();
    }

    private void OnEnable()
    {
        Spline.Changed += OnSplineChanged;
        GetVerts();
    }
    private void OnDisable()
    {
        Spline.Changed -= OnSplineChanged;
    }

    private void OnSplineChanged(Spline arg1, int arg2, SplineModification arg3)
    {
        GetVerts();
        BuildMesh();
    }

    public void AddJunction(Intersection intersection)
    {
        m_intersections.Add(intersection);
        BuildMesh();
    }
    
    public List<Intersection> GetJunctions()
    {
        return m_intersections;
    }

    void GetVerts()
    {
        m_vertsP1 = new List<Vector3>();
        m_vertsP2 = new List<Vector3>();
        float step = 1f / (float)resolution;
        Vector3 p1;
        Vector3 p2;
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        for (int j = 0; j < splineSampler.NumSplines; j++) {
            for (int i = 0; i < resolution; i++) {
                float t = step * i;
                splineSampler.SampleSplineWidth(j, t, out p1, out p2, m_width);
                m_vertsP1.Add(p1);
                m_vertsP2.Add(p2);
            }
            splineSampler.SampleSplineWidth(j, 1f, out p1, out p2, m_width);
            m_vertsP1.Add(p1);
            m_vertsP2.Add(p2);
        }
    }

    private void BuildIntersectionVerts(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
    {       
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        if (m_curveStep == 0f)
        {
            m_curveStep = 0.1f;
        }
        for (int i = 0; i < m_intersections.Count; i++)
        {
            Intersection intersection = m_intersections[i];
            List<JunctionEdge> junctionEdges = new List<JunctionEdge>();
            Vector3 center = new Vector3();
            foreach (JunctionInfo junction in intersection.GetJunctions())
            {
                int splineIndex = junction.splineIndex;
                float t = junction.knotIndex == 0 ? 0f : 1f;
                splineSampler.SampleSplineWidth(splineIndex, t, out Vector3 p1, out Vector3 p2, m_width);
                if (junction.knotIndex == 0)
                {
                    junctionEdges.Add(new JunctionEdge(p1, p2));
                }
                else
                {
                    junctionEdges.Add(new JunctionEdge(p2, p1));
                }
                center += p1;
                center += p2;
            }
            center = center / (junctionEdges.Count * 2);
            junctionEdges.Sort((x, y) => {
                Vector3 xDir = x.Center - center;
                Vector3 yDir = y.Center - center;
                float angleA = Vector3.SignedAngle(center.normalized, xDir.normalized, Vector3.up);
                float angleB = Vector3.SignedAngle(center.normalized, yDir.normalized, Vector3.up);
                if (angleA > angleB)
                {
                    return -1;
                }
                else if (angleA < angleB)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            });

            List<Vector3> curvePoints = new List<Vector3>();
            Vector3 mid;
            Vector3 a;
            Vector3 b;
            Vector3 c;
            BezierCurve curve;
            for (int j = 1; j <= junctionEdges.Count; j++)
            {
                a = junctionEdges[j - 1].left;
                curvePoints.Add(a);
                b = (j < junctionEdges.Count) ? junctionEdges[j].right : junctionEdges[0].right;
                mid = Vector3.Lerp(a, b, 0.5f);
                Vector3 dir = center - mid;
                mid = mid - dir;
                c = Vector3.Lerp(mid, center, intersection.curves[j - 1]);

                curve = new BezierCurve(a, c, b);

                for (float t = 0f; t < 1f; t += m_curveStep)
                {
                    Vector3 pos = CurveUtility.EvaluatePosition(curve, t);
                    curvePoints.Add(pos);
                }
                curvePoints.Add(b);
            }
            curvePoints.Reverse();

            int pointsOffset = verts.Count;
            for (int j = 1; j <= curvePoints.Count; j++)
            {
                int index = (j - 1) * 6;
                Vector3 pointA = curvePoints[j - 1];
                Vector3 pointB;
                if (j == curvePoints.Count)
                {
                    pointB = curvePoints[0];
                }
                else
                {
                    pointB = curvePoints[j];
                }
                
                Vector3 centertop = new Vector3(center.x, center.y + m_height, center.z);
                Vector3 pointAtop = new Vector3(pointA.x, pointA.y + m_height, pointA.z);
                Vector3 pointBtop = new Vector3(pointB.x, pointB.y + m_height, pointB.z);
                // bottom
                verts.Add(center);
                verts.Add(pointA);
                verts.Add(pointB);
    
                // top
                verts.Add(centertop);
                verts.Add(pointAtop);
                verts.Add(pointBtop);

                uvs.Add(new Vector2(center.z, center.x));
                uvs.Add(new Vector2(pointA.z, pointA.x));
                uvs.Add(new Vector2(pointB.z, pointB.x));
                uvs.Add(new Vector2(centertop.z, centertop.x));
                uvs.Add(new Vector2(pointAtop.z, pointAtop.x));
                uvs.Add(new Vector2(pointBtop.z, pointBtop.x));

                // bottom
                tris.Add(pointsOffset + index + 2);
                tris.Add(pointsOffset + index + 1);
                tris.Add(pointsOffset + index + 0);

                // top
                tris.Add(pointsOffset + index + 3);
                tris.Add(pointsOffset + index + 4);
                tris.Add(pointsOffset + index + 5);
                
                // back
                tris.Add(pointsOffset + index + 1);
                tris.Add(pointsOffset + index + 2);
                tris.Add(pointsOffset + index + 4);

                tris.Add(pointsOffset + index + 2);
                tris.Add(pointsOffset + index + 5);
                tris.Add(pointsOffset + index + 4);

            }
        }
    }

    public void BuildMesh()
    {
        Mesh m = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        int offset = 0;

        SplineSampler splineSampler = GetComponent<SplineSampler>();
        for (int currentSplineIndex = 0; currentSplineIndex < splineSampler.NumSplines; currentSplineIndex++) {
            int splineOffset = resolution * currentSplineIndex;
            splineOffset += currentSplineIndex;
            float uvOffset = 0;

            { // Add front triangles
                offset = 8 * resolution * currentSplineIndex;
                int t1 = offset + 0;
                int t2 = offset + 2;
                int t3 = offset + 1;

                int t4 = offset + 0;
                int t5 = offset + 3;
                int t6 = offset + 2;

                tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6});

            }
            // Fill all faces except front/back
            for (int currentSplinePoint = 1; currentSplinePoint < resolution + 1; currentSplinePoint++) {
                int vertoffset = splineOffset + currentSplinePoint;
                Vector3 p1 = m_vertsP1[vertoffset - 1];
                Vector3 p2 = m_vertsP2[vertoffset - 1];
                Vector3 p3 = new Vector3(p2.x, p2.y + m_height, p2.z);
                Vector3 p4 = new Vector3(p1.x, p1.y + m_height, p1.z);
                Vector3 p7 = m_vertsP2[vertoffset];
                Vector3 p8 = m_vertsP1[vertoffset];
                Vector3 p5 = new Vector3(p8.x, p8.y + m_height, p8.z);
                Vector3 p6 = new Vector3(p7.x, p7.y + m_height, p7.z);
                
                offset = 8 * resolution * currentSplineIndex;
                offset += 8 * (currentSplinePoint - 1);

                // top
                int t1 = offset + 2;
                int t2 = offset + 3;
                int t3 = offset + 4;

                int t4 = offset + 2;
                int t5 = offset + 4;
                int t6 = offset + 5;

                // right
                int t7 = offset + 1;
                int t8 = offset + 2;
                int t9 = offset + 5;

                int t10 = offset + 1;
                int t11 = offset + 5;
                int t12 = offset + 6;
                // left
                int t13 = offset + 0;
                int t14 = offset + 7;
                int t15 = offset + 4;

                int t16 = offset + 0;
                int t17 = offset + 4;
                int t18 = offset + 3;

                // bottom
                int t19 = offset + 0;
                int t20 = offset + 6;
                int t21 = offset + 7;

                int t22 = offset + 0;
                int t23 = offset + 1;
                int t24 = offset + 6;

                verts.AddRange(new List<Vector3> {p1, p2, p3, p4, p5, p6, p7, p8});
                tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24});

                float distance = Vector3.Distance(p1, p8) / 4f;
                float uvDistance = uvOffset + distance;
                uvs.AddRange(new List<Vector2> {new Vector2(0, uvOffset), new Vector2(1, uvOffset), new Vector2(0, uvOffset), new Vector2(1, uvOffset), new Vector2(1, uvDistance), new Vector2(0, uvDistance), new Vector2(1, uvDistance), new Vector2(0, uvDistance)});
                uvOffset += distance;
            }
            { // Add back face      
                offset = 8 * resolution * currentSplineIndex;
                offset += 8 * (resolution - 1);
                
                int t1 = offset + 5;
                int t2 = offset + 4;
                int t3 = offset + 7;

                int t4 = offset + 5;
                int t5 = offset + 7;
                int t6 = offset + 6;

                tris.AddRange(new List<int> {t1, t2, t3, t4, t5, t6});
            }
        }

        List<int> trisIntersection = new List<int>();
        BuildIntersectionVerts(verts, trisIntersection, uvs);
        m.subMeshCount = 2;
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetTriangles(trisIntersection, 1);
        m.SetUVs(0, uvs);
        // m.Optimize();
		// m.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = m;
    }
}
