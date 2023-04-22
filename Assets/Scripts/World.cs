using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("Chunk Attributes")]
    public int ChunkSize;
    public int ChunkHeight;

    [Header("Block Attributes")]
    public Material blockMaterial;
    public BlockType[] blockTypeArray;

    public TextureAtlas Atlas { get; private set; }

    private List<Chunk> activeChunks;

    private void Awake()
    {
        activeChunks = new List<Chunk>();
        Atlas = GetComponent<TextureAtlas>();
    }

    private void Start()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        Chunk chunk = new Chunk(this);
        activeChunks.Add(chunk);
    }
}

