using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(SplineRectangleExtruder))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SplineEdgeInstancer : MonoBehaviour
{
    [SerializeField]
    private List<Mesh> meshes = new List<Mesh>();
    [SerializeField]
    private int meshNumberPerSpline = 10;
    [SerializeField]
    private bool bottomDetails = false;

    [SerializeField]
    private Material material;

    private List<GameObject> details;
    private List<Mesh> copiedMeshes = new List<Mesh>();

    private Mesh CopyMesh(Mesh mesh)
    {
        Mesh newmesh = new Mesh();
        newmesh.vertices = mesh.vertices;
        newmesh.triangles = mesh.triangles;
        newmesh.uv = mesh.uv;
        newmesh.normals = mesh.normals;
        newmesh.colors = mesh.colors;
        newmesh.tangents = mesh.tangents;
        return newmesh;
    }

    private void UpdateNormals(ref Mesh mesh, Vector3 normal)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] = normal;
        mesh.normals = normals;
    }

    void InstanciateMesh(Vector3 position, Vector3 normal, ref CombineInstance combine)
    {
        int randomPrefab = Random.Range(0, meshes.Count);
        //var rotation = Quaternion.Euler( Random.Range(0, 360) , 0 , 0);
        var rotation = Quaternion.Euler(normal.x, normal.y, normal.z);
        Mesh copied = CopyMesh(meshes[randomPrefab]);
        UpdateNormals(ref copied, normal);
        combine.mesh = copied;
        combine.transform = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1));
    }

    void InstanciateMeshes()
    {
        if (!meshes.Any()) {
            return;
        }
        Vector3 center;
        Vector3 p1;
        Vector3 p2;
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        SplineRectangleExtruder splineExtruder = GetComponent<SplineRectangleExtruder>();
        for (int i = 0; i < splineSampler.NumSplines; i++) {
            details.Add(new GameObject("Details"));
            details[i].transform.parent = gameObject.transform;
            int numInstance = 0;
            CombineInstance[] combine = new CombineInstance[meshNumberPerSpline];
            // sides
            for (int j = 0; j < (int)(meshNumberPerSpline / 3); j++) {
                float t = Random.Range(0f, 1f);
                splineSampler.SampleSplineWidth(i, t, out p1, out p2, splineExtruder.Width);
                Vector3 meshPoint = p1 + new Vector3(0, Random.Range(0, splineExtruder.Height), 0);
                splineSampler.SampleSpline(i, t, out center);
                Vector3 normal = Vector3.Normalize(transform.TransformPoint(meshPoint - center));
                InstanciateMesh(meshPoint, normal, ref combine[numInstance]);
                numInstance++;
                Vector3 meshPoint2 = p2 + new Vector3(0, Random.Range(0, splineExtruder.Height), 0);
                normal = Vector3.Normalize(transform.TransformPoint(meshPoint2 - center));
                InstanciateMesh(meshPoint2, normal, ref combine[numInstance]);
                numInstance++;
            }
            // top
            for (int j = 0; j < (int)(meshNumberPerSpline / 3); j++) {
                float t = Random.Range(0f, 1f);
                splineSampler.SampleSpline(i, t, out p1);
                Vector3 meshPoint = p1 + new Vector3(Random.Range(-splineExtruder.Width, splineExtruder.Width), splineExtruder.Height, 0);
                Vector3 normal = Vector3.Normalize(transform.TransformPoint(meshPoint - p1));
                InstanciateMesh(meshPoint, normal, ref combine[numInstance]);
                numInstance++;
            }
            if (bottomDetails) {
                for (int j = 0; j < (int)(meshNumberPerSpline / 3); j++) {
                    float t = Random.Range(0f, 1f);
                    splineSampler.SampleSpline(i, t, out p1);
                    Vector3 meshPoint = p1 + new Vector3(Random.Range(-splineExtruder.Width, splineExtruder.Width), 0, 0);
                    Vector3 normal = Vector3.Normalize(meshPoint - p1);
                    InstanciateMesh(meshPoint, normal, ref combine[numInstance]);
                    numInstance++;
                }
            }
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.CombineMeshes(combine);
            MeshFilter filter = details[i].AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.sharedMesh = mesh;
            MeshRenderer renderer = details[i].AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            renderer.material = material;
        }
    }

    [ContextMenu("Instantiate meshes")] 
    private void CreateDetails() {
        if (details == null) {
            details = new List<GameObject>();
        } else {
            for (int i = 0; i < details.Count; i++) {
                GameObject.DestroyImmediate(details[i].gameObject);
            }
            details.Clear();
        }
        InstanciateMeshes();
    }
}
