using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("World Attributes")]
    public int ViewDistanceInChunks;

    [Header("Player Attributes")]
    public Transform player;

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

    private Dictionary<string, byte> blockTypeLookUp;

    private ChunkID playerChunk;
    private ChunkID prevPlayerChunk;

    private void Awake()
    {
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

        playerChunk = GetChunkID(player.position);
        prevPlayerChunk = playerChunk;
    }

    private void Update()
    {
        playerChunk = GetChunkID(player.position);
        if (!playerChunk.Equals(prevPlayerChunk))
        {
            UpdateViewDistance();
            prevPlayerChunk = playerChunk;
        }
    }

    private void GenerateWorld()
    {
        int start = -ViewDistanceInChunks;
        int end = ViewDistanceInChunks;
        for (int x = start; x < end; x++)
        {
            for (int z = start; z < end; z++)
            {
                ChunkID chunkId = new ChunkID(x, z);
                chunkDict.Add(chunkId, new Chunk(this, chunkId));
            }
        }    
    }

    private void UpdateViewDistance()
    {
        List<ChunkID> prevActiveChunks = new List<ChunkID>(chunkDict.Keys); 

        int xStart = playerChunk.X - ViewDistanceInChunks;
        int xEnd = playerChunk.X + ViewDistanceInChunks;
        int zStart = playerChunk.Z - ViewDistanceInChunks;
        int zEnd = playerChunk.Z + ViewDistanceInChunks;

        for (int x = xStart; x < xEnd; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                ChunkID chunkId = new ChunkID(x, z);

                if (!chunkDict.ContainsKey(chunkId))
                {
                    chunkDict.Add(chunkId, new Chunk(this, chunkId));
                }
                prevActiveChunks.Remove(chunkId);
            }
        }

        foreach (var chunkId in prevActiveChunks)
        {
            Destroy(chunkDict[chunkId].ChunkObject);
            chunkDict.Remove(chunkId);
        }
    }

    public bool HasSolidVoxel(Vector3 position)
    {
        ChunkID chunkId = GetChunkID(position);

        if (position.y < 0 || position.y > chunkHeight - 1)
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

