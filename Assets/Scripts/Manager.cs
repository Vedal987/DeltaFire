using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using Photon;
using EZCameraShake;
using UnityEngine.PostProcessing;

public class Manager : Photon.MonoBehaviour {

	public string GameVersion;
	public GameObject[] SpawnPoints;
	public GameObject loading;
	public GameObject loadingStatusText;
	private string loadingStat = "LOADING SCENE";
	public bool isLoading;
	public GameObject TitleScreen;
	public GameObject Options;
	public GameObject Multiplayer;
	public GameObject CreateRoom;
	public AudioClip UIClick;
	AudioSource ad;

	public GameObject imageEffectsTick;
	public bool isImageEffectsEnabled = true;

	public InputField gameName;
	public Dropdown mapSelector;

	public string[] possibleMaps;
	public string[] mapName;

	public void BackToMenu()
	{
		ad.PlayOneShot (UIClick);
		TitleScreen.SetActive (true);
		if (Options.activeSelf) {
			Options.SetActive (false);
			isImageEffectsEnabled = imageEffectsTick.GetComponent<Toggle> ().isOn;
		}
		Multiplayer.SetActive (false);
		CreateRoom.SetActive (false);
	}
	public void Quit()
	{
		ad.PlayOneShot (UIClick);
		Application.Quit ();
	}
	public void MultiplayerMenu()
	{
		ad.PlayOneShot (UIClick);
		TitleScreen.SetActive (false);
		Multiplayer.SetActive (true);
	}
	public void OptionsMenu()
	{
		ad.PlayOneShot (UIClick);
		TitleScreen.SetActive (false);
		Options.SetActive (true);
	}
	public void CreateRoomMenu()
	{
		ad.PlayOneShot (UIClick);
		Multiplayer.SetActive (false);
		CreateRoom.SetActive (true);
		mapSelector.ClearOptions ();
		mapSelector.AddOptions (new List<string>(possibleMaps));
		mapSelector.RefreshShownValue ();
	}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () 
	{
		ad = this.GetComponent<AudioSource> ();
		Connect ();
	}

	void Update () 
	{
		if (isLoading) {
			loadingStatusText.GetComponent<Text> ().text = loadingStat;
		}
	}

	void Connect()
	{
		PhotonNetwork.ConnectUsingSettings (GameVersion);
		loadingStat = "CONNECTING TO SERVER";
	}

	void OnJoinedLobby()
	{
		//PhotonNetwork.JoinRandomRoom ();
		loadingStat = "TRYING TO JOIN ROOM";
	}

	public void CreatePhotonRoom()
	{
		SceneManager.LoadScene (mapName [mapSelector.value]);
		loading.SetActive (true);
		isLoading = true;
		loadingStat = "CREATING ROOM";
		PhotonNetwork.CreateRoom (gameName.text + possibleMaps [mapSelector.value]);
	}

	void OnPhotonRandomJoinFailed()
	{
		PhotonNetwork.CreateRoom (null);
		loadingStat = "NO ROOMS AVAILABLE, CREATING ONE";
	}

	void OnJoinedRoom()
	{
		loadingStat = "ROOM JOINED, SPAWNING PLAYER";
		Spawn ();
	}

	void Spawn() {
		SpawnPoints = GameObject.FindGameObjectsWithTag ("Spawn");
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
		if (!isImageEffectsEnabled) {
			cam.GetComponent<PostProcessingBehaviour> ().enabled = false;
		}
		loading.SetActive (false);
		cam.transform.GetChild (0).gameObject.SetActive (true);
		cam.transform.GetChild (0).GetChild (0).gameObject.GetComponent<WeaponSway> ().enabled = true;
	}
}
