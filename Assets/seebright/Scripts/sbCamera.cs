using UnityEngine;
using System.Collections;

public class sbCamera {

	public void initializeMainCamera() {
		// Modify for Nexus 5 FOV
	#if UNITY_ANDROID && !UNITY_EDITOR

	#elif UNITY_IOS && !UNITY_EDITOR

	#endif
	}

	public static Camera designatedCamera;

	
	private static GameObject camGrandparent;
	private static GameObject camParent;

	public GameObject leftCamera {
		get; set;
	}
	public GameObject rightCamera {
		get; set;
	}
	sbCursors cursors;
	private int _IPD=45;
	private float _fieldOfView=42.3f;
	private bool _orthographic;
	public Camera originalCamera;
	private bool _enabled;

	public bool enabled {
		set {
			_enabled=value; 
			leftCamera.camera.enabled=value;
			rightCamera.camera.enabled=value;
		}
		get {return _enabled;}
	}
	public float fieldOfView {
		
		set {
			if(leftCamera!=null)
				_fieldOfView=leftCamera.camera.fieldOfView=value;
			if(rightCamera!=null)
				rightCamera.camera.fieldOfView=_fieldOfView=value;
		}
		get {return _fieldOfView;}
	}
	
	public int IPD {
		set {leftCamera.transform.localPosition.Set(-_IPD/2,0,0);rightCamera.transform.localPosition.Set(_IPD/2,0,0);}
		get {return _IPD;}
	}
	
	public void cloneCameras(Camera inputCamera) {

		_fieldOfView=inputCamera.fieldOfView;
		originalCamera=inputCamera;
			// Verify there isn't any current children:
		if (GameObject.Find (inputCamera.name + "-SB-Left")) {
			leftCamera = GameObject.Find (inputCamera.name + "-SB-Left");
		} else { 
			leftCamera = initializeCamera ("-SB-Left", inputCamera);
			CopyComponent (inputCamera, leftCamera);
			leftCamera.camera.rect = new Rect (0f, 0f, .5f, 1f);
		}
		if (GameObject.Find (inputCamera.name + "-SB-Right")) {
			rightCamera = GameObject.Find (inputCamera.name + "-SB-Right");
		} else {
			rightCamera = initializeCamera ("-SB-Right", inputCamera);
			CopyComponent (inputCamera, rightCamera);
			rightCamera.camera.rect = new Rect (.5f, 0f, .5f, 1f);
		}
		
		// Hide the original image of the camera
		inputCamera.rect = new Rect (0f, 0f, 0f, 0f);


		if (!seebrightSDK.singleton.gyroCamControl) {
			//Toggle HMD Tyoe for Camera Offset
			if (seebrightSDK.singleton.isThickLens) {
				leftCamera.transform.localPosition = new Vector3 ((-seebrightSDK.IPD / 2 + -54.565f), (0f + -195.325f), (0f + -18.936f));
				rightCamera.transform.localPosition = new Vector3 ((seebrightSDK.IPD / 2 + -54.565f), (0f + -195.325f), (0f + -18.936f));
			} else {
					leftCamera.transform.localPosition = new Vector3 ((-seebrightSDK.IPD / 2 + -43.415f), (0f + -142.2f), (0f + -3.759f));
					rightCamera.transform.localPosition = new Vector3 ((seebrightSDK.IPD / 2 + -43.415f), (0f + -142.2f), (0f + -3.759f));
			}
		}
		else
		{
			leftCamera.transform.localPosition = new Vector3 (-seebrightSDK.IPD / 2, 0f, 0f);
			rightCamera.transform.localPosition = new Vector3 (seebrightSDK.IPD / 2, 0f, 0f);
		}
	}
	
	public GameObject initializeCamera(string suffix, Camera input) {
		GameObject newCam;
		foreach ( Transform child in input.gameObject.transform){
			if (child.name == input.name+suffix){
				newCam = child.gameObject;
				newCam.transform.localPosition = Vector3.zero;
				newCam.transform.localRotation = Quaternion.identity;
				newCam.transform.RotateAround (newCam.transform.position, newCam.transform.up, 180);
				newCam.transform.RotateAround (newCam.transform.position, newCam.transform.forward, 180);
				newCam.transform.RotateAround (newCam.transform.position, newCam.transform.right, 330);
				return newCam;
			}
		}
		newCam = new GameObject();
		newCam.transform.parent = input.gameObject.transform;
		newCam.name = input.name+suffix;
		newCam.transform.localPosition = Vector3.zero;
		newCam.transform.localRotation = Quaternion.identity;
		if(seebrightSDK.singleton.designatedCamera != null && input == seebrightSDK.singleton.designatedCamera.camera) {
			newCam.transform.RotateAround (newCam.transform.position, newCam.transform.up, 180);
			newCam.transform.RotateAround (newCam.transform.position, newCam.transform.forward, 180);
			newCam.transform.RotateAround (newCam.transform.position, newCam.transform.right, 330);
		}
		newCam.transform.localScale = new Vector3(1,1,1);
		//newCam.camera.backgroundColor = Color.black;
		//newCam.camera.depth = 10000000000f;
		
		
		//#if UNITY_IPHONE && !UNITY_EDITOR
		//#endif
		return newCam;
	}
	
	Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}

	private static bool gyroBool;
	static Transform oldRoot;

	static Quaternion rotFix;

	// Construct the necessary grandparent and parent for Gyrocam capabilities
	public static void setupGyroCam () {
		gyroBool = SystemInfo.supportsGyroscope;
		if (seebrightSDK.gyroCameraEnabled && gyroBool) {
			if (GameObject.Find ("GyroCamParent") == null) {
				oldRoot = designatedCamera.transform.parent;
				camParent = new GameObject ("GyroCamParent");
			} else {
				camParent = GameObject.Find ("GyroCamParent");
				oldRoot = GameObject.Find ("GyroCamGrandParent").transform.parent;
			}
			if(oldRoot!=null)
				camParent.transform.position = oldRoot.position;
			if (GameObject.Find ("GyroCamGrandParent") == null) {
				camGrandparent = new GameObject ("GyroCamGrandParent");
			} else {
				camGrandparent = GameObject.Find ("GyroCamGrandParent");
			}
			if(oldRoot!=null)
				camGrandparent.transform.position = designatedCamera.transform.position;
			//camGrandparent is gyroscopically stable and can be moved in other script references.
			camParent.transform.parent = camGrandparent.transform;
			camGrandparent.transform.parent = oldRoot;
			designatedCamera.gameObject.transform.parent = camParent.transform;
		} else {
			if (camGrandparent != null) {
				if (oldRoot != null) {
					designatedCamera.gameObject.transform.parent = oldRoot;
				}
				GameObject.DestroyImmediate (camGrandparent);
				camGrandparent = null;
			}
		}
		
		//Gyroscope Rotation Fixes for LandscapeLeft
		if (gyroBool && camParent != null) {
			Input.gyro.enabled = true;
			camParent.transform.eulerAngles = new Vector3 (90, 270, 0);
		}
		
		rotFix = new Quaternion (0, 0, 1, 0);
	}

	static float cameraPitch;
	static float cameraYaw;
	static float cameraRoll;

	static Vector3 CameraOrientation;

	public static Quaternion myRot = new Quaternion();
	
	public static void updateCamera() {
		//iPhone and Android Quaternion Adjustments for Gyro Data
		if (gyroBool) {
			Quaternion quatMap = Quaternion.identity;
			#if UNITY_IPHONE || UNITY_ANDROID
			quatMap = Input.gyro.attitude;
			#endif
			designatedCamera.transform.localRotation = quatMap * rotFix;
			
			// Allow manual manipulation of camera position in editor.
			#if UNITY_EDITOR
			cameraPitch = designatedCamera.transform.localRotation.eulerAngles.x;
			cameraYaw = designatedCamera.transform.localRotation.eulerAngles.y;
			cameraRoll = designatedCamera.transform.localRotation.eulerAngles.z;
			#endif
		} else {
			CameraOrientation.x = cameraPitch;
			CameraOrientation.y = cameraYaw;
			CameraOrientation.z = cameraRoll;
			// use this predefines pose
			myRot.eulerAngles = CameraOrientation;
			if(designatedCamera!=null)
				designatedCamera.transform.localRotation = myRot;
			// return;
		}
		
	}


}
