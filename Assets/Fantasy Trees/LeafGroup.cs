using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class LeafGroup : MonoBehaviour
{
    [SerializeField]
    private Mesh mesh;
    [SerializeField]
    private Material material;
    [SerializeField]
    private int frequency = 10;
    [SerializeField]
    private float radius = 1;

    private List<GameObject> details;

    private void UpdateNormals(ref Mesh mesh)
    {
        SplineSampler splineSampler = GetComponent<SplineSampler>();
        var vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];
        for (var i = 0; i < vertices.Length; i++)
        {
            normals[i] = vertices[i];
        }
        mesh.normals = normals;
    }

    void InstanciateMesh(Mesh mesh, Vector3 position, ref CombineInstance combine)
    {
        var rotation = Quaternion.AngleAxis(Random.Range(0,360), Vector3.up);
        combine.mesh = mesh;
        combine.transform = Matrix4x4.TRS(position, rotation, new Vector3(1, 1, 1));
    }

    void InstanciateMeshes()
    {
        details.Add(new GameObject("Details"));
        GameObject detail = details.Last();
        detail.transform.parent = gameObject.transform;
        int numInstance = 0;
        CombineInstance[] combine = new CombineInstance[frequency];
        for (int j = 0; j < frequency; j++) {
            Vector3 meshPoint = Random.onUnitSphere * radius;
            InstanciateMesh(mesh, meshPoint, ref combine[numInstance]);
            numInstance++;
        }
        Mesh combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        combinedMesh.CombineMeshes(combine);
        UpdateNormals(ref combinedMesh);
        MeshFilter filter = detail.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.sharedMesh = combinedMesh;
        MeshRenderer renderer = detail.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        renderer.material = material;
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
