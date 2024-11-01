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
    private List<MeshInfo> meshes = new List<MeshInfo>();
    [SerializeField]
    private bool bottomDetails = false;

    private List<GameObject> details;

    private void UpdateNormals(ref Mesh mesh, int splineIndex)
    {
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        var vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            Vector3 position = splineSampler.NearestPointFrom(splineIndex, vertices[i]);
            normals[i] = vertices[i] - position;
        }
        mesh.normals = normals;
    }

    void InstanciateMesh(MeshInfo info, Vector3 position, ref CombineInstance combine)
    {
        var rotation = Quaternion.AngleAxis(Random.Range(0,360), Vector3.up);
        combine.mesh = info.mesh;
        combine.transform = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1));
    }

    void InstanciateMeshes()
    {
        if (!meshes.Any()) {
            return;
        }
        Vector3 p1;
        Vector3 p2;
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        SplineRectangleExtruder splineExtruder = GetComponent<SplineRectangleExtruder>();
        foreach (MeshInfo info in meshes)
        {
            for (int i = 0; i < splineSampler.NumSplines; i++) {
                details.Add(new GameObject("Details"));
                GameObject detail = details.Last();
                detail.transform.parent = gameObject.transform;
                int numInstance = 0;
                CombineInstance[] combine = new CombineInstance[info.frequency];
                // sides
                for (int j = 0; j < (int)(info.frequency / 3); j++) {
                    float t = Random.Range(0f, 1f);
                    splineSampler.SampleSplineWidth(i, t, out p1, out p2, splineExtruder.Width);
                    Vector3 meshPoint = p1 + new Vector3(0, Random.Range(0, splineExtruder.Height), 0);
                    InstanciateMesh(info, meshPoint, ref combine[numInstance]);
                    numInstance++;
                    Vector3 meshPoint2 = p2 + new Vector3(0, Random.Range(0, splineExtruder.Height), 0);
                    InstanciateMesh(info, meshPoint2, ref combine[numInstance]);
                    numInstance++;
                }
                // top
                for (int j = 0; j < (int)(info.frequency / 3); j++) {
                    float t = Random.Range(0f, 1f);
                    splineSampler.SampleSpline(i, t, out p1);
                    Vector3 meshPoint = p1 + new Vector3(Random.Range(-splineExtruder.Width, splineExtruder.Width), splineExtruder.Height, 0);
                    InstanciateMesh(info, meshPoint, ref combine[numInstance]);
                    numInstance++;
                }
                if (bottomDetails) {
                    for (int j = 0; j < (int)(info.frequency / 3); j++) {
                        float t = Random.Range(0f, 1f);
                        splineSampler.SampleSpline(i, t, out p1);
                        Vector3 meshPoint = p1 + new Vector3(Random.Range(-splineExtruder.Width, splineExtruder.Width), 0, 0);
                        InstanciateMesh(info, meshPoint, ref combine[numInstance]);
                        numInstance++;
                    }
                }
                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                mesh.CombineMeshes(combine);
                UpdateNormals(ref mesh, i);
                MeshFilter filter = detail.AddComponent(typeof(MeshFilter)) as MeshFilter;
                filter.sharedMesh = mesh;
                MeshRenderer renderer = detail.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
                renderer.material = info.material;
            }
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
