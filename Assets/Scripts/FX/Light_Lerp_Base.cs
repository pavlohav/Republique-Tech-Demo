using UnityEngine;

/// <summary>
/// Lerps between two given colors to change a light's color over time 
/// </summary>
public abstract class Light_Lerp_Base : MonoBehaviour
{
    private Light _areaLight;
    private Light _light;

    [SerializeField]
    private Color _colorStart;
    [SerializeField]
    private Color _colorEnd;
    [SerializeField]
    protected float _changeSpeed = 2.0f;
    
    private void Start() 
    {
        // Prefer alloy area light of Unity default light.
        _areaLight = GetComponent<Light>();
        if (_areaLight != null)
        {
            _areaLight.color = _colorStart;
        }
        else
        {
            _light = GetComponent<Light>();
            if (_light != null)
            {
                _light.color = _colorStart;
            }
#if UNITY_EDITOR || CAMO_DEBUG
            else
            {
                Debug.LogWarning("[Light_Lerp] No light.", this);
            }
#endif
        }

        Initialize();
    }

    /// <summary>
    /// Use this function for custom initialization during Start
    /// </summary>
    protected virtual void Initialize()
    {
    }
    
    private void Update() 
    {   
        float lerp = GetLerp();
        Color currentColor = Color.Lerp(_colorStart, _colorEnd, lerp);
        if (_areaLight != null)
        {
            _areaLight.color = currentColor;
        }
        else if (_light != null)
        {
            _light.color = currentColor;
        }
    }

    protected abstract float GetLerp();
}