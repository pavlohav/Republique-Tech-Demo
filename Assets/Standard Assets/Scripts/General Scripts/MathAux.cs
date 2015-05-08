using UnityEngine;
using System.Collections;
using System;

public class MathAux
{
    public static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }

    public static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

    public static float Coserp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
    }

    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float SmoothStep (float x, float min, float max)
    {
        x = Mathf.Clamp (x, min, max);
        float v1 = (x-min)/(max-min);
        float v2 = (x-min)/(max-min);
        return -2*v1 * v1 *v1 + 3*v2 * v2;
    }

    public static float Lerp(float start, float end, float value)
    {
        return ((1.0f - value) * start) + (value * end);
    }

    public static float StickTo(float value, float stickTo, float within)
    {
        if ( Approx(value, stickTo, within) )
        {
            value = stickTo;
        }
        return value;
    }

    public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 lineDirection = Vector3.Normalize(lineEnd-lineStart);
        float closestPoint = Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
        return lineStart+(closestPoint*lineDirection);
    }

    public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        Vector3 fullDirection = lineEnd-lineStart;
        Vector3 lineDirection = Vector3.Normalize(fullDirection);
        float closestPoint = Vector3.Dot((point-lineStart),lineDirection)/Vector3.Dot(lineDirection,lineDirection);
        return lineStart+(Mathf.Clamp(closestPoint,0.0f,Vector3.Magnitude(fullDirection))*lineDirection);
    }

    public static float Bounce(float x)
    {
        return Mathf.Abs(Mathf.Sin(6.28f*(x+1f)*(x+1f)) * (1f-x));
    }

    // test for value that is near specified float (due to floating point inprecision)
    // all thanks to Opless for this!
    public static bool Approx(float val, float about, float range)
    {
        return ( ( Mathf.Abs(val - about) < range) );
    }

    // test if a Vector3 is close to another Vector3 (due to floating point inprecision)
    // compares the square of the distance to the square of the range as this
    // avoids calculating a square root which is much slower than squaring the range
    public static bool Approx(Vector3 val, Vector3 about, float range)
    {
        return ( (val - about).sqrMagnitude < range*range);
    }

    public static bool Approx(Vector2 val, Vector2 about, float range)
    {
        return ((val - about).sqrMagnitude < range * range);
    }

    // Test if a Vector3 is close to another Vector3 (due to floating point inprecision)
    // Returns false if close to equal, true if different
    public static bool Compare(Vector3 lhs, Vector3 rhs)
    {
        const float comparisonRangeSqr = 0.000001f;
        return ((lhs - rhs).sqrMagnitude > comparisonRangeSqr);
    }

   /*
     * CLerp - Circular Lerp - is like lerp but handles the wraparound from 0 to 360.
     * This is useful when interpolating eulerAngles and the object
     * crosses the 0/360 boundary.  The standard Lerp function causes the object
     * to rotate in the wrong direction and looks stupid. Clerp fixes that.
     */
    public static float Clerp(float start , float end, float value){
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min)/2.0f);//half the distance between min and max
        float retval = 0.0f;
        float diff = 0.0f;

        if((end - start) < -half)
		{
            diff = ((max - start)+end)*value;
			retval =  start+diff;
        }
        else if((end - start) > half)
		{
            diff = -((max - end)+start)*value;
            retval =  start+diff;
        }
        else retval =  start+(end-start)*value;

        // Debug.Log("Start: "  + start + "   End: " + end + "  Value: " + value + "  Half: " + half + "  Diff: " + diff + "  Retval: " + retval);
        return retval;
    }

    /// <summary>
    /// Returns the number that has the lowest absolute value. Note that this returns the original value and not the abs.
    /// </summary>
    public static float LowestAbs(float a, float b)
    {
        return Mathf.Abs(a) < Mathf.Abs(b)? a : b;
    }

    /// <summary>
    /// Returns the number that has the highest absolute value. Note that this returns the original value and not the abs.
    /// </summary>
    public static float HighestAbs(float a, float b)
    {
        return Mathf.Abs(a) > Mathf.Abs(b)? a : b;
    }

	/// <summary>
	/// Determine the signed angle between two vectors, with normal 'n' as the rotation axis.
    /// Vectors must be normalized beforehand.
	/// </summary>
	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
	    return Mathf.Atan2(
	        Vector3.Dot(n, Vector3.Cross(v1, v2)),
	        Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
	}

	/// <summary>
	/// returns -1 or 1 if object is to the right or left of a forward vector
	/// </summary>
	/// <returns>
	/// The dir.
	/// </returns>
	/// <param name='fwd'>
	/// Fwd.
	/// </param>
	/// <param name='targetDir'>
	/// Target dir.
	/// </param>
	/// <param name='up'>
	/// Up.
	/// </param>
	public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		if (dir > 0.0f)
		{
			return 1.0f;
		}
		else if (dir < 0.0f)
		{
			return -1.0f;
		}
		else
		{
			return 0.0f;
		}
	}

	/// <summary>
	/// Snaps Vector3 to supplied grid size
	/// </summary>
	/// <returns>
	/// Snapped Vector3
	/// </returns>
	/// <param name='v'>
	/// Vector3 to Snap
	/// </param>
	/// <param name='snap'>
	/// Snap amount
	/// </param>
	public static Vector3 SnapVector3( Vector3 v, float snap )
	{
		float xx,yy,zz;

		xx = Mathf.CeilToInt(v.x/snap)+snap;
		yy = Mathf.CeilToInt(v.y/snap)+snap;
		zz = Mathf.CeilToInt(v.z/snap)+snap;

		return new Vector3( xx*snap-snap/2, yy*snap-snap/2, zz*snap-snap/2 );
	}

    /// <summary>
    /// Calculates the signed angle between the two vectors. Vectors must be noralized beforehand.
    /// </summary>
    public static float AngleSignedInDegrees(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2( Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

	public static Color MaskToColor( int mask )
	{
		return new Color32( (byte)(( mask & 0xff000000 ) >> 24 ), (byte)(( mask & 0xff0000 ) >> 16 ), (byte)(( mask & 0xff00 ) >> 8 ),(byte)(( mask & 0xff ))  );
	}

	public static int ColorToMask( Color color )
	{
		return (byte)(color.r*255.0f)<<24 | (byte)(color.g*255.0f)<<16 | (byte)(color.b*255.0f)<<8 | (byte)(color.a*255.0f);
	}

    /// <summary>
    /// Returns true if the arguments have opposite signs, and false if they do not.
    /// </summary>
    /// <param name="num1"></param>
    /// <param name="num2"></param>
    /// <returns></returns>
    public static bool HaveOppositeSigns(float num1, float num2)
    {
        return (num1 * num2) < 0.0f;
    }

    public static float Square(float number)
    {
        return (number * number);
    }

    public static int Mod(int number, int modulus)
    {
        modulus = Mathf.Abs(modulus);
        int result = number % modulus;
        return (result < 0 ? result + modulus : result);
    }
	
	public static int GreatestCommonDivisor(int a, int b)
	{
		return (b == 0)? a : GreatestCommonDivisor(b, a % b);
	}
}

