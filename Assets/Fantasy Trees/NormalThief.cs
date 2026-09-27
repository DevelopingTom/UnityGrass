using UnityEngine;

public class NormalThief : MonoBehaviour
{
    public MeshFilter highPolyMeshFilter; // The high-poly mesh filter to steal normals from
    public MeshFilter lowPolyMeshFilter;  // The low-poly mesh filter to apply normals to

    void Start()
    {
        if (highPolyMeshFilter == null || lowPolyMeshFilter == null)
        {
            Debug.LogError("Please assign both highPolyMeshFilter and lowPolyMeshFilter.");
            return;
        }

        Mesh highPolyMesh = highPolyMeshFilter.mesh;
        Mesh lowPolyMesh = lowPolyMeshFilter.mesh;

        // Clone the low-poly mesh to make it editable
        Mesh editableLowPolyMesh = Instantiate(lowPolyMesh);

        TransferNormals(highPolyMesh, editableLowPolyMesh);

        // Assign the edited mesh back to the low-poly mesh filter
        lowPolyMeshFilter.mesh = editableLowPolyMesh;
    }

    void TransferNormals(Mesh highPolyMesh, Mesh lowPolyMesh)
    {
        Vector3[] highPolyVertices = highPolyMesh.vertices;
        Vector3[] highPolyNormals = highPolyMesh.normals;
        Vector3[] lowPolyVertices = lowPolyMesh.vertices;
        Vector3[] lowPolyNormals = new Vector3[lowPolyVertices.Length];

        for (int i = 0; i < lowPolyVertices.Length; i++)
        {
            Vector3 closestPoint = FindClosestPointOnHighPolyMesh(lowPolyVertices[i], highPolyVertices);
            lowPolyNormals[i] = GetNormalAtPoint(closestPoint, highPolyVertices, highPolyNormals);
        }

        lowPolyMesh.normals = lowPolyNormals;
        lowPolyMesh.RecalculateTangents();
    }

    Vector3 FindClosestPointOnHighPolyMesh(Vector3 point, Vector3[] highPolyVertices)
    {
        Vector3 closestPoint = Vector3.zero;
        float closestDistance = float.MaxValue;

        foreach (Vector3 vertex in highPolyVertices)
        {
            float distance = Vector3.Distance(point, vertex);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPoint = vertex;
            }
        }

        return closestPoint;
    }

    Vector3 GetNormalAtPoint(Vector3 point, Vector3[] highPolyVertices, Vector3[] highPolyNormals)
    {
        for (int i = 0; i < highPolyVertices.Length; i++)
        {
            if (highPolyVertices[i] == point)
            {
                return highPolyNormals[i];
            }
        }

        return Vector3.zero;
    }
}