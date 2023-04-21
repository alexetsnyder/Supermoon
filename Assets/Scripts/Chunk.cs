using UnityEngine;

public class Chunk : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        GenerateChunk();
    }

    public void GenerateChunk()
    {
        Voxel voxel = new Voxel();
        voxel.CreateVoxel();

        Mesh mesh = new Mesh();
        mesh.vertices = voxel.vertexArray;
        mesh.triangles = voxel.triangleArray;
        mesh.uv = voxel.uvArray;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
