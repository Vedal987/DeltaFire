using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hitDirection : MonoBehaviour {
	public Transform target;
	public Image direction;
	void Update()
	{
		Vector3 relativePos = target.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos);
		print(rotation.eulerAngles) ;

		direction.rectTransform.localRotation = Quaternion.Euler(0,0,-rotation.eulerAngles.y);
	}
}
