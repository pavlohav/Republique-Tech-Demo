using UnityEngine;
using System.Collections;

public class VertexDisruption : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    Shader.SetGlobalVector("_DisruptionPos", new Vector4(transform.position.x,transform.position.y,transform.position.z,1.0f) );
	}
}
