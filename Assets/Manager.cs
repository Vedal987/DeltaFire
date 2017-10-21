using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using Photon;
using EZCameraShake;

public class Manager : Photon.MonoBehaviour {

	public string GameVersion;
	public GameObject[] SpawnPoints;
	public GameObject loading;
	public GameObject loadingStatusText;
	private string loadingStat = "LOADING SCENE";

	void Start () 
	{
		Connect ();
	}

	void Update () 
	{
		loadingStatusText.GetComponent<Text>().text = loadingStat;
	}

	void Connect()
	{
		PhotonNetwork.ConnectUsingSettings (GameVersion);
		loadingStat = "CONNECTING TO SERVER";
	}

	void OnJoinedLobby()
	{
		PhotonNetwork.JoinRandomRoom ();
		loadingStat = "TRYING TO JOIN ROOM";
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom (null);
		loadingStat = "NO ROOMS AVAILABLE, CREATING ONE";
	}

	void OnJoinedRoom()
	{
		Debug.Log("JoinedRoom");
		loadingStat = "ROOM JOINED, SPAWNING PLAYER";
		Spawn ();
	}

	void Spawn() {
		Transform point = SpawnPoints [Random.Range (0, SpawnPoints.Length)].transform;
		GameObject player = PhotonNetwork.Instantiate ("Player", point.position, Quaternion.identity, 0) as GameObject;
		player.GetComponent<FirstPersonController> ().enabled = true;
		player.GetComponent<CharacterController> ().enabled = true;
		player.GetComponent<CapsuleCollider> ().enabled = false;
		player.transform.tag = "Player";
		player.transform.GetChild (1).transform.GetChild(0).GetComponent<SkinnedMeshRenderer> ().enabled = false;
		player.transform.GetChild (0).gameObject.GetComponent<CameraShaker> ().enabled = true;
		GameObject cam = player.transform.GetChild (0).GetChild (0).gameObject;
		cam.GetComponent<Camera> ().enabled = true;
		cam.GetComponent<AudioListener> ().enabled = true;
		loading.SetActive (false);
		cam.transform.GetChild (0).gameObject.SetActive (true);
		cam.transform.GetChild (0).GetChild (0).gameObject.GetComponent<WeaponSway> ().enabled = true;
	}
}
