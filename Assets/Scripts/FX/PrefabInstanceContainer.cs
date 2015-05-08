#if UNITY_EDITOR || CAMO_DEBUG
    #define ENABLE_WARNING_LOGGING
#else
    #undef ENABLE_WARNING_LOGGING
#endif

using UnityEngine;
using System.Collections;

[System.Serializable]
public class PrefabInstanceInstantiator
{
    [SerializeField]
    private GameObject _prefab;
    [SerializeField]
    private Transform _attachToBone;
    [SerializeField]
    private Vector3 _positionOffset;
    [SerializeField]
    private Vector3 _eulerRotation;
    [SerializeField]
    private Vector3 _localScale = Vector3.one;

    private GameObject _instance;

    /// <summary>
    /// Gets or instantiates the instance of the prefab.
    /// </summary>
    public T SpawnInstance<T>() where T : Component
    {
        T component;

        if (_instance != null)
        {
            component = _instance.GetComponent<T>();
        }
        else if (_prefab != null && _attachToBone != null)
        {
            _instance = GameObject.Instantiate(_prefab) as GameObject;
            component = _instance.GetComponent<T>();
            Transform transform = component.transform;
            transform.parent = _attachToBone;
            transform.localPosition = _positionOffset;
            transform.localRotation = Quaternion.Euler(_eulerRotation);
            transform.localScale = _localScale;
        }
        else
        {
#if ENABLE_WARNING_LOGGING
            Debug.LogWarning("[PrefabInstanceInstantiator] Cannot instantiate prefab due to null references.");
#endif
            _instance = null;
            component = null;
        }
        return component;
    }
}
