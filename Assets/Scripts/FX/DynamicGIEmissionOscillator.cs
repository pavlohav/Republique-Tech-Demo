using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Renderer))]
public class DynamicGIEmissionOscillator : MonoBehaviour
{
    [SerializeField]
    private Color _minColor = Color.black;  //color to use at bottom of sin curve

    [SerializeField]
    private Color _maxColor = Color.white;  //color to use at top of sin curve

    [SerializeField, Range(0.0001f, 10)]
    private float _period = 1;  //in seconds

    [SerializeField, Range(0, 1)]
    private float _phase = 0;

    [SerializeField]
    private bool _respectsTimeScale = true;

    private Renderer _renderer;
    private float _elapsedTime = 0;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _elapsedTime += (_respectsTimeScale ? Time.deltaTime : Time.unscaledDeltaTime) / _period;
        _elapsedTime -= Mathf.Floor(_elapsedTime);  //wrap around to keep in 0-1 range to avoid eventual overflow
        
        const float TWO_PI = 2 * Mathf.PI;
        float animValue = Mathf.Sin((_elapsedTime + _phase) * TWO_PI) * 0.5f + 0.5f;

        Color newColor = Color.Lerp(_minColor, _maxColor, animValue);
        DynamicGI.SetEmissive(_renderer, newColor);
    }
}
