using UnityEngine;
using System.Collections;

public class KeyboardControl : MonoBehaviour
{
	private CameraRotater rotater;

	private void Update()
	{
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		rotater = GetComponent<CameraRotater>();
		rotater.SetRotationInput(horizontal, vertical);
	}
}
