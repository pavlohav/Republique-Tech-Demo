using UnityEngine;
using System.Collections;

public class rotateAroundXAxis : MonoBehaviour {
	
	public int speed = 70;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.right * Time.deltaTime * speed);
	}
}
