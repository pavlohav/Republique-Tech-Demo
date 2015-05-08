using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraRotater))]
public class FramePlayerOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		CameraRotater rotater = GetComponent<CameraRotater>();
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player)
		{
			rotater.ApplyRotationToFramePoint(player.transform.position, 0.5f);
		}
	}
}
