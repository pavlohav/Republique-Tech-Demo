using UnityEngine;
using System.Collections;
using Util;

public class AnimatedCaustics : MonoBehaviour
{
    [SerializeField, Range(1, 60)]
	private float fps = 30.0f;
    [SerializeField]
	private Texture2D[] frames;
	
	private int frameIndex;
	private Light causticLight;
    private LoopTimer loopTimer;
    private float activefps;

	private void Start()
	{
		causticLight = GetComponent<Light>();
		frameIndex = 0;
        activefps = fps;
        loopTimer = new LoopTimer();
        loopTimer.Variation = 0;
        if (frames.Length > 0)
        {
            SetCurrentFrame(0);
            loopTimer.LoopDuration = GetActiveSecondsPerFrame();
            loopTimer.Running = true;
        }
        else
        {
            loopTimer.Running = false;
        }
	}
    
    private float GetActiveSecondsPerFrame()
    {
        return 1 / activefps;
    }

    private void Update()
    {
        if (!MathAux.Approx(fps, activefps, 0.1f))
        {
            activefps = fps;
            loopTimer.LoopDuration = GetActiveSecondsPerFrame();
        }

        int framesElapsed = loopTimer.Update(Time.deltaTime);
        if (framesElapsed > 0)
        {
            SetCurrentFrame(frameIndex + framesElapsed);
        }
    }

    private void SetCurrentFrame(int index)
    {
        frameIndex = index % frames.Length;
        causticLight.cookie = frames[frameIndex];
    }
}
