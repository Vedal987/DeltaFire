using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ShakeCamera : MonoBehaviour {

	public void GunShake()
	{
		CameraShaker.Instance.ShakeOnce (1f, 1f, 0.2f, 0.3f);
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
