using System.Drawing;
using UnityEngine;

public class Chunk
{
    private int chunkSize;
    private int chunkHeight;

    private Voxels voxels;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private World world;
    private GameObject chunk;

    private byte[,,] voxelMap;

    public Chunk(World world)
    {
        this.world = world;
        chunkSize = world.ChunkSize;
        chunkHeight = world.ChunkHeight;

        voxelMap = new byte[chunkSize, chunkHeight, chunkSize];

        chunk = new GameObject();
        meshFilter = chunk.AddComponent<MeshFilter>();
        meshRenderer = chunk.AddComponent<MeshRenderer>();

        meshRenderer.material = world.blockMaterial;
        chunk.transform.SetParent(world.transform);

        voxels = new Voxels(this, new Vector3Int(chunkSize, chunkHeight, chunkSize));

        PopulateVoxelMap();
        GenerateChunk();
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (y >= chunkHeight - 1)
                    {
                        voxelMap[x, y, z] = 4;
                    }
                    else if (y >= chunkHeight - 2)
                    {
                        voxelMap[x, y, z] = 3;
                    }
                    else if (y == 0)
                    {
                        voxelMap[x, y, z] = 1;
                    }
                    else
                    {
                        voxelMap[x, y, z] = 2;
                    }
                }
            }
        }
    }

    private bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x >= chunkSize ||
            y < 0 || y >= chunkHeight ||
            z < 0 || z >= chunkSize)
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

        return world.blockTypeArray[voxelMap[x, y, z]].isSolid;
    }

    public Vector2[] GetVoxelUVArray(Vector3 position, int face)
    {
        byte voxel = voxelMap[(int)position.x, (int)position.y, (int)position.z];

        return world.Atlas.GetUVArray(world.blockTypeArray[voxel].GetTextureName(face));
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
