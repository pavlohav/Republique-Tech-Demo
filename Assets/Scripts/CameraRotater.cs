using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
#endif

public class CameraRotater : MonoBehaviour
{
	[SerializeField]
	private Transform pivot;
	[SerializeField]
	private Camera frameCamera;

    [SerializeField]
    private bool angleLimitsAroundPivot = false;
    // The next three vectors are used for limiting the camera rotation
    // Their values depend if this camera angle limits are in reference of its pivot or main gameobject
    private Vector3 forwardReference;
    private Vector3 rightReference;
    private float pivotVerticalAngleOffset;
    private float pivotHorizontalAngleOffset;

	[SerializeField]
	private Vector2 horizontalRotationLimit = new Vector2(-45.0f, 45.0f);
	[SerializeField]
	private Vector2 verticalRotationLimit = new Vector2(-10f, 10f);
	private Vector4 rotationLimit; // just another representation of the above two

    private Vector2 currentPivotAngle;

    [SerializeField, Range(1, 200)]
    private float speedLimit = 74;

    //time to accelerate from rest to max speed in seconds
    [SerializeField, Range(0, 5)]
    private float accelerationTime = 0.37f;  

    //used to calculate deceleration time as a function of of acceleration time
    //ex: 0.5 would be it should take 50% of the time to decelerate as it takes to
    //accelerate
    [SerializeField, Range(0, 2)]
    private float decelerationTimeProportion = 0.5714f;

    private Vector2 freeAxisInput = Vector2.zero;  //hold ths input vector from the user (must be a vector bounded by unit circle)
    private Vector2 angularVelocity = Vector2.zero;
    private Vector2 previousAngularVelocity = Vector2.zero;  //used to integrate more accurately
	private Quaternion initialRotation;

    private float FreeAxisAcceleration
    {
        get { return speedLimit / accelerationTime; }
    }

    private float FreeAxisDeceleration
    {
        get { return FreeAxisAcceleration / decelerationTimeProportion; }
    }

    private void Awake()
    {
        SetupPivot();
    }

	private void OnValidate()
	{
		horizontalRotationLimit.x = Mathf.Clamp(horizontalRotationLimit.x, -180f, 0f);
		horizontalRotationLimit.y = Mathf.Clamp(horizontalRotationLimit.y, 0f, 180f);
		verticalRotationLimit.x = Mathf.Clamp(verticalRotationLimit.x, -180f, 0f);
		verticalRotationLimit.y = Mathf.Clamp(verticalRotationLimit.y, 0f, 180f);
	}

    private void UpdateReferenceVectors()
    {
        // Update reference vectors according to checkbox set on inspector.
        // They might be in reference to camera's pivot or the main game object itself
        if (angleLimitsAroundPivot)
        {
            forwardReference = pivot.transform.forward;
            rightReference = pivot.transform.right;
        }
        else
        {
            forwardReference = transform.forward;
            rightReference = transform.right;
        }
    }

    private void SetupPivot()
    {
        //For our float math checks
        const float marginOfError = 0.01f;
		rotationLimit = new Vector4(horizontalRotationLimit.x, horizontalRotationLimit.y, verticalRotationLimit.x, verticalRotationLimit.y);

        if (pivot != null)
        {
            UpdateReferenceVectors();

            // Vertical and horizontal angle differences between pivot and gameobject forward to assist rotation limit calculations
            if (angleLimitsAroundPivot)
            {
                pivotVerticalAngleOffset = MathUtil.AngleSignedInDegrees(pivot.forward, transform.forward, rightReference);
				pivotHorizontalAngleOffset = MathUtil.AngleSignedInDegrees(forwardReference, transform.forward, transform.up);
            }
            else
            {
                pivotVerticalAngleOffset = 0f;
                pivotHorizontalAngleOffset = 0f;
            }

            // Set up currentPivotAngle based on pivot orientation
            UpdateCurrentPivotAngle();

            float angleOnHorizontal = currentPivotAngle.x;
            if(rotationLimit.x < rotationLimit.y - marginOfError)
            {
                // Check if out of horizontal limits
                if(angleOnHorizontal < rotationLimit.x)
                {
                    // Angle out of limit. Clamp it rotating pivot to its limit
                    float angleDifference = rotationLimit.x - angleOnHorizontal;
                    RotatePivotQuaternion( transform.up, angleDifference );
                    Debug.LogWarning("[CameraRotater] " + this.name + "'s pivot horizontal angle is out of rotation limit range defined. Clamping its pivot forward to closest angle limit: " + rotationLimit.x, this);
                }
                else if(angleOnHorizontal > rotationLimit.y)
                {
                    // Angle out of limit. Clamp it rotating pivot to its limit
                    float angleDifference = rotationLimit.y - angleOnHorizontal;
                    RotatePivotQuaternion( transform.up, angleDifference );
                    Debug.LogWarning("[CameraRotater] " + this.name + "'s pivot horizontal angle is out of rotation limit range defined. Clamping its pivot forward to closest angle limit: " + rotationLimit.y, this);
                }
            }
            else if(rotationLimit.x > rotationLimit.y + marginOfError)
            {
                Debug.LogError("[CameraRotater] " + this.name + ": Rotation Limit X needs to be smaller than its Y!", this);
            }
            //Otherwise our math tells us both rotations are more or less equivalent

            float angleOnVertical = currentPivotAngle.y;
            if(rotationLimit.z < rotationLimit.w - marginOfError)
            {
                // Check if out of vertical limits
                if(angleOnVertical < rotationLimit.z)
                {
                    // Angle out of limit. Clamp it rotating pivot to its limit
                    float angleDifference = rotationLimit.z - angleOnVertical;
                    RotatePivotQuaternion( -pivot.right, angleDifference );
                    Debug.LogWarning("[CameraRotater] " + this.name + "'s pivot vertical angle is out of rotation limit range defined. Clamping its pivot forward to closest angle limit: " + rotationLimit.z, this);
                }
                else if(angleOnVertical > rotationLimit.w)
                {
                    // Angle out of limit. Clamp it rotating pivot to its limit
                    float angleDifference = rotationLimit.w - angleOnVertical;
                    RotatePivotQuaternion( -pivot.right, angleDifference );
                    Debug.LogWarning("[CameraRotater] " + this.name + "'s pivot vertical angle is out of rotation limit range defined. Clamping its pivot forward to closest angle limit: " + rotationLimit.w, this);
                }
            }
            else if(rotationLimit.z > rotationLimit.w + marginOfError)
            {
                Debug.LogError("[CameraRotater] " + this.name + ": Rotation Limit Z needs to be smaller than its W!", this);
            }
            //Otherwise our math tells us both rotations are more or less equivalent

            initialRotation = pivot.rotation;
        }
        else
        {
			Debug.LogError("[CameraRotater] Pivot transform not assigned, camera can't rotate", this);
        }
    }

    //more accurate rotation, does not introduce z-axis drift present in RotatePivot
    private void RotatePivotQuaternion( Vector3 rotationAxis, float angleDelta)
    {
		SetPivotQuaternion( Quaternion.AngleAxis( angleDelta, rotationAxis ) * pivot.rotation );
    }

	private void SetPivotQuaternion(Quaternion rotation)
	{
		pivot.rotation = rotation;
		UpdateCurrentPivotAngle();
	}

    private void UpdateCurrentPivotAngle()
    {
        // Update current pivot angle used for camera rotation caused by game input
        // currentPivotAngle needs to be updated every time the pivot rotation is changed
		float angleOnHorizontal = MathUtil.AngleSignedInDegrees(transform.forward, pivot.forward, transform.up) + pivotHorizontalAngleOffset;
        float angleOnVertical = - pivot.eulerAngles.x - pivotVerticalAngleOffset;
		currentPivotAngle.x = MathUtil.ConvertAngleTo180Range(angleOnHorizontal);
		currentPivotAngle.y = MathUtil.ConvertAngleTo180Range(angleOnVertical);
    }

    private void Update()
    {
		previousAngularVelocity = angularVelocity;
		angularVelocity.x = CalculateSingleAxisVelocity(angularVelocity.x, freeAxisInput.x);
		angularVelocity.y = CalculateSingleAxisVelocity(angularVelocity.y, freeAxisInput.y);
		
		//Use the average of the velocity over the last to frames to correctly integrate the area
		//under the linearly changing velocity.  Not perfect when we change the angle of the line
		//(for example, when reaching the speed threshold), but more accurate than just using current
		//frame's velocity.
		Vector2 averageAngularVelocity = (previousAngularVelocity + angularVelocity) / 2;
		
		Vector2 angleDelta = averageAngularVelocity * Time.unscaledDeltaTime;
		
		RotateHorizontal(angleDelta.x);
		RotateVertical(angleDelta.y);
    }

    /// <summary>
    /// Stops the camera in its place.
    /// </summary>
    public void StopCurrentRotation()
    {
		freeAxisInput = Vector2.zero;
		angularVelocity = Vector2.zero;
		previousAngularVelocity = Vector2.zero;
    }

	/// <summary>
	/// Sets the input values used for rotating the camera.
	/// </summary>
	/// <param name="x">x input</param>
	/// <param name="y">y input</param>
    public void SetRotationInput(float x, float y)
    {
		freeAxisInput.x = x;
		freeAxisInput.y = y;
    }

	/// <summary>
	/// Cancels the rotation.
	/// </summary>
    public void EndRotationInput()
    {
		freeAxisInput = Vector2.zero;
    }

    private float CalculateSingleAxisVelocity(float currentVelocity, float inputValue)
    {
		const float inputRestMarginOfError = 0.00001f;
        float newVelocity;

        if (Mathf.Abs(inputValue) > inputRestMarginOfError)
        {
            //has input, accelerate in input direction
            newVelocity = currentVelocity + Mathf.Sign(inputValue) * FreeAxisAcceleration * Time.unscaledDeltaTime;
            float limit = Mathf.Abs(inputValue) * speedLimit;
            newVelocity = Mathf.Clamp(newVelocity, -limit, limit);
        }
        else if (Mathf.Abs(currentVelocity) > inputRestMarginOfError)
	    {
	        //not at rest yet, so decelerate
			newVelocity = currentVelocity + -Mathf.Sign(currentVelocity) * FreeAxisDeceleration * Time.unscaledDeltaTime;
	        
	        if (MathUtil.HaveOppositeSigns(newVelocity, currentVelocity))  //if we passed 0 while applying deceleration, set to 0
	        {
	            newVelocity = 0f;
	        }
	    }
	    else  //at rest, ensure velocity is exactly 0
	    {
	        newVelocity = 0f;
	    }

        return newVelocity;
    }

    private bool RotateHorizontal(float deltaAngle)
    {
        return Rotate(currentPivotAngle.x, rotationLimit.x, rotationLimit.y, deltaAngle, Vector3.up);
    }

    private bool RotateVertical(float deltaAngle)
    {
        return Rotate(currentPivotAngle.y, rotationLimit.z, rotationLimit.w, deltaAngle, -pivot.right);
    }

    /// <summary>
    /// Rotates the pivot.
    /// Returns true when the full delta rotation was performed.
    /// </summary>
    private bool Rotate(float currentAngle, float angleLimitMin, float angleLimitMax, float deltaAngle, Vector3 rotationAxis)
    {
        bool performedRotation;

        if (!Mathf.Approximately(deltaAngle, 0f))
        {
            float newAngle = currentAngle + deltaAngle;
			newAngle = MathUtil.ConvertAngleTo180Range(newAngle);

            // If we would go beyond limit, change delta such that we
            //will rotate to the limit
            bool clamp = newAngle < angleLimitMin || newAngle > angleLimitMax;
            if (clamp)
            {
                // We need to find the closer limit by comparing deltas with the current as well as the opposite sign equivalent of the current
                float oppositeSign = MathUtil.ConvertAngleToOppositeSign(currentAngle);
                float closestToMin = MathUtil.LowestAbs(angleLimitMin - currentAngle, angleLimitMin - oppositeSign);
				float closestToMax = MathUtil.LowestAbs(angleLimitMax - currentAngle, angleLimitMax - oppositeSign);
				deltaAngle = MathUtil.LowestAbs(closestToMin, closestToMax);
            }

            RotatePivotQuaternion(rotationAxis, deltaAngle);

            performedRotation = !clamp;  //if we got clamped, do not report that we performed the rotation
        }
        else
        {
            performedRotation = false;
        }

        return performedRotation;
    }

	public void ResetPivotAngle()
	{
		SetPivotQuaternion(initialRotation);
	}

    private void OnDisable()
    {
		// clear input and rotations
        EndRotationInput();
        StopCurrentRotation();
    }

	public bool WithinCameraViewport( Vector3 worldPoint )
	{
		bool withinView;
		Vector3 viewportPoint = frameCamera.WorldToViewportPoint( worldPoint );
		withinView = (viewportPoint.x > 0.0f && viewportPoint.x < 1.0f && viewportPoint.y > 0.0f && viewportPoint.y < 1.0f) && Vector3.Dot( (worldPoint-pivot.position).normalized, pivot.forward ) > 0.0f;
		return withinView;
	}
	
	private bool WithinCameraViewportHorizontal( Vector3 worldPoint, out float side)
	{
		bool withinView;
		Vector3 viewportPoint = frameCamera.WorldToViewportPoint( worldPoint );
		withinView = (viewportPoint.x > 0.0f && viewportPoint.x < 1.0f);
		if ( Vector3.Dot( (worldPoint-pivot.position).normalized, pivot.forward ) < 0.0f )
		{
			viewportPoint.x = viewportPoint.x >= 0.5f ? 0.0f : 1.0f;
			withinView = false;
		}
		side = Mathf.Clamp01(viewportPoint.x);
		return withinView;
	}
	
	private bool WithinCameraViewportVertical( Vector3 worldPoint, out float side)
	{
		bool withinView;
		Vector3 viewportPoint = frameCamera.WorldToViewportPoint( worldPoint );
		withinView = (viewportPoint.y > 0.0f && viewportPoint.y < 1.0f);
		if ( Vector3.Dot( (worldPoint-pivot.position).normalized, pivot.forward ) < 0.0f )
		{
			viewportPoint.y = viewportPoint.y >= 0.5f ? 0.0f : 1.0f;
			withinView = false;
		}
		side = Mathf.Clamp01(viewportPoint.y);
		return withinView;
	}

	public void ApplyRotationToFramePoint(Vector3 worldPoint, float amountCentered)
	{
		Vector3 directionToTarget = (worldPoint-pivot.position).normalized;

		float side;
		WithinCameraViewportHorizontal(worldPoint, out side);
		{
			Vector3 hBorder = frameCamera.ViewportToWorldPoint( new Vector3( side, 0.5f, 1.0f ) );
			Vector3 directionToEdge = (hBorder-pivot.position).normalized;

			Vector3 directionStart = Vector3.Lerp( directionToEdge, pivot.forward, amountCentered );
			
			float horizontalAngle = MathAux.AngleSignedInDegrees(directionStart, directionToTarget, transform.up);
			//yaw
			float xAngle = ConvertAndClampYaw( horizontalAngle );
			RotateHorizontal(xAngle);
		}

		WithinCameraViewportVertical(worldPoint, out side);
		{
			Vector3 vBorder = frameCamera.ViewportToWorldPoint( new Vector3( 0.5f, side, 1.0f ) );
			Vector3 directionToEdge = (vBorder-pivot.position).normalized;

			Vector3 directionStart = Vector3.Lerp( directionToEdge, pivot.forward, amountCentered );
			
			float verticalAngle = MathAux.AngleSignedInDegrees(directionStart, directionToTarget, -pivot.right);
			//pitch
			float yAngle = ConvertAndClampPitch( verticalAngle );
			RotateVertical(yAngle);
		}
	}

	private float ConvertAndClampYaw( float horizontalAngle )
	{
		float xAngle = MathUtil.ConvertAngleTo180Range( horizontalAngle );
		
		//check if this cant pivot 360 degrees
		if ( rotationLimit.x > -180.0f && rotationLimit.y < 180.0f )
		{
			if ( xAngle+currentPivotAngle.x < rotationLimit.x )
			{ //too far left
				xAngle += rotationLimit.x-(xAngle+currentPivotAngle.x);
			}
			else if ( xAngle+currentPivotAngle.x > rotationLimit.y )
			{ //too far right
				xAngle += rotationLimit.y-(xAngle+currentPivotAngle.x);
			}
		}
		return xAngle;
	}
	
	private float ConvertAndClampPitch( float verticalAngle )
	{
		float yAngle = MathUtil.ConvertAngleTo180Range( verticalAngle );
		
		if ( yAngle+currentPivotAngle.y < rotationLimit.z )
		{
			yAngle += rotationLimit.z-(yAngle+currentPivotAngle.y);
		}
		else if ( yAngle+currentPivotAngle.y > rotationLimit.w )
		{
			yAngle += rotationLimit.w-(yAngle+currentPivotAngle.y);
		}
		return yAngle;
	}

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if( pivot != null )
        {
			UpdateReferenceVectors();
            Vector3 upReference = Vector3.Cross(forwardReference, rightReference);

            // Yaw
            Handles.color = new Color( 0.0f, 1.0f, 0.2f, 0.1f );
			if(horizontalRotationLimit.x <= 0.0f && horizontalRotationLimit.y >= 0.0f)
            {
				Handles.DrawSolidArc(pivot.position, upReference, forwardReference, horizontalRotationLimit.x, 0.5f);
				Handles.DrawSolidArc(pivot.position, upReference, forwardReference, horizontalRotationLimit.y, 0.5f);
            }
			else if(horizontalRotationLimit.y > horizontalRotationLimit.x)
            {
				Vector3 startDirection = Quaternion.Euler(0.0f, horizontalRotationLimit.x, 0.0f) * forwardReference;
				Handles.DrawSolidArc(pivot.position, upReference, startDirection, horizontalRotationLimit.y - horizontalRotationLimit.x, 0.5f);
            }
            else
            {
                // Broken angle limits, x needs to be smaller or equal to y
                // Draw in magenta color to show it
                Handles.color = Color.magenta;
				Handles.DrawSolidArc(pivot.position, upReference, forwardReference, horizontalRotationLimit.x, 0.5f);
				Handles.DrawSolidArc(pivot.position, upReference, forwardReference, horizontalRotationLimit.y, 0.5f);
            }

            // Pitch
            UnityEditor.Handles.color = new Color( 1.0f, 0.0f, 0.2f, 0.1f );
			if(verticalRotationLimit.x <= 0.0f && verticalRotationLimit.y >= 0.0f)
            {
				Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.x, 0.5f);
				Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.y, 0.5f);
            }
			else if(verticalRotationLimit.y > verticalRotationLimit.x)
            {
                if (!angleLimitsAroundPivot)
                {
					Vector3 startDirection = Quaternion.Euler(verticalRotationLimit.x, 0.0f, 0.0f) * forwardReference;
					Handles.DrawSolidArc(pivot.position, rightReference, startDirection, verticalRotationLimit.y - verticalRotationLimit.x, 0.5f);
                }
                else
                {
					Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.x, 0.5f);
					Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.y, 0.5f);
                }
            }
            else
            {
                // Broken angle limits, x needs to be smaller or equal to y
                // Draw in magenta color to show it
                Handles.color = Color.magenta;
				Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.x, 0.5f);
				Handles.DrawSolidArc(pivot.position, -rightReference, forwardReference, verticalRotationLimit.y, 0.5f);
            }
        }
    }
#endif
}