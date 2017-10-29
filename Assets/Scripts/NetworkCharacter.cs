﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPos;
	Quaternion realRot;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			
		} else {
			transform.position = Vector3.Lerp (transform.position, realPos, Time.deltaTime * 15f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRot, Time.deltaTime * 30f);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);

		} else {
			realPos = (Vector3)stream.ReceiveNext ();
			realRot = (Quaternion)stream.ReceiveNext ();
		}
	}
}
