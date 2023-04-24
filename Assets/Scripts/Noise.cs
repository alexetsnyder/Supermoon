using Unity.Mathematics;
using UnityEngine;

public static class Noise
{
    public static float Get2DPerlinNoise(Vector2 position, float scale, float offset)
    {
        return noise.cnoise(new float2(position.x * scale + offset, position.y * scale + offset));
    }
}
