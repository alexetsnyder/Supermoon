using Unity.Mathematics;
using UnityEngine;

[System.Serializable] 
public class Noise
{
    public float waveLength;
    public int octaves;

    public float Frequency
    {
        get
        {
            return 1.0f / waveLength;
        }

        set
        {
            waveLength = 1 / value;
        }
    }

    public Noise(float waveLength, int octaves)
    {
        this.waveLength = waveLength;
        this.octaves = octaves;
    }

    public float PNoise(float x, float y)
    {
        float a = 0, frequency = Frequency, amplitude = 1.0f, range = 0.0f;

        for (int octave = 0; octave < octaves; octave++)
        {
            a += Mathf.PerlinNoise(frequency * x, frequency * y) * amplitude;

            range += amplitude;
            frequency *= 2.0f;
            amplitude *= 0.5f;
        }

        return a / range;
    }

    public float SNoise(float x, float y)
    {
        float a = 0, frequency = Frequency, amplitude = 1.0f, range = 0.0f;

        for (int octave = 0; octave < octaves; octave++)
        {
            float z = math.remap(-1.0f, 1.0f, 0.0f, 1.0f, noise.snoise(new float2(frequency * x, frequency * y)));
            z *= amplitude;
            a += z;

            range += amplitude;
            frequency *= 2.0f;
            amplitude *= 0.5f;
        }

        return a / range;
    }
}
