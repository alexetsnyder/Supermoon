using UnityEngine;

public class Voxels
{
    public Vector3Int Size;

    public Vector3[] VertexArray { get; private set; }

    public int[] TriangleArray { get; private set; }

    public Vector2[] UVArray { get; private set; }

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

        int voxelCount = Size.x * Size.y * Size.z;
        VertexArray = new Vector3[24 * voxelCount];
        UVArray = new Vector2[24 * voxelCount];
        TriangleArray = new int[36 * voxelCount];

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
                    CreateVoxel(new Vector3(x, y, z));
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

        VertexArray[index] = position + UnitVoxelVertexArray[v1];
        VertexArray[index + 1] = position + UnitVoxelVertexArray[v2];
        VertexArray[index + 2] = position + UnitVoxelVertexArray[v3];
        VertexArray[index + 3] = position + UnitVoxelVertexArray[v4];

        UVArray[index] = new Vector2(0.0f, 0.0f);
        UVArray[index + 1] = new Vector2(0.0f, 1.0f);
        UVArray[index + 2] = new Vector2(1.0f, 0.0f);
        UVArray[index + 3] = new Vector2(1.0f, 1.0f);

        TriangleArray[triangleIndex] = index;
        TriangleArray[triangleIndex + 1] = index + 1;
        TriangleArray[triangleIndex + 2] = index + 2;

        TriangleArray[triangleIndex + 3] = index + 2;
        TriangleArray[triangleIndex + 4] = index + 1;
        TriangleArray[triangleIndex + 5] = index + 3;

        index += 4;
        triangleIndex += 6;
    }
}
