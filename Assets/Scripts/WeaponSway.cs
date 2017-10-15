using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour {

	public float amount = 0.02f;
	public float maxAmount = 0.03f;
	float smooth = 3;
	private Vector3 def;

	void  Start (){
		def = transform.localPosition;
	}

	void  Update (){

		float factorX = -Input.GetAxis("Mouse X") * amount;
		float factorY = -Input.GetAxis("Mouse Y") * amount;

		factorX = Mathf.Clamp(factorX, -maxAmount, maxAmount);
		factorY = Mathf.Clamp(factorY, -maxAmount, maxAmount);

		Vector3 Final = new Vector3(def.x+factorX, def.y+factorY, def.z);
		transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * smooth);       
	}
}
