using UnityEngine;

[System.Serializable]
public struct MeshInfo
{
    public Mesh mesh;
    public Material material;
    public int frequency;
    public MeshInfo(Mesh mesh, Material material, int frequency)
    {
        this.mesh = mesh;
        this.material = material;
        this.frequency = frequency;
    }
}
