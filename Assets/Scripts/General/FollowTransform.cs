using UnityEngine;
using System.Collections;

public class FollowTransform : MonoBehaviour
{
    [SerializeField]
    private Transform transformToFollow;
    
    private void Awake()
    {
        if (transformToFollow != null)
        {
            transform.parent = transformToFollow;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
    
    
}
