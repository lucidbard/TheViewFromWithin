using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	int zoomLevel = 1;
	float denominator = 10;
	public Camera cameraLeft;
	public Camera cameraRight;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Input.GetButton ("Fire2")) {
						Vector3 curr = transform.position;
						Vector3 adder = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
						transform.position = curr + (adder / denominator) * zoomLevel;
				} else {
			Vector3 curr = transform.position;
			// Find all game objects with tag Enemy
			if(cameraLeft.orthographicSize >= 0 ) { 
			cameraLeft.orthographicSize -= Input.GetAxis ("Vertical")/100;
			cameraRight.orthographicSize -= Input.GetAxis ("Vertical")/100;
			} else 
				cameraLeft.orthographicSize = 0;
		}
	}
}
