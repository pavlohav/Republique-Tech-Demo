using UnityEngine;
using System.Collections.Generic;

public abstract class MaterialParameterPasser : MonoBehaviour 
{
    [SerializeField]
    private List<Material> _targetMaterials;

    [SerializeField]
    private bool _applyOnAwake;

    private void Awake()
    {
        if (_applyOnAwake)
        {
            ApplyToMaterials();
        }
    }

    public void ApplyToMaterials()
    {
        if (_targetMaterials != null)
        {
            for (int i = 0; i < _targetMaterials.Count; ++i)
            {
                if (_targetMaterials[i] != null)
                {
                    ApplyToMaterial(_targetMaterials[i]);
                }
#if UNITY_EDITOR || CAMO_DEBUG
                else
                {
                    Debug.LogError("[MaterialParameterPasser] \"" + name + "\" has"
                        + " null material at index " + i + ".  Is this a mistake?",
                        this);
                }
#endif
            }
        }
#if UNITY_EDITOR || CAMO_DEBUG
        else
        {
            Debug.LogError("[MaterialParameterPasser] \"" + name + "\"'s target"
                + " material list is null. This script is pointless if it is not"
                + " targeting any materials.", this);
        }
#endif
    }

    protected abstract void ApplyToMaterial(Material mat);
}
