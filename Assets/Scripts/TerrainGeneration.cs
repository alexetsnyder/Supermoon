using Unity.Mathematics;
using UnityEngine;

public class TerrainGeneration : MonoBehaviour
{
    [Header("Blocks")]
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject stoneBlock;

    [Header("World Generation")]
    public int worldSize;
    public int dirtDepth;

    [Header("Noise")]
    public float seed;
    public Noise terrainNoise;
    public int MinHeight;
    public int MaxHeight;

    private void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        for (int x = 0; x < worldSize; x += 2)
        {
            for (int z = 0; z < worldSize; z += 2)
            {
                float height = math.remap(0.0f, 1.0f, MinHeight, MaxHeight, terrainNoise.SNoise(x + seed, z + seed));
                for (int y = 0; y < height; y += 2)
                {
                    if (y >= height - 2)
                    {
                        PlaceBlock(x, y, z, grassBlock, transform);
                    }
                    else if (y > height - dirtDepth)
                    {
                        PlaceBlock(x, y, z, dirtBlock, transform);
                    }
                    else
                    {
                        PlaceBlock(x, y, z, stoneBlock, transform);
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
