using UnityEngine;
using System.Collections;

public class TextureScroller : MonoBehaviour 
{
    [SerializeField]
    private bool createCopyOfMaterial = false;
    [SerializeField]
    private bool respectTimeScale = true;
	[SerializeField]
	private float scrollingSpeed = 1.0f;
	
	private Renderer rend;
    private Material targetMaterial;
	
	private void Awake ()
	{
		rend = GetComponent<Renderer>();
        targetMaterial = (createCopyOfMaterial ? rend.material : rend.sharedMaterial);
	}

    private void OnDestroy()
    {
        if (createCopyOfMaterial && targetMaterial != null)
        {
            Destroy(targetMaterial);
        }
        rend = null;
        targetMaterial = null;
    }
	
	private void Update ()
	{
		float matOffsetX = targetMaterial.mainTextureOffset.x;
        float matOffsetY = targetMaterial.mainTextureOffset.y;

        matOffsetY += scrollingSpeed * GetElapsedTime();
		
		if (matOffsetY > 1) 
		{
			matOffsetY = matOffsetY - Mathf.Floor(matOffsetY);
		}

        targetMaterial.mainTextureOffset = new Vector2(matOffsetX, matOffsetY);
	}

    private float GetElapsedTime()
    {
        return respectTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
    }
}
