using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Chunk
{
    private int chunkSize;
    private int chunkHeight;

    private Voxels voxels;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private World world;
    public GameObject ChunkObject { get; private set; }

    private byte[,,] voxelMap;

    private ChunkID chunkId;
    public Vector3 Position
    {
        get
        {
            float x = chunkId.X * chunkSize;
            float z = chunkId.Z * chunkSize;
            return new Vector3(x, 0.0f, z);
        }
    }

    private bool isActive;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            isActive = value;
            if (ChunkObject != null)
            {
                ChunkObject.SetActive(value);
            }
        }
    }

    public bool IsVoxelMapGenerated { get; private set; }

    public Chunk(World world, ChunkID chunkId, bool generateOnLoad)
    {
        this.world = world;
        chunkSize = world.chunkSize;
        chunkHeight = world.chunkHeight;
        this.chunkId = chunkId;
        isActive = true;

        ChunkObject = new GameObject();
        meshFilter = ChunkObject.AddComponent<MeshFilter>();
        meshRenderer = ChunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.blockMaterial;

        ChunkObject.transform.SetParent(world.transform);
        ChunkObject.transform.position = Position;
        ChunkObject.name = chunkId.ToString();

        voxelMap = new byte[chunkSize, chunkHeight, chunkSize];
        voxels = new Voxels(this, new Vector3Int(chunkSize, chunkHeight, chunkSize));

        IsVoxelMapGenerated = false;

        if (generateOnLoad)
        {
            Init();
        }       
    }

    public void Init()
    {
        PopulateVoxelMap();
        GenerateMesh();
    }

    public void OnPopulatedVoxelMap()
    {
        world.RequestMeshData(this, OnMeshDataCreated);
    }

    private void OnMeshDataCreated(MeshData meshData)
    {
        meshFilter.mesh = meshData.CreateMesh();
    }

    public void PopulateVoxelMap()
    {
        for (int y = 0; y < chunkHeight; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + Position);
                }
            }
        }

        IsVoxelMapGenerated = true;
    }

    public bool IsVoxelSolid(int x, int y, int z)
    {
        return world.blockTypeArray[voxelMap[x, y, z]].isSolid;
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
            return world.HasSolidVoxel(position + Position);
        }

        return world.blockTypeArray[voxelMap[x, y, z]].isSolid;
    }

    public Vector2[] GetVoxelUVArray(Vector3 position, int face)
    {
        byte voxel = voxelMap[(int)position.x, (int)position.y, (int)position.z];

        return world.Atlas.GetUVArray(world.blockTypeArray[voxel].GetTextureName(face));
    }

    public void GenerateMesh()
    {
        MeshData mesh = GetMeshData();

        meshFilter.mesh = mesh.CreateMesh();
    }

    public MeshData GetMeshData()
    {
        voxels.GenerateVoxels();
        return voxels.Data;
    }

    public byte GetVoxelFromGlobalPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);
        int z = Mathf.FloorToInt(position.z);

        x -= (chunkId.X * chunkSize);
        z -= (chunkId.Z * chunkSize);

        return voxelMap[x, y, z];
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
