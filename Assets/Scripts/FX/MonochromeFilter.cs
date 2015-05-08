using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class MonochromeFilter : MonoBehaviour 
{
    [SerializeField]
    private Material blackAndWhiteMaterial;

    private bool enabledByControlState = false;
    public bool EnabledByControlState
    {
        get { return enabledByControlState; }
        set
        {
            enabledByControlState = value;
            UpdateEnabled();
        }
    }

    private bool enableByEffectEvent = false;
    public bool EnableByEffectEvent
    {
        get { return enableByEffectEvent; }
        set
        {
            enableByEffectEvent = value;
            UpdateEnabled();
        }
    }

    private void UpdateEnabled()
    {
        enabled = enabledByControlState | enableByEffectEvent;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, blackAndWhiteMaterial);
    }
}
