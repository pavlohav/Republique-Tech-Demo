using UnityEngine;
using System.Collections;

public class TexturePanAround : MonoBehaviour 
{
    public float zoomFactor = 0.75f;
    public float hertz;

    private float currentTime;
    private float quarterTime;

    private Vector2 offset;
    private Vector2 scale;
    private float u;
    private float v;

    private void Start()
    {
        if (zoomFactor >= 0.0f || zoomFactor <= 0.75f)
        {
            Debug.LogWarning("Zoom Factor must be greater than or equal to 0.0 but less than or equal to 0.75f or you will read outside the texture", this);
        }

        currentTime = 0.0f;
        if (hertz != 0)
        {
            quarterTime = (1.0f / hertz) / 4.0f;
        }
        else
        {
            quarterTime = 0.0f;
        }
        scale = new Vector2(zoomFactor, zoomFactor);
        offset = new Vector2(0.0f, 0.0f);
    }
	private void Update () 
    {
        if (currentTime <= quarterTime)
        {
            // pan right!
            u += (Time.deltaTime * hertz);
            currentTime += Time.deltaTime;
        }
        else if (currentTime <= (quarterTime * 2.0f))
        {
            //pan down!
            v += (Time.deltaTime * hertz);
            currentTime += Time.deltaTime;
        }
        else if (currentTime <= (quarterTime * 3.0f))
        {
            //pan Left
            u -= (Time.deltaTime * hertz);
            currentTime += Time.deltaTime;
        }
        else if (currentTime <= (quarterTime * 4.0f))
        {
            //Pan up
            v -= (Time.deltaTime * hertz);
            currentTime += Time.deltaTime;
        }
        else
        {
            // reset!
            u = 0.0f;
            v = 0.0f;
            currentTime = 0.0f; ;
        }
        scale = new Vector2(zoomFactor, zoomFactor);
        offset = new Vector2(u, v);
       
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
	}

}