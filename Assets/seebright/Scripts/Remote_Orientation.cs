using UnityEngine;
using System.Collections;
//using metaio;

public class Remote_Orientation : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		transform.rotation = sbRemote.remoteOrientation;
		transform.Rotate (0, 0, 180);

	}
}
