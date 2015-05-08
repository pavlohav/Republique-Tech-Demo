using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class ReflectionProxy : MonoBehaviour 
{
    // This enum will be used to choose a rotation constraint
    // For example Y means that the proxy will rotate only around Y while following the target
    public enum AxisContraint
    {
        X    = 0,
        Y    = 1,
        Z    = 2,
        NONE = 3
    }

    [SerializeField, HideInInspector]
    private bool _shouldFollowCamera = false;
    public bool ShouldFollowCamera
    {
        get { return _shouldFollowCamera; }
        set { _shouldFollowCamera = value; }
    }

    [SerializeField, HideInInspector]
    private AxisContraint _axConstraint = AxisContraint.NONE;
    public AxisContraint AxContraint
    {
        get { return _axConstraint; }
        set { _axConstraint = value; }
    }

#pragma warning disable 0414
    [SerializeField, HideInInspector]
    private Camera _camera = null;
#pragma warning restore 0414

#if UNITY_EDITOR
    public Camera Editor_GetCamera()
    {
        return _camera;
    }
    public void Editor_SetCamera(Camera camera)
    {
        _camera = camera;
    }
#endif

    // This function will make sure that the object is facing the camera accordint to potential axis constraint
    public void UpdateProxy(Transform target)
    {
        if (_shouldFollowCamera)
        {
            // Look At vector
            Vector3 lookAt = target.position - transform.position;

            // Constraint!
            if (_axConstraint != AxisContraint.NONE)
            {
                lookAt[(int)_axConstraint] = 0;
            }

            // Perform the rotation
            //TODO: This works fine but if an object is tilted, than the rotation is lost. Change if needed in the future
            transform.rotation = Quaternion.LookRotation(lookAt);
        }
    }
}
