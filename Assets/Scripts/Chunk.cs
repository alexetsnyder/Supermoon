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

    private ChunkID chunkID;
    public Vector3 Position
    {
        get
        {
            float x = chunkID.X * chunkSize;
            float z = chunkID.Z * chunkSize;
            return new Vector3(x, 0.0f, z);
        }
    }

    public Chunk(World world, ChunkID chunkID)
    {
        this.world = world;
        chunkSize = world.ChunkSize;
        chunkHeight = world.ChunkHeight;
        this.chunkID = chunkID;

        Init();       
    }

    public void Init()
    {
        chunk = new GameObject();
        meshFilter = chunk.AddComponent<MeshFilter>();
        meshRenderer = chunk.AddComponent<MeshRenderer>();
        meshRenderer.material = world.blockMaterial;

        chunk.transform.SetParent(world.transform);
        chunk.transform.position = Position;
        chunk.name = chunkID.ToString();

        voxelMap = new byte[chunkSize, chunkHeight, chunkSize];
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
                        voxelMap[x, y, z] = world.GetBlockID("Grass");
                    }
                    else if (y >= chunkHeight - 2)
                    {
                        voxelMap[x, y, z] = world.GetBlockID("Dirt");
                    }
                    else if (y == 0)
                    {
                        voxelMap[x, y, z] = world.GetBlockID("Bedrock");
                    }
                    else
                    {
                        voxelMap[x, y, z] = world.GetBlockID("Stone");
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

        AddMesh();
    }

    private void AddMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = voxels.VertexArray;
        mesh.triangles = voxels.TriangleArray;
        mesh.uv = voxels.UVArray;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
}

public class ChunkID
{
    public int X { get; private set; }
    public int Z { get; private set; }

    public ChunkID(int x, int y)
    {
        X = x;
        Z = y;
    }

    public override string ToString()
    {
        return "Chunk " + X + ", " + Z;
    }

    public override bool Equals(object obj)
    {
        if (obj != null && obj is ChunkID chunkId)
        {
            return X == chunkId.X && Z == chunkId.Z;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return (X, Z).GetHashCode();
    }
}
