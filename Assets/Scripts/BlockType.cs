using UnityEngine;

[System.Serializable]
public class BlockType
{
    public string name;
    public bool isSolid;

    public string frontTextureName;
    public string backTextureName;
    public string topTextureName;
    public string bottomTextureName;
    public string leftTextureName;
    public string rightTextureName;

    public string GetTextureName(int face)
    {
        switch (face)
        {
            case 0:
                return frontTextureName;
            case 1:
                return backTextureName;
            case 2:
                return topTextureName;
            case 3:
                return bottomTextureName;
            case 4:
                return leftTextureName;
            case 5:
                return rightTextureName;
            default:
                return "";
        }
    }
}
