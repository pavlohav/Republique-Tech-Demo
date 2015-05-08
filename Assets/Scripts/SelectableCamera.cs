using UnityEngine;

public class SelectableCamera : MonoBehaviour
{
	static private SelectableCamera currentCamera;

	public static SelectableCamera CurrentCamera {
		get 
		{
			return currentCamera;
		}
	}

	public bool IsActiveCamera
	{
		get
		{
			return this == currentCamera;
		}
	}

	private void Awake()
	{
		ActivateCamera(false);
	}

	public void Switch()
	{
		if (IsActiveCamera) return;

		SelectableCamera previous = currentCamera;

		currentCamera = this;
		this.ActivateCamera(true);

		if (previous != null)
		{
			previous.ActivateCamera(false);
		}
	}

	private void ActivateCamera(bool activate)
	{
		gameObject.SetActive(activate);
	}
    
    private void OnDestroy()
    {
        // prevent memory leak
        if (this == currentCamera)
        {
            currentCamera = null;
        }
    }
}