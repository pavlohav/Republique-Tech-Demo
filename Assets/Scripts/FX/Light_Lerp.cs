using UnityEngine;

/// <summary>
/// Lerps between two given colors to change a light's color over time 
/// </summary>
public class Light_Lerp : Light_Lerp_Base
{
    private float _offset;

    protected override void Initialize()
    {
        _offset = Random.Range(0.0f, 2.0f);
    }

    protected override float GetLerp()
    {
        return Mathf.PerlinNoise(((Time.timeSinceLevelLoad + _offset) * _changeSpeed), 0.0f);
    }
}