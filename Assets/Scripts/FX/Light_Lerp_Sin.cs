using UnityEngine;

/// <summary>
/// Lerps the color of a light with a smooth sin wave
/// </summary>
public class Light_Lerp_Sin : Light_Lerp_Base
{
    protected override float GetLerp()
    {
        return Mathf.Sin(Time.timeSinceLevelLoad * _changeSpeed);
    }
}