using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("World Attributes")]
    public int worldSize;

    [Header("Noise Attributes")]
    public float seed;
    public float terrainScale;

    [Header("Chunk Attributes")]
    public int chunkHeight;
    public int chunkSize;
    public int terrainHeight;
    public int solidTerrainHeight;
    public int dirtDepth;

    [Header("Block Attributes")]
    public Material blockMaterial;
    public BlockType[] blockTypeArray;


    public TextureAtlas Atlas { get; private set; }

    private Dictionary<ChunkID, Chunk> chunkDict;
    private List<ChunkID> activeChunks;

    private Dictionary<string, byte> blockTypeLookUp;

    private void Awake()
    {
        activeChunks = new List<ChunkID>();
        Atlas = GetComponent<TextureAtlas>();

        chunkDict = new Dictionary<ChunkID, Chunk>();

        blockTypeLookUp = new Dictionary<string, byte>();
        for (byte i = 0; i < blockTypeArray.Length; i++)
        {
            blockTypeLookUp.Add(blockTypeArray[i].name, i);
        }
    }

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        for (int x = -worldSize / 2; x < worldSize / 2; x++)
        {
            for (int z = -worldSize / 2; z < worldSize / 2; z++)
            {
                ChunkID chunkId = new ChunkID(x, z);
                chunkDict.Add(chunkId, new Chunk(this, chunkId));
                activeChunks.Add(chunkId);
            }
        }    
    }

    public bool HasSolidVoxel(Vector3 position)
    {
        ChunkID chunkId = GetChunkID(position);

        if (!IsChunkInWorld(chunkId) || position.y < 0 || position.y > chunkHeight - 1)
        {
            return false;
        }

        if (chunkDict.ContainsKey(chunkId))
        {
            byte voxel = chunkDict[chunkId].GetVoxelFromGlobalPosition(position);
            return blockTypeArray[voxel].isSolid;
        }

        return blockTypeArray[GetVoxel(position)].isSolid;
    }

    private bool IsChunkInWorld(ChunkID chunkId)
    {
        int boundSize = worldSize / 2;
        if (chunkId.X < -boundSize || chunkId.X > boundSize - 1 ||
            chunkId.Z < -boundSize || chunkId.Z > boundSize - 1)
        {
            return false;
        }

        return true;
    }

    public byte GetVoxel(Vector3 position)
    {
        int yPos = Mathf.FloorToInt(position.y);

        /* IMMUTABLE PASS */

        if (yPos == 0)
        {
            return GetBlockID("Bedrock");
        }

        /* BASIC TERRAIN GENERATION */

        Vector2 noisePos = new Vector2(position.x / chunkSize, position.z / chunkSize);
        float noise = Noise.Get2DPerlinNoise(noisePos, terrainScale, seed);
        int height = Mathf.FloorToInt(terrainHeight * noise) + solidTerrainHeight;

        if (yPos > height)
        {
            return GetBlockID("Air");
        }
        if (yPos == height)
        {
            return GetBlockID("Grass");
        }
        else if (yPos >= height - dirtDepth)
        {
            return GetBlockID("Dirt");
        }
        else
        {
            return GetBlockID("Stone");
        }
    }

    public byte GetBlockID(string name)
    {
        return blockTypeLookUp[name];
    }

    public ChunkID GetChunkID(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int z = Mathf.FloorToInt(position.z / chunkSize);

        return new ChunkID(x, z);
    }
}

