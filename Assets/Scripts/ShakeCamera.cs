using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ShakeCamera : MonoBehaviour {

	public bool running;

	CameraShakeInstance c;

	public void GunShake(float a, float b, float c, float d)
	{
		CameraShaker.Instance.ShakeOnce (a, b, c, d);
	}


	// Use this for initialization
	void Start () {
		c = CameraShaker.Instance.ShakeOnce (0f, 0f, 0f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (running) {
			if (c.CurrentState == CameraShakeState.Inactive) {
				c = CameraShaker.Instance.StartShake (2f, 0.8f, 0.3f);
			}
		} else {
			c.StartFadeOut (0.2f);
			c.DeleteOnInactive = true;
		}
	}
}
