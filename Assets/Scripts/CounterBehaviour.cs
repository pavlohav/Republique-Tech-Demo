using UnityEngine;
using System.Collections;

public abstract class CounterBehaviour : MonoBehaviour
{
	private int count = 0;
	protected int Count { get { return count; } }

	/// <summary>
	/// Increments the counter.
	/// </summary>
	public void Increment()
	{
		++count;
		OnCounterChanged();
	}
	/// <summary>
	/// Increments or decrements the counter.
	/// </summary>
	/// <param name="up">If set to <c>true</c> increments. Otherwise decrements.</param>
	public void Increment(bool up)
	{
		if (up)
		{
			Increment();
		}
		else
		{
			Decrement();
		}
	}
	/// <summary>
	/// Decrements the counter.
	/// </summary>
	public void Decrement()
	{
		--count;
		OnCounterChanged();
		if (count < 0)
		{
			Debug.LogError("Counter went negative. A client decreased the counter too much.", this);
		}
	}

	protected abstract void OnCounterChanged();

}
