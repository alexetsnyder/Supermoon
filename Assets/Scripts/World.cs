using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("Chunk Attributes")]
    public int worldSize;
    public int ChunkSize;
    public int ChunkHeight;

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

        foreach (var chunkId in chunkDict.Keys)
        {
            Debug.Log(chunkDict[chunkId]);
        }
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

    public byte GetBlockID(string name)
    {
        return blockTypeLookUp[name];
    }
}

