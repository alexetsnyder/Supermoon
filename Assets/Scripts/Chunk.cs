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

        voxels = new Voxels(this, new Vector3Int(ChunkSize, ChunkHeight, ChunkSize));
    }

    private void Start()
    {
        GenerateChunk();
    }

    private bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= ChunkSize ||
            y < 0 || y >= ChunkHeight ||
            z < 0 || z >= ChunkSize)
        {
            return false;
        }

        return true;
    }

    public bool HasSolidVoxel(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int z = Mathf.FloorToInt(position.z);
        
        if (!IsVoxelInChunk(x, y, z))
        {
            return false;
        }

        return true;
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
