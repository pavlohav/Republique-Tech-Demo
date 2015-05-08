using UnityEngine;
using System.Collections;

/// <summary>
/// The purpose of the proxy camera is to generate a texture for the proxy that will be used to generate reflection for a specific part of the world. 
/// </summary>.
[RequireComponent(typeof(Camera))]
public class ProxyCamera : MonoBehaviour 
{
    // This is the UI layer
    private const int LAYER = 5;

    [SerializeField, HideInInspector]
    private ReflectionProxy _targetProxy = null;
    public ReflectionProxy TargetProxy
    {
        get { return _targetProxy; }
        set { _targetProxy = value; }
    }

    [SerializeField]
    private Renderer[] _objectsToRender = new Renderer[0];
    public Renderer[] ObjectsToRender
    {
        get { return _objectsToRender; }
        set { _objectsToRender = value; }
    }

    // Set the camera culling mask
    public void SetCullingMask()
    {
        Camera cam = gameObject.GetComponent<Camera>();
        cam.cullingMask = 1 << LAYER;
    }

    // Render the selected geometries into the proxy texture
    // NOTE: Use this function carefully. It could create issues with the Layer of the objects to render, if used unproperly
    public void ProxyRender()
    {
        Camera cam = gameObject.GetComponent<Camera>();

        // Store the actual layer mask into this array
        int[] originalLayers = new int[_objectsToRender.Length];

        // Save and set the new layer
        SaveAndSetLayer(ref originalLayers, ref _objectsToRender);

        // Render
        InternalRender(cam);

        // Reset the original layer
        ResetOriginalLayer(ref originalLayers, ref _objectsToRender);
    }

    // Save the original layer of the objects and temporarly set it to our custom one
    public static void SaveAndSetLayer(ref int[] layers, ref Renderer[] objects)
    {
        // This needs to be done in TWO steps
        // To avoid issues with the same object being present into two spots
        for (int objectsIndex = 0; objectsIndex < objects.Length; ++objectsIndex)
        {
            // Make sure that the rederer reference is not null
            if (objects[objectsIndex] != null)
            {
                layers[objectsIndex] = objects[objectsIndex].gameObject.layer;
            }
            else
            {
                Debug.LogWarning("[ProxyCamera] There is a null reference in the array of objects to render. Please check the array of objects to render related to the proxy image. " +
                                 "If you are editing and you are aware of this, you can diregard the warning");
            }
        }

        // Change the layer
        for (int objectsIndex = 0; objectsIndex < objects.Length; ++objectsIndex)
        {
            if (objects[objectsIndex] != null)
            {
                objects[objectsIndex].gameObject.layer = LAYER;
            }
            // There is no need for another Log Warning
        }
    }

    // Reset back the original layer
    public static void ResetOriginalLayer(ref int[] layers, ref Renderer[] objects)
    {
        for (int objectsIndex = 0; objectsIndex < objects.Length; ++objectsIndex)
        {
            // Make sure that the renderer reference is not null
            if (objects[objectsIndex] != null)
            {
                objects[objectsIndex].gameObject.layer = layers[objectsIndex];
            }
            else
            {
                // Should this be a warning
                Debug.LogError("[ProxyCamera] There is a null reference in the array of objects to render. Please check the array of objects to render related to the proxy image.");
            }
        }
    }

    // Perform the render
    private void InternalRender(Camera cam)
    {
        // Get the material from the proxy renderer
        Material targetMat = _targetProxy.GetComponent<Renderer>().sharedMaterial;
          
        // Temp Renderer, will be deleted at the end of the rendering
        cam.targetTexture = RenderTexture.GetTemporary(targetMat.mainTexture.width, targetMat.mainTexture.height, 32, RenderTextureFormat.ARGB32);

        // Before rendering we want to make sure that proxy itself will NOT be rendered
        // Disable the renderer will do the job
        bool renderState = _targetProxy.gameObject.GetComponent<Renderer>().enabled;
        _targetProxy.gameObject.GetComponent<Renderer>().enabled = false;

        cam.Render();

        // Reset the render state
        _targetProxy.gameObject.GetComponent<Renderer>().enabled = renderState;

        // This is needed to read the render target
        RenderTexture.active = cam.targetTexture;

        // Assume the target is a Texture2D
        if (targetMat.mainTexture is Texture2D)
        {
            Texture2D texture = targetMat.mainTexture as Texture2D;
            texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            texture.Apply();
        }
        else
        {
            Debug.LogError("[ProxyCamera] Target texture is not a Texture2D");
        }

        // Release the temporary texture
        RenderTexture.ReleaseTemporary(cam.targetTexture);
        cam.targetTexture = null;
    }
}
