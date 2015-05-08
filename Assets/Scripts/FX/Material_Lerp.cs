using UnityEngine;

/// <summary>
/// Lerps between two given materials over time 
/// </summary>
public class Material_Lerp : MonoBehaviour
{
    [SerializeField]
    private Material _material1;
    [SerializeField]
    private Material _material2;
    private Renderer _renderer;
    public float _changeSpeed = 2.0f;
    float _perlinOffset = 0.0f;

    void Start()
    {

        _renderer = GetComponent<Renderer>();
        _renderer.material = _material1;
        _perlinOffset = Random.Range(0.0f, 2.0f);
    }
    void Update()
    {
        float lerp = Mathf.PerlinNoise(((Time.time + _perlinOffset) * _changeSpeed), 0.0f);
        _renderer.material.Lerp(_material1, _material2, lerp);
    }
}
