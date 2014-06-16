using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {
	public bool debugOutput;
	public float zoomLevel = 1;
	public float cameraDepth = -10;
	public float tourLength=5f;
	private bool motionControl=false;
#if !UNITY_IOS
	float denominator = 10;
#else
	float denominator = 10f;
#endif
	public Camera cameraLeft;
	public Camera cameraRight;
	public Camera cameraNormal;

	public SpriteRenderer startSprite;
	public SpriteRenderer warpSprite;
	public MeshRenderer outerObject;
	private Vector3 startPos = new Vector3(3132.36f,-101.3f,.25f);
	public Transform[] scenes;
	public int currentScene=0;
	private string currentSceneTitle="";
	Vector3 center= Vector3.zero;
	private string currentObjectTree="";
	private string currentCenterTree="";

	void recursiveCenterCalc(Transform trans) {
		foreach (Transform child in trans)
		{
			if(child.gameObject.GetComponent ("renderer")) {
				center += child.gameObject.renderer.bounds.center;
				currentCenterTree+="\nCenter: " + child.gameObject.name + "," + child.gameObject.renderer.bounds.center;
				childCount++;
				if(child.childCount>0)
					recursiveCenterCalc(child.transform);
			}
			else {
				recursiveCenterCalc(child.transform);
			}
		}
	}

	Bounds recursiveBoundsCalc(Transform trans, Bounds bounds){
		currentObjectTree+="\nParent: " + trans.gameObject.name;
		foreach (Transform child in trans)
		{
			if(child.gameObject.GetComponent("Renderer")) { 
				currentObjectTree+="\nRenderer: " + child.gameObject.name + child.gameObject.renderer.bounds;
				bounds.Encapsulate(child.gameObject.renderer.bounds);
			} else
			   bounds = recursiveBoundsCalc(child.gameObject.transform,bounds);
		}
		return bounds;
	}

	int childCount=0;
	Bounds getBounds(Transform trans) {
		currentObjectTree="";
		currentCenterTree="";
		// First find a center for your bounds.
		Bounds bounds;
		center = Vector3.zero;
		childCount = 0;
		if(trans.childCount==1 && trans.GetChild(0).GetComponent("Renderer")) {
			center = trans.GetChild(0).renderer.bounds.center;
			bounds = trans.GetChild(0).renderer.bounds;
		} else {
			recursiveCenterCalc(trans);
			Debug.Log ("Calculating " + childCount + " children");
			center /= childCount; //center is average center of children
			//Now you have a center, calculate the bounds by creating a zero sized 'Bounds', 
			 bounds = new Bounds(center,Vector3.zero); 
			bounds=recursiveBoundsCalc(trans,bounds);
		}
		return bounds;
	}

	// Use this for initialization
	void Start () {
		Debug.Log("Sprite Bounds:" + warpSprite.sprite.bounds);
		Debug.Log("Sprite Renderer Bounds: " + warpSprite.bounds);
		Debug.Log("Outer Object Bounds: " + outerObject.bounds);
		Vector3 newPosition = new Vector3(startSprite.transform.position.x,startSprite.transform.position.y,-10);
		cameraLeft.orthographicSize = cameraRight.orthographicSize = cameraNormal.orthographicSize = .25f;
		transform.position=newPosition;
	}
	
	Vector3 mousePos;
	Vector3 dragOrigin;
	// Update is called once per frame
	void Update () {
		Vector3 curr = transform.position;
		Vector3 adder;
		float posX=0f;
		float posY=0f;
#if !UNITY_IOS

		adder = new Vector3 (Input.GetAxis ("Horizontal")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f), 
		                             Input.GetAxis ("Vertical")*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:.3f));
		posX = (curr + (adder / denominator) * zoomLevel).x;
		posY = (curr + (adder / denominator) * zoomLevel).y;
#else
		if(sbRemote.GetButtonDown (sbRemote.BUTTON_BACK)) {
			motionControl=!motionControl;
		}
		if(!motionControl) {
			adder = new Vector3 (sbRemote.GetAxis(sbRemote.JOY_HORIZONTAL), 
			                     sbRemote.GetAxis (sbRemote.JOY_VERTICAL));
		} else {
			// Use the remote tilt and roll to handle movement.
			float pitch=0f;
			if(sbRemote.remoteOrientation.eulerAngles.x>180){
				pitch=(360f-sbRemote.remoteOrientation.eulerAngles.x);
			} else {
				pitch=-(sbRemote.remoteOrientation.eulerAngles.x);
			}
			adder = new Vector3 ((180f-(Mathf.Clamp(sbRemote.remoteOrientation.eulerAngles.z,90f,270f)))/180f,
			                     pitch/180f);
		}
		
		posX = (curr + (adder / denominator) * zoomLevel).x;
		posY = (curr + (adder / denominator) * zoomLevel).y;
		#endif
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
		//ScrollWheel to zoom in and out.
		mousePos = cameraNormal.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraNormal.transform.position.z));
		dragOrigin = cameraNormal.ScreenToWorldPoint(new Vector3 (Screen.width / 2, Screen.height / 2, cameraNormal.transform.position.z));
		if((cameraLeft.orthographicSize >= .20f && cameraLeft.orthographicSize<=8500)&&
		   (cameraNormal.orthographicSize >= .20f && cameraNormal.orthographicSize<=8500)) { 
			if(cameraLeft.orthographicSize > .2f) {
				if( Input.GetAxis("Mouse ScrollWheel") > 0 )
				{
					
					cameraLeft.orthographicSize -= cameraLeft.orthographicSize/4f;
					cameraRight.orthographicSize -= cameraRight.orthographicSize/4f;
					cameraNormal.orthographicSize -= cameraNormal.orthographicSize/4f;
					newPosition.y = transform.position.y + ((mousePos.y - dragOrigin.y) / (cameraNormal.orthographicSize * 2));
                	newPosition.x = transform.position.x + ((mousePos.x - dragOrigin.x) / (cameraNormal.orthographicSize * 2));
					newPosition.z = -10;
				} else {
					cameraLeft.orthographicSize -= cameraLeft.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_SELECT)|Input.GetKey (KeyCode.PageDown)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
					cameraRight.orthographicSize -= cameraRight.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_SELECT)|Input.GetKey(KeyCode.PageDown)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
					cameraNormal.orthographicSize -= cameraNormal.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_SELECT)|Input.GetKey(KeyCode.PageDown)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				}
			}
			if(cameraLeft.orthographicSize < 8500) {
				if(Input.GetAxis("Mouse ScrollWheel") < 0)
				{
					cameraLeft.orthographicSize += cameraLeft.orthographicSize/4f;
					cameraRight.orthographicSize += cameraRight.orthographicSize/4f;
					cameraNormal.orthographicSize += cameraNormal.orthographicSize/4f;
					newPosition.y = transform.position.y - ((mousePos.y - dragOrigin.y) / (cameraNormal.orthographicSize * 2));
					newPosition.x = transform.position.x - ((mousePos.x - dragOrigin.x) / (cameraNormal.orthographicSize * 2));
					mousePos = cameraNormal.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0 - cameraNormal.transform.position.z));
					dragOrigin = cameraNormal.transform.position;
				} else {
					cameraLeft.orthographicSize += cameraLeft.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_OPTION)|Input.GetKey (KeyCode.PageUp)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
					cameraRight.orthographicSize += cameraRight.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_OPTION)|Input.GetKey (KeyCode.PageUp)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
					cameraNormal.orthographicSize += cameraNormal.orthographicSize*
						(sbRemote.GetButton(sbRemote.BUTTON_OPTION)|Input.GetKey(KeyCode.PageUp)?1:0)/
							(20.0f*((Input.GetKey(KeyCode.LeftShift)|Input.GetKey (KeyCode.RightShift))?1:2));
				}

			}
			transform.position = newPosition;
			zoomLevel=cameraRight.orthographicSize;
		} else if(cameraLeft.orthographicSize<.2f) {
				cameraLeft.orthographicSize = .2f; 
				cameraRight.orthographicSize = .2f;
				cameraNormal.orthographicSize = .2f;
		} else if (cameraLeft.orthographicSize>=8500||Input.GetKey (KeyCode.PageUp) || Input.GetAxis("Mouse ScrollWheel") < 0){ 
				warpToCorrespondingObject(outerObject,warpSprite);
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
		if(Input.GetKeyDown (KeyCode.Home)) {
			animating=false;
			goToPosition(startPos);
		} else if (Input.GetKeyDown (KeyCode.Space)||sbRemote.GetButton(sbRemote.BUTTON_TRIGGER)) {
			animating=false;
			nextScene();
		}else if (Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown(KeyCode.Z)) {
			animateToPosition(warpSprite.bounds);
			animating=false;
		} else if (Input.GetKeyDown (KeyCode.V)) {
			animating=false;
			goToPosition(outerObject);
		} else if (Input.GetKeyDown (KeyCode.Backspace)) {
			animating=false;
			Debug.Log (printPosition());
		}
		if(animating)
		{
			elapsed+=Time.deltaTime;
//			lastTime=Time.time;
//			float fracJourney = distCovered / journeyLength;
			
			curPosition.x = Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(start_x,dist_x,elapsed,tourLength);
			curPosition.y = Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(start_y,dist_y,elapsed,tourLength);
			transform.position = curPosition;
			if(elapsed<=7.5f) {
				zoomLevel=Interpolate.Ease(Interpolate.EaseType.EaseInOutQuad)(startZoom,endZoom-startZoom,elapsed,tourLength+1f);
			} else {
				animating=false;
				curPosition.x=start_x+dist_x;
				curPosition.y=start_y+dist_y;
				transform.position = destination;
			}
			cameraLeft.orthographicSize = cameraRight.orthographicSize = cameraNormal.orthographicSize = zoomLevel;
		}
		
	}
	private bool animating=false;
	private float journeyLength;
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

	
	void nextScene() {
		currentObjectTree="";
		if(currentScene < scenes.Length-1)
		{
			++currentScene;
		} else
			currentScene = 0;
		Debug.Log ("Going to " + scenes[currentScene].gameObject.name);
		currentSceneTitle = scenes[currentScene].gameObject.name;
		Bounds objectBounds = getBounds(scenes[currentScene]);
		animateToPosition(objectBounds);
		Debug.Log (scenes[currentScene].transform.position + " bounds: " + objectBounds);

	}

	public void animateToPosition(Bounds bounds) {
		destination = bounds.center;
//		origin = transform.position;
		startZoom = zoomLevel;
		float destZoom = bounds.extents.y;
		if(destZoom==0) {
			destZoom=endZoom=startZoom;
		} else {
			endZoom=destZoom;
		}
		elapsed=0;
		animating=true;
		start_x=transform.position.x;
		start_y=transform.position.y;
		dist_x=bounds.center.x-start_x;
		dist_y=bounds.center.y-start_y;
	}


	// Warps the camera to a corresponding location on an equally sized larger object (used for recursive camera movements).
	public void warpToCorrespondingObject(Renderer smallObject, Renderer largeObject) {
		Vector3 newPosition = new Vector3(0,0,-10);
		float uniform_x = transform.position.x-smallObject.transform.position.x;
		float uniform_y = transform.position.y-smallObject.transform.position.y;
		double scale_x = largeObject.bounds.extents.x/smallObject.bounds.extents.x;
		double scale_y = largeObject.bounds.extents.y/smallObject.bounds.extents.y;
		Debug.Log ("Warping: " + uniform_x + ", " + uniform_y + " at " + (zoomLevel/smallObject.bounds.extents.y) + " scale x: " + scale_x + "scale_y: " + scale_y );
		zoomLevel = (zoomLevel/smallObject.bounds.extents.y)*largeObject.bounds.extents.y;
		newPosition.x = (float)(uniform_x*scale_x);
		newPosition.y = (float)(uniform_y*scale_y);
		newPosition = newPosition+largeObject.transform.position;
		newPosition.z = -10;
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
		if(debugOutput) {
			GUI.color = Color.black;
			GUI.Label(new UnityEngine.Rect(10, 10, 150, 100), "Mouse Pos: " + mousePos + ", dragOrigin: " + dragOrigin);
//			GUI.Label(new UnityEngine.Rect(10, 10, 150, 100), printPosition());
			GUI.Label(new UnityEngine.Rect(10, 40, 150, 100), currentSceneTitle);
			GUI.Label(new UnityEngine.Rect(10, 70, 200, 1000), currentObjectTree);
			GUI.Label(new UnityEngine.Rect(420, 70, 200, 1000), currentCenterTree);
		}
	}
}
