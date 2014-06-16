/**
 * Seebright SDK v0.1.1 - Seebright, Inc
 *
 *  Copyright 2014 Seebright. 
 *  All rights reserved.
 * 
 *  Includes License-Free code:
 *      Gyroscope-controlled camera for iPhone & Android revised 2.26.12
 *      Initially authored by Perry Hoberman <hoberman@bway.net>
 *      Modified by Simon McCorkindale <simon@aroha.mobi>
 *      Authors: 
 *          Matthew McCanty <matt@seebright.com>
 *          John Murray <john@seebright.com>
 * 
 */
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System;

#pragma warning disable 0414 // variable assigned but not used.
[ExecuteInEditMode()]  
public class seebrightSDK : MonoBehaviour
{
	

	public bool debugOutput = false;
	public GameObject designatedCamera;
	//Gyro Variables
	public static bool gyroCameraEnabled;
	private Quaternion rotFix;
	[HideInInspector]
	public bool simulatePose;
	//Seebright Camera Objects and Settings
	[HideInInspector]
	public GameObject	interfaceCameraPrefab;
	[HideInInspector]
	public Color bgColor;
	[Range (0, 80)]
	public static float	IPD = .06f;

	
	Camera designatedInterfaceCamera;
	Camera devCamera;
	[HideInInspector]
	public GameObject  cameraPrefab;
	[Range (-180,180)]
	public float cameraYaw = 0f;
	[Range (-180,180)]
	public float cameraPitch = 0f;
	[Range (-180,180)]
	public float cameraRoll = 0f;
	public bool gyroCamControl = true;
	public bool constrainCameraToPlane;
	public int planeDistanceToCamera;
	[Range (30.0f,100.0f)]
	public float
		mainCamFieldOfView = 32;
	public float nearClipPlane = .3f;
	public float farClipPlane = 1000f;
	public float lerpDistanceThreshold = 5f;


	private Vector3 viewportPoint;

	public static seebrightSDK singleton;

	public bool isThickLens = true;
	public float local3DScale = 1.0f;

	void Start() {
		singleton=this;
		sbRemote.initializeRemote();
	}
	
	void OnApplicationPause(bool pauseStatus) {
		if(pauseStatus)
			sbRemote.StopService();
		else
			sbRemote.StartService();
	}

	Vector3 CameraOrientation;

	/************************************************************************************
     *  Update is called once per frame, handles updating remote controls                |
     ************************************************************************************/
	void Update ()
	{
		#if UNITY_EDITOR
		#endif

		// Emulate the mouse in the editor
		#if UNITY_EDITOR || UNITY_EDITOR_IOS
		//mouseDelta.x = Input.mousePosition.x-mouseLastPosition.x;
		//mouseLastPosition.x = Input.mousePosition.x;
		//mouseDelta.y = Input.mousePosition.y-mouseLastPosition.y;
		//mouseLastPosition.y = Input.mousePosition.y;
		#endif
	}
	
	const string GUI_LABEL = "guiLabel";
	GUIStyle cursorStyle;
	
	void OnGUI ()
	{
		if (debugOutput) {
			if (cursorStyle == null) {
				cursorStyle = new GUIStyle ();
				cursorStyle.fontSize = 20;
				cursorStyle.fontStyle = FontStyle.Bold;
				cursorStyle.normal.textColor = Color.white;
				
			}
			if(sbCursors.seebrightCursor2D !=null)
				writeGUI (GUI_LABEL, -150, -200, 100, 50, "Cursor2D: \nx:" + (int)sbCursors.cursorScreenPosition.x + "\ny:" + (int)sbCursors.cursorScreenPosition.y);
			if(sbCursors.seebrightCursor3D!=null)
				writeGUI (GUI_LABEL, 25, -200, 100, 100, "Cursor3D: \nx:" +
			          sbCursors.seebrightCursor3D.transform.position.x + "\ny:" + 
			          sbCursors.seebrightCursor3D.transform.position.y + "\nz:" + 
			          sbCursors.seebrightCursor3D.transform.position.z);
			
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR 
			writeGUI(GUI_LABEL,Screen.width/4-200, 100, 400, 50, "Remote " + (sbRemote.remoteStatus?"Connected":"Disconnected") + sbRemote.printRemoteControls());
			#elif UNITY_EDITOR
			writeGUI(GUI_LABEL,Screen.width/4-200, 100, 400, 50, "UnityEdit: " + sbRemote.printRemoteControls());
			#endif
			if(Input.gyro.enabled) {
			writeGUI (GUI_LABEL, Screen.width / 4 - 150, -75, 300, 50, 
				          "Phone: yaw:" + Input.gyro.attitude.eulerAngles.y + "  pitch:" + Input.gyro.attitude.eulerAngles.x + "  roll:" + Input.gyro.attitude.eulerAngles.z);
			} else {
			writeGUI (GUI_LABEL, Screen.width / 4 - 150, -75, 300, 50, 
				          "Phone: yaw:" + 0 + "pitch:" + 0 + "roll:" + 0);
			}
				writeGUI (GUI_LABEL, Screen.width / 4 - 150, -50, 300, 50, 
			          "Remote: yaw:" + sbRemote.remoteOrientation.eulerAngles.y + "  pitch:" + sbRemote.remoteOrientation.eulerAngles.x + "  roll:" + sbRemote.remoteOrientation.eulerAngles.z);
		}
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR   
			sbRemote.updateRemoteStatus();
		#endif
	}
	
	public void writeGUI (string type, int x, int y, int width, int height, string stringContent)
	{
		int newLeftX = (x < 0) ? Screen.width / 2 + x : x;
		int newRightX = (x < 0) ? Screen.width + x : Screen.width / 2 + x;
		int newY = (y < 0) ? Screen.height + y : y;
		switch (type) {
		case GUI_LABEL:
			GUI.contentColor = Color.white;
			GUI.color = Color.white;
			GUI.Label (new UnityEngine.Rect (newLeftX, newY, width, height), stringContent, cursorStyle);
			GUI.Label (new UnityEngine.Rect (newRightX, newY, width, height), stringContent, cursorStyle);
			//Rotation not necessary for SeebrightSDK, because right-side-up text on Landscape Left shows up
			//upside-down through the mirrors.
			break;
		default:
			break;
		}
	}

	void LateUpdate() {
		sbRemote.remoteLateUpdate();
	}
 	

}
