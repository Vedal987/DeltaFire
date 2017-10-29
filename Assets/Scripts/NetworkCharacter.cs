using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkCharacter : Photon.MonoBehaviour {

	Vector3 realPos;
	Quaternion realRot;
	float camXRot;
	Transform cam;
	Animator model;

	// Use this for initialization
	void Start () {
		cam = this.transform.GetChild (0).GetChild (0);
		model = this.transform.GetChild (1).GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			
		} else {
			transform.position = Vector3.Lerp (transform.position, realPos, Time.deltaTime * 15f);
			transform.rotation = Quaternion.Lerp (transform.rotation, realRot, Time.deltaTime * 30f);
		}
	}

	void FixedUpdate()
	{
		if (!photonView.isMine) {
			if (camXRot >= 0 && camXRot <= 90) {
				model.SetLayerWeight (3, Mathf.Lerp(model.GetLayerWeight(3), camXRot / 90, Time.deltaTime * 15f));
				model.SetLayerWeight (2, 0);
			} else if (camXRot >= 270 && camXRot <= 360){
				model.SetLayerWeight (2, Mathf.Lerp(model.GetLayerWeight(2), 1 - ((camXRot - 270) / 90), Time.deltaTime * 15f));
				model.SetLayerWeight (3, 0);
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			stream.SendNext (cam.rotation.eulerAngles.x);
		} else {
			realPos = (Vector3)stream.ReceiveNext ();
			realRot = (Quaternion)stream.ReceiveNext ();
			camXRot = (float)stream.ReceiveNext ();
		}
	}
}
