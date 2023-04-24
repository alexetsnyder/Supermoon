using System.Drawing;
using System;
using UnityEngine;

public class Voxels
{
    public Vector3Int Size { get; set; }

    public MeshData Data { get; private set; }

    private readonly Vector3Int[] UnitVoxelVertexArray = new Vector3Int[8]
    {
        new Vector3Int(0, 0, 0), //0
        new Vector3Int(0, 1, 0), //1
        new Vector3Int(1, 0, 0), //2
        new Vector3Int(1, 1, 0), //3
        new Vector3Int(0, 0, 1), //4
        new Vector3Int(0, 1, 1), //5
        new Vector3Int(1, 0, 1), //6
        new Vector3Int(1, 1, 1), //7
    };

    private readonly int[,] VoxelTriangleIndices = new int[6, 4]
    {
        { 0, 1, 2, 3 }, //Front Face
        { 6, 7, 4, 5 }, //Back Face
        { 1, 5, 3, 7 }, //Top Face
        { 4, 0, 6, 2 }, //Bottom Face
        { 4, 5, 0, 1 }, //Left Face
        { 2, 3, 6, 7 }, //Right Face
    };

    private readonly Vector3[] Neighbors = new Vector3[6]
    {
        new Vector3( 0.0f,  0.0f, -1.0f),
        new Vector3( 0.0f,  0.0f,  1.0f),
        new Vector3( 0.0f,  1.0f,  0.0f),
        new Vector3( 0.0f, -1.0f,  0.0f),
        new Vector3(-1.0f,  0.0f,  0.0f),
        new Vector3( 1.0f,  0.0f,  0.0f),
    };

    private Chunk chunk;

    private int index;
    private int triangleIndex;

    public Voxels(Chunk chunk, Vector3Int size)
    {
        this.chunk = chunk;
        this.Size = size;

        Data = new MeshData(size);

        index = 0;
        triangleIndex = 0;
    }

    public void GenerateVoxels()
    {
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int z = 0; z < Size.z; z++)
                {
                    if (chunk.IsVoxelSolid(x, y, z))
                    {
                        CreateVoxel(new Vector3(x, y, z));
                    }    
                }
            }
        }
    }

    public void CreateVoxel(Vector3 position)
    {
        for (int face = 0; face < 6; face++)
        {
            if (!chunk.HasSolidVoxel(position + Neighbors[face]))
            {
                CreateFace(face, position);
            } 
        }
    }

    private void CreateFace(int face, Vector3 position)
    {
        int v1 = VoxelTriangleIndices[face, 0];
        int v2 = VoxelTriangleIndices[face, 1];
        int v3 = VoxelTriangleIndices[face, 2];
        int v4 = VoxelTriangleIndices[face, 3];

        Data.vertexArray[index] = position + UnitVoxelVertexArray[v1];
        Data.vertexArray[index + 1] = position + UnitVoxelVertexArray[v2];
        Data.vertexArray[index + 2] = position + UnitVoxelVertexArray[v3];
        Data.vertexArray[index + 3] = position + UnitVoxelVertexArray[v4];

        Vector2[] faceUVArray = chunk.GetVoxelUVArray(position, face);
        Data.uvArray[index] = faceUVArray[0];
        Data.uvArray[index + 1] = faceUVArray[1];
        Data.uvArray[index + 2] = faceUVArray[2];
        Data.uvArray[index + 3] = faceUVArray[3];

        Data.triangleArray[triangleIndex] = index;
        Data.triangleArray[triangleIndex + 1] = index + 1;
        Data.triangleArray[triangleIndex + 2] = index + 2;

        Data.triangleArray[triangleIndex + 3] = index + 2;
        Data.triangleArray[triangleIndex + 4] = index + 1;
        Data.triangleArray[triangleIndex + 5] = index + 3;

        index += 4;
        triangleIndex += 6;
    }
}

public struct MeshData
{
    public Vector3[] vertexArray;
    public int[] triangleArray;
    public Vector2[] uvArray;

    public MeshData(Vector3Int size)
    {
        int voxelCount = size.x * size.y * size.z;
        vertexArray = new Vector3[24 * voxelCount];
        uvArray = new Vector2[24 * voxelCount];
        triangleArray = new int[36 * voxelCount];
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = vertexArray;
        mesh.triangles = triangleArray;
        mesh.uv = uvArray;

        mesh.RecalculateNormals();

        return mesh;
    }
}