using UnityEngine;
using System.Collections;

public class FakeWind : MonoBehaviour 
{
	private Vector3 _unityVector = new Vector3(1, 1, 1);
	private Vector3[] _vertices = null;
	private Vector3[] _normals = null;

	//@TODO: KEVBOT! Clean up unused paramaters maybe?
	public Vector3 _animParams = new Vector3();
	public Vector3 _wind = new Vector3();
	public float _windW = 0;
	public float _windEdgeFlutterFreqScale = 0;

	// Use this for initialization
	void Start () 
	{
		// Save the vertices
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			Mesh mesh = meshFilter.mesh;
			if (mesh != null)
			{
				_vertices = mesh.vertices;
				_normals = mesh.normals;
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if (_vertices != null)
		{
			Vector3[] newVertices = new Vector3[_vertices.Length];

			// _Time.y = Time * 2
			float windTime = Time.time * _windEdgeFlutterFreqScale;

			for (int vertIndex = 0; vertIndex < _vertices.Length; ++vertIndex)
			{
				// Animate the vertices
				newVertices[vertIndex] = AnimateVertex(_vertices[vertIndex], _normals[vertIndex], _animParams, windTime);
			}

			// Reassign the vertices to the mesh
			GetComponent<MeshFilter>().mesh.vertices = newVertices;

			// If the mesh needs collision, the bounding volume should be recalculated using mesh.RecalculateBounds();
		}
	}

	//http://game.ceeger.com/forum/read.php?tid=10510
	private Vector3 AnimateVertex(Vector3 pos, Vector3 normal, Vector4 animParams, float windTime)
	{
		// animParams stored in color
		// animParams.x = branch phase
		// animParams.y = edge flutter factor
		// animParams.z = primary factor
		// animParams.w = secondary factor
		
		float fDetailAmp = 0.1f;
		float fBranchAmp = 0.3f;

		// Phases (object, vertex, branch)
		float fObjPhase = Vector3.Dot(transform.position , _unityVector);
		float fBranchPhase = fObjPhase + animParams.x;

		float fVtxPhase = Vector3.Dot(pos, new Vector3(animParams.y + fBranchPhase, animParams.y + fBranchPhase, animParams.y + fBranchPhase));

		// x is used for edges; y is used for branches
		Vector2 vWavesIn = new Vector2(fVtxPhase + windTime, fBranchPhase + windTime);
	
		// 1.975, 0.793, 0.375, 0.193 are good frequencies
		Vector4 vWaves = new Vector4(Frac(1.975f * vWavesIn.x), Frac(0.793f * vWavesIn.x), Frac(0.375f * vWavesIn.y), Frac(0.193f * vWavesIn.y));

		vWaves = vWaves * 2.0f;
		vWaves.x -= 1.0f;
		vWaves.y -= 1.0f;
		vWaves.z -= 1.0f;
		vWaves.w -= 1.0f;

		vWaves = SmoothTriangleWave( vWaves );

		Vector2 vWavesSum = new Vector2(vWaves.x + vWaves.y, vWaves.z + vWaves.w);

		// Edge (xz) and branch bending (y)
		Vector3 bend = animParams.y * fDetailAmp * normal;
		bend.y = animParams.w * fBranchAmp;
		pos += ((new Vector3(vWavesSum.x * bend.x, vWavesSum.y * bend.y, vWavesSum.x * bend.y)) + (_wind * vWavesSum.y * animParams.w)) * _windW; 

		// Primary bending
		// Displace position
		pos += animParams.z * _wind;
		
		return pos;
	}

	//@TODO: Change this.. at some point... convert from shader magic to C#
	private float Frac(float value)
	{
		return value - (float)System.Math.Floor(value);
	}

	private Vector4 SmoothCurve(Vector4 x)
	{
		//return x * x * ( 3.0f - 2.0f * x );
		return new Vector4(x.x * x.x * (3.0f - 2.0f * x.x), 
		                   x.y * x.y * (3.0f - 2.0f * x.y), 
		                   x.z * x.z * (3.0f - 2.0f * x.z), 
		                   x.w * x.w * (3.0f - 2.0f * x.w));
	}
	
	private Vector4 TriangleWave(Vector4 x)
	{
		//return Mathf.Abs( Frac( x + 0.5f ) * 2.0f - 1.0f );
		return new Vector4(Mathf.Abs( Frac( x.x + 0.5f ) * 2.0f - 1.0f ),
		                   Mathf.Abs( Frac( x.y + 0.5f ) * 2.0f - 1.0f ),
		                   Mathf.Abs( Frac( x.z + 0.5f ) * 2.0f - 1.0f ),
		                   Mathf.Abs( Frac( x.w + 0.5f ) * 2.0f - 1.0f ));
	}
	
	private Vector4 SmoothTriangleWave(Vector4 x)
	{
		return SmoothCurve(TriangleWave(x));
	}
	
}
