using UnityEngine;
using System.Collections;

public class ActivateByCounter : CounterBehaviour
{
	[SerializeField] private bool invert = false;

	private void Awake()
	{
		OnCounterChanged();
	}

	protected override void OnCounterChanged()
	{
		gameObject.SetActive(invert ^ Count > 0);
	}
}
