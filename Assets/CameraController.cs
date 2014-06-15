using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {
	public float zoomLevel = 1;
	public float cameraDepth = -10;
	float denominator = 10;
	public Camera cameraLeft;
	public Camera cameraRight;
	public Camera cameraNormal;

	public SpriteRenderer startSprite;
	public SpriteRenderer warpSprite;
	public MeshRenderer outerObject;
	private Vector3 startPos = new Vector3(3132.36f,-101.3f,.25f);

	// Use this for initialization
	void Start () {
		Debug.Log("Sprite Bounds:" + warpSprite.sprite.bounds);
		Debug.Log("Sprite Renderer Bounds: " + warpSprite.bounds);
		Debug.Log("Outer Object Bounds: " + outerObject.bounds);
		Vector3 newPosition = new Vector3(startSprite.transform.position.x,startSprite.transform.position.y,-10);
		cameraLeft.orthographicSize = cameraRight.orthographicSize = cameraNormal.orthographicSize = .25f;
		transform.position=newPosition;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 curr = transform.position;
		Vector3 adder = new Vector3 (Input.GetAxis ("Horizontal")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f), 
		                             Input.GetAxis ("Vertical")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f));
		float posX = (curr + (adder / denominator) * zoomLevel).x;
		float posY = (curr + (adder / denominator) * zoomLevel).y;
		Vector3 newPosition = new Vector3();
		newPosition.x=curr.x;
		newPosition.y=curr.y;
		newPosition.z=curr.z;
		if(posX < 16039.46f && posX > -17743.39f)
			newPosition.x = posX;
		else if (posX > 16039.46)
			newPosition.x = 16039.46f;
		else if (posX < -17743.39f)
			newPosition.x = -17743.39f;

		if(posY < 8800 && posY > -8154.197)
			newPosition.y = posY;
		else if (posY < -8154.197)
			newPosition.y = -8154;
		else if (posY > 8800.0)
			newPosition.y = 8800.0f;
//					newPiority = true

		transform.position = newPosition;


		if((cameraLeft.orthographicSize >= .20f && cameraLeft.orthographicSize<=8500)&&
		   (cameraNormal.orthographicSize >= .20f && cameraNormal.orthographicSize<=8500)) { 
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
			if(cameraLeft.orthographicSize < 8400) {
				cameraLeft.orthographicSize += cameraLeft.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey (KeyCode.PageUp)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraRight.orthographicSize += cameraRight.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey (KeyCode.PageUp)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				cameraNormal.orthographicSize += cameraNormal.orthographicSize*
					(Input.GetButton ("Fire2")|Input.GetKey(KeyCode.PageUp)?1:0)/
						(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
			}
				zoomLevel=cameraRight.orthographicSize;


		} else {
			if(cameraLeft.orthographicSize<.2f) {
				cameraLeft.orthographicSize = .2f; 
				cameraRight.orthographicSize = .2f;
				cameraNormal.orthographicSize = .2f;
			} else if (cameraLeft.orthographicSize>=8500){ 
				cameraLeft.orthographicSize = 8500;
				cameraRight.orthographicSize = 8500;
				cameraNormal.orthographicSize = 8500;
			}
		}
		// Detect if we are within a warp zone
		if(zoomLevel<warpSprite.bounds.extents.y && 
		   transform.position.x > warpSprite.transform.position.x-warpSprite.bounds.extents.x-.5 &&
		   transform.position.x < warpSprite.transform.position.x+warpSprite.bounds.extents.x+.5 && 
		   transform.position.y > warpSprite.transform.position.y-warpSprite.bounds.extents.y-.5 &&
		   transform.position.y < warpSprite.transform.position.y+warpSprite.bounds.extents.y+.5)
		   {
			warpToCorrespondingObject(warpSprite,outerObject);
		}
		if(Input.GetKeyDown (KeyCode.Home)||Input.GetKeyDown (KeyCode.Space)) {
			goToPosition(startPos);
		} else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown(KeyCode.Z)) {
			animateToPosition(warpSprite.transform.position,warpSprite.bounds.extents.y+1f);
		} else if (Input.GetKeyDown (KeyCode.V)) {
			goToPosition(outerObject);
		} else if (Input.GetKeyDown (KeyCode.Backspace)) {
			Debug.Log (printPosition());
		}
		if(animating)
		{
			elapsed+=Time.deltaTime;
//			lastTime=Time.time;
//			float fracJourney = distCovered / journeyLength;
			
			curPosition.x = Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(start_x,dist_x,elapsed,10);
			curPosition.y = Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(start_y,dist_y,elapsed,10);
			transform.position = curPosition;
			if(elapsed<=5.0f) {
				zoomLevel=Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(startZoom,midZoom-startZoom,elapsed,5.0f);
			} else if (elapsed >5.0f && elapsed < 10.0f) {
				zoomLevel=Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(midZoom,-(midZoom-endZoom),elapsed-5.0f,5.0f);
			} else {
				animating=false;
				curPosition.x=start_x+dist_x;
				curPosition.y=start_y+dist_y;
				transform.position = curPosition;
			}

			cameraLeft.orthographicSize = cameraRight.orthographicSize = cameraNormal.orthographicSize = zoomLevel;
		}
		
	}
	float distCovered = 0f;
	private float lastTime;
	private bool animating=false;
	private float speed = 2.0F;
	private float journeyLength;
	private Vector3 origin;
	private Vector3 destination;
	private Vector3 curPosition=new Vector3(0,0,-10);
	private float startZoom;
	private float midZoom;
	private float endZoom;
	private float elapsed=0f;
	private float start_x;
	private float start_y;
	private float dist_x;
	private float dist_y;

	public void animateToPosition(Vector3 dest, float destZoom=0) {
		destination = dest;
		distCovered = 0f;
		origin = transform.position;
		startZoom = zoomLevel;

		if(destZoom==0) {
			destZoom=endZoom=startZoom;
		} else {
			if(Mathf.Abs(destZoom-startZoom)>100)
				midZoom = Mathf.Abs((destZoom-startZoom)/2);
			else
				midZoom = Vector3.Distance(origin,destination)/6f;
			endZoom=destZoom;
		}

		animating=true;
		lastTime=Time.time;
		start_x=transform.position.x;
		start_y=transform.position.y;
		dist_x=dest.x-start_x;
		dist_y=dest.y-start_y;
	}


	// Warps the camera to a corresponding location on an equally sized larger object (used for recursive camera movements).
	public void warpToCorrespondingObject(Renderer smallObject, Renderer largeObject) {
		Vector3 newPosition = new Vector3(0,0,-10);
		float uniform_x = transform.position.x-smallObject.transform.position.x;
		float uniform_y = transform.position.y-smallObject.transform.position.y;
		double scale_x = largeObject.bounds.extents.x/smallObject.bounds.extents.x;
		double scale_y = largeObject.bounds.extents.y/smallObject.bounds.extents.y;
//		Debug.Log ("Warping: " + uniform_x + ", " + uniform_y + " at " + (zoomLevel/smallObject.bounds.extents.y) + " scale x: " + scale_x + "scale_y: " + scale_y );
		zoomLevel = (zoomLevel/smallObject.bounds.extents.y)*largeObject.bounds.extents.y;
		newPosition.x = (float)(uniform_x*scale_x);
		newPosition.y = (float)(uniform_y*scale_y);
		transform.position = newPosition;
		cameraLeft.orthographicSize = cameraRight.orthographicSize = cameraNormal.orthographicSize = zoomLevel;
	}
	
	public void goToPosition(SpriteRenderer position) {
		cameraLeft.orthographicSize=position.bounds.extents.y;
		cameraRight.orthographicSize=position.bounds.extents.y;
		cameraNormal.orthographicSize=position.bounds.extents.y;
		transform.position = new Vector3(position.gameObject.transform.position.x,position.gameObject.transform.position.y,-10.0f);
	}

	public void goToPosition(MeshRenderer position) {
		cameraLeft.orthographicSize=position.bounds.extents.y;
		cameraRight.orthographicSize=position.bounds.extents.y;
		cameraNormal.orthographicSize=position.bounds.extents.y;
		transform.position = new Vector3(position.gameObject.transform.position.x,position.gameObject.transform.position.y,-10.0f);
	}

	public void goToPosition(Vector3 position) {
		cameraLeft.orthographicSize=position.z;
		cameraRight.orthographicSize=position.z;
		cameraNormal.orthographicSize=position.z;
		transform.position = new Vector3(position.x,position.y,-10.0f);
	}

	public string printPosition() {
		string curPos = "Current position: (" + 
			transform.position.x.ToString() + 
			"," + transform.position.y.ToString() + 
			"," + cameraNormal.orthographicSize.ToString() +
			")";
		return curPos;
	}
	
	void OnGUI() {
		GUI.color = Color.black;
		GUI.Label(new UnityEngine.Rect(10, 10, 150, 100), printPosition());
	}
}
