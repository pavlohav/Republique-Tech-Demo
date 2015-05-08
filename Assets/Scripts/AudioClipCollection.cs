using UnityEngine;
using System.Collections;

public class AudioClipCollection : ScriptableObject
{
	[SerializeField]
	private AudioClip[] clips;

	public AudioClip this[int number]
	{
		get
		{
			return clips[number];
		}
	}

	public AudioClip GetRandom()
	{
		if (clips.Length < 1) return null;

		int randomIndex = Random.Range(0, clips.Length); // inclusive exclusive
		return clips[randomIndex];
	}
}
