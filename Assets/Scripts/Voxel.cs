using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public Vector3[] vertexArray
    {
        get
        {
            return vertexList.ToArray();
        }
    }

    public int[] triangleArray
    {
        get
        {
            return triangleList.ToArray();
        }
    }

    public Vector2[] uvArray
    {
        get
        {
            return uvList.ToArray();
        }
    }

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

    private List<Vector3> vertexList;
    private List<int> triangleList;
    private List<Vector2> uvList;

    public Voxel()
    {
        vertexList = new List<Vector3>();
        triangleList = new List<int>();
        uvList = new List<Vector2>();
    }

    public void CreateVoxel()
    {
        for (int face = 0; face < 6; face++)
        {
            CreateFace(face);
        }
    }

    private void CreateFace(int face)
    {
        int vertexIndex = vertexList.Count;

        int v1 = VoxelTriangleIndices[face, 0];
        int v2 = VoxelTriangleIndices[face, 1];
        int v3 = VoxelTriangleIndices[face, 2];
        int v4 = VoxelTriangleIndices[face, 3]; 

        vertexList.Add(UnitVoxelVertexArray[v1]);
        vertexList.Add(UnitVoxelVertexArray[v2]);
        vertexList.Add(UnitVoxelVertexArray[v3]);
        vertexList.Add(UnitVoxelVertexArray[v4]);

        uvList.Add(new Vector2(0.0f, 0.0f));
        uvList.Add(new Vector2(0.0f, 1.0f));
        uvList.Add(new Vector2(1.0f, 0.0f));
        uvList.Add(new Vector2(1.0f, 1.0f));

        triangleList.Add(vertexIndex);
        triangleList.Add(vertexIndex + 1);
        triangleList.Add(vertexIndex + 2);

        triangleList.Add(vertexIndex + 2);
        triangleList.Add(vertexIndex + 1);
        triangleList.Add(vertexIndex + 3);
    }
}
