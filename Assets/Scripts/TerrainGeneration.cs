using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Blocks")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject stoneBlock;

    [Header("World Generation")]
    public int worldSize;
    public int dirtDepth;
    public int chunkSize;

    [Header("Noise")]
    public float seed;
    public Noise terrainNoise;
    public int MinHeight;
    public int MaxHeight;

    private void Start()
    {
        GenerateChunks();
    }

    public void GenerateChunks()
    {
        seed = Random.Range(0.0f, 100000000.0f);

        int chunkCount = worldSize / chunkSize;

        for (int chkX = -chunkCount / 2; chkX < chunkCount / 2; chkX++)
        {
            for (int chkZ = -chunkCount / 2; chkZ < chunkCount / 2; chkZ++)
            {
                GameObject chunk = new GameObject();
                chunk.name = "Chunk " + chkX.ToString() + ":" + chkZ.ToString();
                chunk.transform.parent = transform;
                chunk.transform.position = new Vector3(chkX * 2 * chunkSize, transform.position.y, chkZ * 2 * chunkSize);

                GenerateChunkTerrain(chunk);
            }         
        }
    }

    private void GenerateChunkTerrain(GameObject chunk)
    {
        int xStart = (int)chunk.transform.position.x;
        int zStart = (int)chunk.transform.position.z;

        for (int x = xStart; x < xStart + 2 * chunkSize; x += 2)
        {
            for (int z = zStart; z < zStart + 2 * chunkSize; z += 2)
            {
                float height = math.remap(0.0f, 1.0f, MinHeight, MaxHeight, terrainNoise.SNoise(x + seed, z + seed));
                for (int y = 0; y < height; y += 2)
                {
                    if (y >= height - 2)
                    {
                        PlaceBlock(x, y, z, grassBlock, chunk.transform);
                    }
                    else if (y > height - 2 * dirtDepth)
                    {
                        PlaceBlock(x, y, z, dirtBlock, chunk.transform);
                    }
                    else
                    {
                        PlaceBlock(x, y, z, stoneBlock, chunk.transform);
                    }
                }
            }
        }
    }

    private void PlaceBlock(int x, int y, int z, GameObject block, Transform parent)
    {
        GameObject go = Instantiate(block, parent);
        go.name = block.name;
        go.transform.position = new Vector3(x, y, z);
    }
}
