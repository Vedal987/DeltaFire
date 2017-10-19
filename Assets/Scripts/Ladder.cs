using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Ladder : MonoBehaviour {

	public GameObject onLadder;

	public bool isLadder = false;

	public float power;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isLadder = true) {
			if (onLadder != null) {
				onLadder.GetComponent<Rigidbody> ().useGravity = false;
				//onLadder.GetComponent<Rigidbody> ().isKinematic = false;
				//onLadder.GetComponent<FirstPersonController> ().enabled = false;
				onLadder.GetComponent<FirstPersonController> ().m_GravityMultiplier = 0.3f;
				if (Input.GetKey(KeyCode.W)) {
					onLadder.transform.position += Vector3.up / power;
					//onLadder.GetComponent<Rigidbody>().AddForce(Vector3.up * power);
				}
			}
		} else {
			onLadder.GetComponent<Rigidbody> ().useGravity = true;
			//onLadder.GetComponent<Rigidbody> ().isKinematic = false;
			//onLadder.GetComponent<FirstPersonController> ().enabled = true;
			onLadder.GetComponent<FirstPersonController> ().m_GravityMultiplier = 1f;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Player") {
			onLadder = col.gameObject;
			isLadder = true;
			onLadder.gameObject.SendMessage("OnLadder");
		}
	}
	void OnTriggerExit(Collider col)
	{
		if (col.gameObject.tag == "Player") {
			onLadder.gameObject.SendMessage("OffLadder");
			onLadder.GetComponent<Rigidbody> ().useGravity = true;
			onLadder.GetComponent<FirstPersonController> ().m_GravityMultiplier = 1f;
			onLadder = null;
			isLadder = false;
		}
	}
}
