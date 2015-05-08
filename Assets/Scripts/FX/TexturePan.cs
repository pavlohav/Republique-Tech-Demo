using UnityEngine;
using System.Collections;

public class TexturePan : MonoBehaviour 
{
    public float width = 0.125f;
    public float hertz;
    private float head;
    private float tail;
    
    private void Start()
    {
        if ( width <= 0 || width >= 1.0f )
        {
            Debug.LogWarning("Texture Pan width must be greater than 0 but less than 1");
        }
        tail = 0.0f;
        head = width;
    }
	private void Update () 
    {
        // Shift the texture to the right every frame.
        // If you use a wrapping texture this will cycle it
        // If you use a clamped texture it will stop
        float advanced = hertz * Time.deltaTime;
        head += advanced;
        tail += advanced;

        Vector2 offset = new Vector2(tail, 0.0f);
        Vector2 scale = new Vector2(width, 1.0f);
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

}