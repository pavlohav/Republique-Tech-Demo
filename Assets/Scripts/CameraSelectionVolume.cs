
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Trigger logic for handling when to switch cameras and what to switch to based on trigger volumes.
/// Supports overlapping triggers and cameras that don't need to be switched from.
/// </summary>
public class CameraSelectionVolume : MonoBehaviour
{
	/// <summary>
	/// The first camera in the array is the camera that we try to switch to.
	/// The others are considered 'valid' cameras: ones we do not switch from.
	/// </summary>
	[SerializeField]
	private SelectableCamera[] selectableCameras;
	private List<Collider> collidersContained;
	
	// List of triggers containing the player is managed by individual instances registering and unregistering.
	private static List<CameraSelectionVolume> triggersContainingPlayer = new List<CameraSelectionVolume>();
	private static CameraSelectionVolume lastAttemptedSwitch = null;

	private static bool autoSwitchesEnable = true;
	public static bool AutoSwitchesEnabled { get { return autoSwitchesEnable; } set { autoSwitchesEnable = value; } }

	private void Awake()
	{
		if (selectableCameras == null)
		{
			selectableCameras = new SelectableCamera[0];
		}
		
		collidersContained = new List<Collider>();
	}

	private void OnDisable()
	{
		// necessary to prevent memory leaks by static variables
		triggersContainingPlayer.Remove(this);
		if (this == lastAttemptedSwitch)
		{
			lastAttemptedSwitch = null;
		}

		// the trigger contents is purged so we must do the same for our list
		collidersContained.Clear();
	}
	
	private void OnTriggerEnter (Collider other)
	{
		if (IsPlayer(other))
		{
			if (!collidersContained.Contains(other))
			{
				collidersContained.Add(other);

				if (!triggersContainingPlayer.Contains(this))
				{
					triggersContainingPlayer.Add(this);
					SelectCamAndAttemptSwitch();
				}
			}
		}
		// else dont care about whatever this was
	}
	
	private bool IsPlayer(Collider other)
	{
		return other.gameObject.CompareTag("Player");
	}
	
	private void OnTriggerExit (Collider other)
	{
		if (collidersContained.Contains(other))
		{
			// remove the collider from our list
			collidersContained.Remove(other);

			// if the list is empty then we are no longer containing the player
			if (collidersContained.Count < 1)
			{
				if (triggersContainingPlayer.Remove(this))
				{
					// perform a switch here
					SelectCamAndAttemptSwitchAll();
				}
			}
		}
	}
	
	static private bool CameraIsAcceptable(SelectableCamera cam)
	{
		return true; // can specify criteria here
	}
	
	static private CameraSelectionVolume GetFirstTriggerWithAcceptableCamera()
	{
		CameraSelectionVolume firstAcceptable = null;
		for (int i = 0; i < triggersContainingPlayer.Count; ++i)
		{
			CameraSelectionVolume autoSwitch = triggersContainingPlayer[i];
			if (autoSwitch.GetFirstAcceptableCamera() != null)
			{
				firstAcceptable = autoSwitch;
				break;
			}
			// else cannot select this trigger.
		}
		
		return firstAcceptable;
	}
	
	private SelectableCamera GetFirstAcceptableCamera()
	{
		SelectableCamera firstAcceptable = null;
		
		for (int i = 0; i < selectableCameras.Length; ++i)
		{
			SelectableCamera cam = selectableCameras[i];
			if (CameraIsAcceptable(cam))
			{
				// Found an acceptable camera.
				firstAcceptable = cam;
				break;
			}
		}
		
		return firstAcceptable;
	}
	
	// Go through all the camera switch triggers containing the player (shouldnt be many) and try to find the best camera to switch to.
	// This function is static since we need to treat trigger exits on one trigger like they are trigger enters on another.
	static private void SelectCamAndAttemptSwitchAll()
	{
		CameraSelectionVolume autoSwitch = GetFirstTriggerWithAcceptableCamera();
		
		// If there were any then lets try to switch to one!
		if (autoSwitch != null)
		{
			autoSwitch.SelectCamAndAttemptSwitch();
		}
		// else No acceptable cameras were found.
	}
	
	/// <summary>
	/// Attempts the switch on enter. Will only switch to the first camera on the newly hit trigger.
	/// </summary>
	private void SelectCamAndAttemptSwitch()
	{
		SelectableCamera firstAcceptable = GetFirstAcceptableCamera();
		if (firstAcceptable != null)
		{
			if (AutoSwitchesEnabled)
			{
				lastAttemptedSwitch = this;
				AttemptSwitch(firstAcceptable);
			}
		}
	}
	
	static private bool CheckOnAcceptableCamera()
	{
		// First determine if we are on any camera in the acceptable list.
		bool activeIsAcceptable = false;
		for (int triggerIndex = 0; triggerIndex < triggersContainingPlayer.Count; ++triggerIndex)
		{
			CameraSelectionVolume trigger = triggersContainingPlayer[triggerIndex];
			for (int camIndex = 0; camIndex < trigger.selectableCameras.Length; camIndex++)
			{
				SelectableCamera cam = trigger.selectableCameras[camIndex];
				activeIsAcceptable |= CameraIsAcceptable(cam) && cam.IsActiveCamera;
			}
		}
		return activeIsAcceptable;
	}
	
	/// <summary>
	/// Switches to the camera if we need to (depends on what camera we currently are on).
	/// </summary>
	private void AttemptSwitch(SelectableCamera switchTarget)
	{
		if (!CheckOnAcceptableCamera())
		{
			switchTarget.Switch();
		}
	}
}