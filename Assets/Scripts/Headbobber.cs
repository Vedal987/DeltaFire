using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbobber : MonoBehaviour {

	public GameObject parent;

	private float timer = 0.0f;
	float bobbingSpeed = 0.04f;
	float bobbingAmount = 0.004f;
	float midpoint = 0.0f;

	void Update () {
		float waveslice = 0.0f;
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		if (parent.GetComponent<Main> ().scoped) {
			bobbingSpeed = 0.01f;
			bobbingAmount = 0.001f;
		} else {
			bobbingSpeed = 0.04f;
			bobbingAmount = 0.004f;
		}

		Vector3 cSharpConversion = transform.localPosition; 

		if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
			timer = 0.0f;
		}
		else {
			waveslice = Mathf.Sin(timer);
			timer = timer + bobbingSpeed;
			if (timer > Mathf.PI * 2) {
				timer = timer - (Mathf.PI * 2);
			}
		}
		if (waveslice != 0) {
			float translateChange = waveslice * bobbingAmount;
			float totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
			totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
			translateChange = totalAxes * translateChange;
			cSharpConversion.y = midpoint + translateChange;
		}
		else {
			cSharpConversion.y = midpoint;
		}

		transform.localPosition = cSharpConversion;
	}
}
