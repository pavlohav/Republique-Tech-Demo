using UnityEngine;
using System.Collections;

public class CountOnEnable : MonoBehaviour
{
	// Do not change this data at runtime or the counts will get screwed up.
	[SerializeField] private CounterBehaviour[] counters;
	[SerializeField] private bool invert = false;

	private void OnEnable()
	{
		Count(!invert);
	}

	private void OnDisable()
	{
		Count(invert);
	}

	private void Count(bool up)
	{
		if (counters != null)
		{
			for (int i = 0; i < counters.Length; ++i)
			{
				if (counters[i] != null)
				{
					counters[i].Increment(up);
				}
			}
		}
	}
}
