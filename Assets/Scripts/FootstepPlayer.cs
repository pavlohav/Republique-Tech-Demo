using UnityEngine;
using System.Collections;

public class FootstepPlayer : MonoBehaviour
{
	[SerializeField]
	private AudioClipCollection clips;
	[SerializeField]
	private AudioSource sourceSettings;
	private Animator animator;

	private AudioSource left;
	private AudioSource right;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		Transform foot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
		left = GameObject.Instantiate<AudioSource>(sourceSettings);
		Attatch(left.transform, foot);

		foot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
		right = GameObject.Instantiate<AudioSource>(sourceSettings);
		Attatch(right.transform, foot);
	}

	static private void Attatch(Transform child, Transform parent)
	{
		child.parent = parent;
		child.localPosition = Vector3.zero;
		child.localRotation = Quaternion.identity;
	}

	public void PlayFootstepLeft(int dummy)
	{
		left.PlayOneShot(clips.GetRandom(), GetRandomVolumeScale());
	}

	public void PlayFootstepRight(int dummy)
	{
		right.PlayOneShot(clips.GetRandom(), GetRandomVolumeScale());
	}

	static private float GetRandomVolumeScale()
	{
		const float minRandom = 0.7f;
		return Random.Range(minRandom, 1f);
	}
}
