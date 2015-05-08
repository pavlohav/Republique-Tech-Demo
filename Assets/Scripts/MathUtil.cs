using UnityEngine;

static public class MathUtil
{
	/// <summary>
	/// Converts a degree angle into the range of [-180, 180].
	/// </summary>
	public static float ConvertAngleTo180Range(float angle)
	{
		// Infinity cannot be logically converted (and would cause our method of calulation to blow the stack!)
		// indicate this by returning NaN
		if (float.IsInfinity(angle))
		{
			angle = float.NaN;
		}
		
		// Keep adding a full circle towards zero until we are within 180 degrees
		// The angle may swap signs. This is intended.
		while (Mathf.Abs(angle) > 180f)
		{
			angle -= Mathf.Sign(angle) * 360f;
		}
		
		return angle;
	}

	/// <summary>
	/// Converts a degree angle into the closest equivalent angle that has the opposite sign.
	/// </summary>
	public static float ConvertAngleToOppositeSign(float angle)
	{
		if (!float.IsInfinity(angle))
		{
			// Move the angle by 360 degree increments until it flips signs
			float intendedSign = -Mathf.Sign(angle);
			float increment = intendedSign * 360f;
			do
			{
				angle += increment;
			}
			while ( Mathf.Sign(angle) != intendedSign );
		}
		else
		{
			// Infinity would blow our stack if we did not special case it.
			// Just flip infinity.
			angle = -angle;
		}
		
		return angle;
	}

	/// <summary>
	/// Calculates the signed angle between the two vectors. Vectors must be noralized beforehand.
	/// </summary>
	public static float AngleSignedInDegrees(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2( Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	/// <summary>
	/// Returns whether or not the two numbers have opposite signs.
	/// </summary>
	public static bool HaveOppositeSigns(float a, float b)
	{
		return (a * b) < 0.0f;
	}

	/// <summary>
	/// Return whichever number has a lower absolute value.
	/// </summary>
	public static float LowestAbs(float a, float b)
	{
		return Mathf.Abs(a) < Mathf.Abs(b)? a : b;
	}
}
