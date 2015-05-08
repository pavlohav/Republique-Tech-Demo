using UnityEngine;
using System.Collections;

public class AnimationShaderDriver : MonoBehaviour 
{
    const string ANIMATED_SLIDER_NAME="_Animated_Slider";
    
    [SerializeField]
    private Animation _animation;

	private Renderer _renderer;
    
    public Animation Animation
    {
        get { return _animation; }
        set { _animation = value; }
    }
    
    [SerializeField,Range(0,1)]
    private float _initValue = 0;
    
    [SerializeField]
    private float _availableBeginValue = 0.5f;
    
    [SerializeField]
    private float _availableEndValue = 1.0f;
    
    void Awake()
    {
        float normalizedTime=NormalizeNumber(_initValue,1.0f, _availableBeginValue, _availableEndValue);
		_renderer = GetComponent<Renderer>();
		_renderer.sharedMaterial.SetFloat(ANIMATED_SLIDER_NAME,normalizedTime);
    }
    
	// Update is called once per frame
	void Update () 
    {
        if(_animation)
        {
            AnimationState openAnimationState=_animation[_animation.clip.name];
            if(openAnimationState!=null)
            {
                if(openAnimationState.enabled)
                {
                    float normalizedTime=openAnimationState.normalizedTime;
                    normalizedTime=normalizedTime-Mathf.Floor(normalizedTime);
                    normalizedTime=NormalizeNumber(normalizedTime,1.0f, _availableBeginValue, _availableEndValue);
					_renderer.sharedMaterial.SetFloat(ANIMATED_SLIDER_NAME,normalizedTime);
                }
            }
        }
        else
        {
            Debug.LogError("Animation is not set up");
        }
       
	}
    
    float NormalizeNumber(float number, float rangeBefore, float beginOfRangeAfter, float endOfRangeAfter)
    {
        float rangeAfter=endOfRangeAfter-beginOfRangeAfter;
        return number/rangeBefore*rangeAfter+ beginOfRangeAfter;
    }
}
