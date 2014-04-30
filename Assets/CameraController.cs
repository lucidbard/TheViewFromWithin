using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public float zoomLevel = 1;
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
			if(cameraLeft.orthographicSize >= 0 && cameraLeft.orthographicSize<7600) { 
				cameraLeft.orthographicSize -= cameraLeft.orthographicSize*Input.GetAxis ("Vertical")/20;
				cameraRight.orthographicSize -= cameraLeft.orthographicSize*Input.GetAxis ("Vertical")/20;
				zoomLevel=cameraRight.orthographicSize;
			} else {
				if(cameraLeft.orthographicSize==0) {
					cameraLeft.orthographicSize = 1; 
					cameraRight.orthographicSize = 1;
				} else { 
					if(Input.GetAxis ("Vertical")>0) {
						cameraLeft.orthographicSize -= cameraLeft.orthographicSize*Input.GetAxis ("Vertical")/20;
						cameraRight.orthographicSize -= cameraLeft.orthographicSize*Input.GetAxis ("Vertical")/20;
					} else {
					cameraLeft.orthographicSize = 7600;
					cameraRight.orthographicSize = 7600;
					}
				}
			}
		}
	}
}
