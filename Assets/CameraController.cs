using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public float zoomLevel = 1;
	float denominator = 10;
	public Camera cameraLeft;
	public Camera cameraRight;
	public Camera cameraNormal;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curr = transform.position;
		Vector3 adder = new Vector3 (Input.GetAxis ("Horizontal")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f), 
		                             Input.GetAxis ("Vertical")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f));

		transform.position = curr + (adder / denominator) * zoomLevel;
		if((cameraLeft.orthographicSize >= .20f && cameraLeft.orthographicSize<=7600)&&
		   (cameraNormal.orthographicSize >= .20f && cameraNormal.orthographicSize<=7600)) { 
			if(cameraLeft.orthographicSize > .2f) {
				cameraLeft.orthographicSize -= cameraLeft.orthographicSize*
					(Input.GetButton ("Fire1")|Input.GetKey (KeyCode.PageDown)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraRight.orthographicSize -= cameraRight.orthographicSize*
					(Input.GetButton ("Fire1")|Input.GetKey(KeyCode.PageDown)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraNormal.orthographicSize -= cameraNormal.orthographicSize*
					(Input.GetButton ("Fire1")|Input.GetKey(KeyCode.PageDown)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
			}
			if(cameraLeft.orthographicSize < 7600) {
				cameraLeft.orthographicSize += cameraLeft.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey (KeyCode.PageUp)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraRight.orthographicSize += cameraRight.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey (KeyCode.PageUp)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraNormal.orthographicSize += cameraNormal.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey(KeyCode.PageDown)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
			}
			zoomLevel=cameraRight.orthographicSize;
		} else {
			if(cameraLeft.orthographicSize<.2f) {
				cameraLeft.orthographicSize = .2f; 
				cameraRight.orthographicSize = .2f;
				cameraNormal.orthographicSize = .2f;
			} else if (cameraLeft.orthographicSize>=7600){ 
				cameraLeft.orthographicSize = 7600;
				cameraRight.orthographicSize = 7600;
				cameraNormal.orthographicSize = 7600;
			}
		}
		if(Input.GetKeyDown (KeyCode.Home)||Input.GetKeyDown (KeyCode.Space)) {
			cameraLeft.orthographicSize=.25f;
			cameraRight.orthographicSize=.25f;
			cameraNormal.orthographicSize=.25f;
			transform.position = new Vector3(3121.223f,-104.2486f);
		}
	}
}
