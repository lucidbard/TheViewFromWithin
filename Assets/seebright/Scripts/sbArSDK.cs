// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;

public class sbArSDK
{

	
	/* Metaio Related Variables */
	private static System.Type metSDK;
	private static GameObject metSDKInstance = null;
	private static GameObject metTracker;


	public static string USING_METAIO = "usingMetaio";
	public static string USING_VUFORIA = "usingVuforia";

	string CURSOR_TRACKER_NAME;

	public sbArSDK ()
	{

	}

	public String detectArSDK() {
		if (metSDK == null) {
			metSDK = System.Type.GetType ("metaioSDK");
		}
		if (metSDKInstance == null) {
			metSDKInstance = GameObject.Find ("metaioSDK");
		}
		if (metTracker == null) {
			metTracker = GameObject.Find (CURSOR_TRACKER_NAME);
		}
		return USING_METAIO;
	}
}

