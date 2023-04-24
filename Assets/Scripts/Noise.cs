using Unity.Mathematics;
using UnityEngine;

public static class Noise
{
    public static float Get2DPerlinNoise(Vector2 position, float scale, float offset)
    {
        float2 point = new float2(position.x * scale + offset, position.y * scale + offset);
        return math.remap(-1.0f, 1.0f, 0.0f, 1.0f, noise.cnoise(point));
    }
}
