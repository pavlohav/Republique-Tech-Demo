using UnityEngine;
using System.Collections;

public class TextureAnim : MonoBehaviour 
{
    // In order to save memory we can hold the video on a specific frame, 
    // This is useful when a specific frame is basically static for most of the video
    private enum PlaybackState
    {
        Playing = 0,
        HoldFrame = 1,
    }
	public int colCount = 4;
	public int rowCount = 4;
	public int totalCells = 4;
	public int fps = 15;
    public int startCell = 0;
	public float frameHoldTime = 0.0f;
    private float time;
    private Vector2 offset;
    private Vector2 size;
    private float videoLength;
    private PlaybackState playBackState;
    private bool holdFrame;
    private int framesPerTexture;

    public Texture[] textures;
	private Renderer cachedRenderer;
    private int currentTexture;
    
    // These values helps keep the animated textures lined up in a way that looks ok, so they don't read from the next or previous cell for example
    public float offsetFudgeFactor = 0.01f;
    private Vector2 fudgedSize;

	private void Awake()
	{
		cachedRenderer = GetComponent<Renderer>();
	}

    private void Start()
    {
        size = new Vector2((1.0f / colCount), (1.0f / rowCount));
        fudgedSize = new Vector2((size.x - offsetFudgeFactor), (size.y - offsetFudgeFactor));

        time = 0.0f;
        videoLength = totalCells * 1.0f / fps;
        if (frameHoldTime > 0.01f)
        {
            holdFrame = true;
        }
        else
        {
            holdFrame = false;
        }

        playBackState = PlaybackState.Playing;


        currentTexture = 0;
        if (textures != null)
        {
            if (textures.Length > 0)
            {
                framesPerTexture = totalCells / textures.Length;
                cachedRenderer.material.mainTexture = textures[0];
            }
        }
        else
        {
            framesPerTexture = totalCells;
        }

    }
	private void Update () 
    {
        if (playBackState == PlaybackState.Playing)
        {
            UpdateAnimation();
            if (holdFrame)
            {
                if (time > videoLength)
                {
                    playBackState = PlaybackState.HoldFrame;
                    time = 0.0f;
                    HoldFrame();
                }
            }
        }
        else if (playBackState == PlaybackState.HoldFrame)
        {
           
            if (time > frameHoldTime)
            {
                playBackState = PlaybackState.Playing;
                time = 0.0f;
            }
        }
        time += Time.deltaTime;
	}

    private void UpdateAnimation()
    {
		int index = Mathf.RoundToInt(Time.time*fps);
		index = index%totalCells;

        if ( textures.Length > 1 )
        {
            int textureIndex = index / framesPerTexture;
            if (textureIndex != currentTexture)
            {
                currentTexture = textureIndex;
				cachedRenderer.material.mainTexture = textures[currentTexture];
            }
        }

        float uIndex = Mathf.Abs((index + startCell) % colCount)  + offsetFudgeFactor;
        float vIndex = Mathf.Abs((index + startCell) / colCount) + offsetFudgeFactor;
		offset = new Vector2((uIndex)*size.x,((1.0f-size.y)-(vIndex)*size.y) + offsetFudgeFactor/2.0f);;

		cachedRenderer.material.SetTextureOffset("_MainTex",offset);
		cachedRenderer.material.SetTextureScale("_MainTex", fudgedSize);
	}

    private void HoldFrame ()
    {
        int index = totalCells - 1;
        float uIndex = Mathf.Abs((index + startCell) % colCount);
        float vIndex = Mathf.Abs((index + startCell) / colCount);
        offset = new Vector2((uIndex) * size.x, (1.0f - size.y) - (vIndex) * size.y);

		cachedRenderer.material.SetTextureOffset("_MainTex", offset);
		cachedRenderer.material.SetTextureScale("_MainTex", fudgedSize);
    }
}