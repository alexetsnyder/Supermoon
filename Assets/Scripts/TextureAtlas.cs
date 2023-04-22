using System.Collections.Generic;
using UnityEngine;

public class TextureAtlas : MonoBehaviour 
{
    public int TextureSize;
    public int TextureBlockSize;

    public TextureSwatch[] atlasTextures;
    public Dictionary<string, int> atlasTextureLookUp;

    private int TextureBlockCount
    {
        get
        {
            return TextureSize / TextureBlockSize;
        }
    }
    private float NormalizedTextureBlockSize
    {
        get
        {
            return (float)TextureBlockSize / TextureSize;
        }
    }

    private void Awake()
    {
        atlasTextureLookUp = new Dictionary<string, int>();
        foreach (var tex in atlasTextures)
        {
            atlasTextureLookUp.Add(tex.name, tex.index);
        }
    }

    public Vector2[] GetUVArray(string name)
    {
        return GetUVArrayFromIndex(atlasTextureLookUp[name]);
    }

    private Vector2[] GetUVArrayFromIndex(int index)
    {
        Vector2[] uvArray = new Vector2[4];

        float y = index / TextureBlockCount;
        float x = (index - TextureBlockCount * y);

        y *= NormalizedTextureBlockSize;
        x *= NormalizedTextureBlockSize;

        uvArray[0] = new Vector2(x, y);
        uvArray[1] = new Vector2(x, y + NormalizedTextureBlockSize);
        uvArray[2] = new Vector2(x + NormalizedTextureBlockSize, y);
        uvArray[3] = new Vector2(x + NormalizedTextureBlockSize, y + NormalizedTextureBlockSize);

        return uvArray;
    }
}

[System.Serializable]
public class TextureSwatch
{
    public string name;
    public int index;
}
