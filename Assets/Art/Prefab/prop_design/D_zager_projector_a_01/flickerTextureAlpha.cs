using UnityEngine;
using System.Collections;

public class flickerTextureAlpha : MonoBehaviour {
	
	public Renderer rend;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float rand = Random.Range(0.1f, 0.9f);
		rend.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, rand));
	}
}
