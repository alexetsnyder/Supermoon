using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int ChunkSize;
    public int ChunkHeight;

    private Voxels voxels;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();

        voxels = new Voxels(new Vector3Int(ChunkSize, ChunkHeight, ChunkSize));
    }

    private void Start()
    {
        GenerateChunk();
    }

    public void GenerateChunk()
    {
        voxels.GenerateVoxels();

        Mesh mesh = new Mesh();
        mesh.vertices = voxels.VertexArray;
        mesh.triangles = voxels.TriangleArray;
        mesh.uv = voxels.UVArray;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}
