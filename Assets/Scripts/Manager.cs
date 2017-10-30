using System.Collections;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using Photon;
using EZCameraShake;
using UnityEngine.PostProcessing;

public class Manager : Photon.MonoBehaviour {

	public string currentGameMode;
	public int kills;
	public int deaths;
	public int team;

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
	public Dropdown GameModeSelector;

	public GameObject RegisterUI;
	public InputField RegisterUser;
	public InputField RegisterPass;

	public GameObject LogInUI;
	public InputField LogInUser;
	public InputField LogInPass;

	public string[] possibleHardpointMaps;
	public string[] HardpointMapName;

	public string[] possibleFreeForAllMaps;
	public string[] FreeForAllMapName;

	public string[] possibleTeamDeathmatchMaps;
	public string[] TeamDeathmatchMapName;

	public Text LogText;
	public InputField Command;
	public GameObject console;

	public bool isPlaying;

	public GameObject Player;
	public GameObject connectingUI;

	public GameObject namePopUp;
	public InputField name;

	AsyncOperation asyncLoadLevel;

	public void Log(string m)
	{
		LogText.text += "\n" + m;
	}

	public void NetworkLog(string m)
	{
		if (Player != null) {
			Player.GetPhotonView ().RPC ("ExecuteCommand", PhotonTargets.All, m, Player.GetComponent<PhotonView> ().viewID);
		} else {
			Log ("Network Log Failed. Could not find player.");
		}
	}

	public void EnterName()
	{
		if (name.text != "") {
			PhotonNetwork.player.NickName = name.text;
			namePopUp.SetActive (false);
			TitleScreen.SetActive (true);
		}
	}


	public void LogIn()
	{
		string username = LogInUser.text;
		string password = LogInPass.text;
		if (username != "" && password != "") {

		} else {
			//boi
		}
	}

	public void Register()
	{
		string username = RegisterUser.text;
		string password = RegisterPass.text;
		if (username != "" && password != "") {
			
		} else {
			//boi
		}
	}

	public void HasAccount()
	{
		LogInUI.SetActive (true);
		RegisterUI.SetActive (false);
	}
	public void NoAccount()
	{
		LogInUI.SetActive (false);
		RegisterUI.SetActive (true);
	}
		
	public void RequestCommand(string com)
	{
		Command.Select ();
		if (com != "") {
			Command.text = "";
			if (com.StartsWith ("m ")) {
				if (Player != null) {
					
					if (com.StartsWith ("m SetHealth ")) {
						Player.GetPhotonView().RPC ("ExecuteCommand", PhotonTargets.AllBuffered, com, Player.GetComponent<PhotonView>().viewID);
						return;
					}
					Log ("Invalid Command");
				} else {
					Log ("Failed to execute command. There is no player.");
				}
					
			} else {

				if (com.StartsWith ("Join")) {
					if (com.StartsWith ("Join Random")) {
						Log ("Joining Random Room");
						loadingStat = "JOINING RANDOM ROOM";
						PhotonNetwork.JoinRandomRoom ();
						loading.SetActive (true);
						isLoading = true;
						return;
					}

				}
				if (com.StartsWith ("Leave")) {
					PhotonNetwork.LeaveRoom ();
					loading.SetActive (false);
					isLoading = false;
					SceneManager.LoadScene ("Menu");
					GameObject.Destroy (GameObject.Find ("_Manager"));
				}
				if (com.StartsWith ("Quit")) {
					Log ("Quitting Game...");
					Application.Quit ();
					return;
				}

				Log ("Invalid Command");
			}
		}
	}



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
		Log ("Quitting Game...");
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
		mapSelector.AddOptions (new List<string>(possibleFreeForAllMaps));
	}

	public void GameModeChanged(int v)
	{
		mapSelector.ClearOptions ();
		if (GameModeSelector.value == 0) {
			mapSelector.AddOptions (new List<string>(possibleFreeForAllMaps));
		} else if (GameModeSelector.value == 1) {
			mapSelector.AddOptions (new List<string>(possibleHardpointMaps));
		} else if (GameModeSelector.value == 2) {
			mapSelector.AddOptions (new List<string>(possibleTeamDeathmatchMaps));
		}
		mapSelector.RefreshShownValue ();
	}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	void Start () 
	{
		ad = this.GetComponent<AudioSource> ();
		connectingUI.SetActive (true);
		Connect ();

	}

	void Update () 
	{
		if (isLoading) {
			loadingStatusText.GetComponent<Text> ().text = loadingStat;
		}
		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			if (console.activeSelf) {
				if (Player != null) {
					Player.GetComponent<FirstPersonController> ().enabled = true;
					Player.GetComponent<Main> ().canShoot = true;
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
					Player.transform.GetChild (0).GetChild (0).GetChild (0).GetChild (0).gameObject.GetComponent<WeaponSway> ().enabled = true;
				}

				console.SetActive (!console.activeSelf);
			} else {
				
				console.SetActive (!console.activeSelf);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.Confined;
				if (Player != null) {
					Player.GetComponent<FirstPersonController> ().enabled = false;
					Player.GetComponent<Main> ().canShoot = false;
					Player.transform.GetChild (0).GetChild (0).GetChild (0).GetChild (0).gameObject.GetComponent<WeaponSway> ().enabled = false;
				}
			}
		}
	}

	void Connect()
	{
		PhotonNetwork.ConnectUsingSettings (GameVersion);
		loadingStat = "CONNECTING TO SERVER";
		Log ("Connecting To Server");
	}

	void OnJoinedLobby()
	{
		//PhotonNetwork.JoinRandomRoom ();
		//loadingStat = "TRYING TO JOIN ROOM";
		//Log ("Trying To Join Room");
		Log ("Connected To Server");
		connectingUI.SetActive (false);
	}

	public void CreatePhotonRoom()
	{
		string roomDetails = "";
		if (GameModeSelector.value == 0) {
			roomDetails = FreeForAllMapName [mapSelector.value] + " FreeForAll ";
		}
		if (GameModeSelector.value == 1) {
			roomDetails = HardpointMapName [mapSelector.value] + " Hardpoint ";
		}
		if (GameModeSelector.value == 2) {
			roomDetails = TeamDeathmatchMapName [mapSelector.value] + " TeamDeathmatch ";
		}
		string name = roomDetails + " " + gameName.text;
		foreach (RoomInfo info in PhotonNetwork.GetRoomList()) {
			if (name == info.Name) {
				Log ("A room with that name already exists. Please try again.");
				return;
			}
		}
		loading.SetActive (true);
		isLoading = true;
		loadingStat = "CREATING ROOM";
		Log ("Creating Room");
		PhotonNetwork.CreateRoom (name) ;

	}

	void OnPhotonRandomJoinFailed()
	{
		Log ("No Rooms Available");
		loading.SetActive (false);
		isLoading = false;
	}

	void OnJoinedRoom()
	{
		loadingStat = "ROOM JOINED, LOADING SCENE";
		char[] space = " ".ToCharArray();
		string[] MapName = PhotonNetwork.room.Name.Split(space);
		string map = MapName [0].TrimEnd (space [0]);
		currentGameMode = MapName [1].TrimEnd (space [0]);
		StartCoroutine (LoadScene (map));
		PhotonNetwork.isMessageQueueRunning = false;
	}

	IEnumerator LoadScene (string scene){
		asyncLoadLevel = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
		Log ("Loading Scene");
		while (!asyncLoadLevel.isDone){
			yield return null;
		}
		PhotonNetwork.isMessageQueueRunning = true;
		yield return new WaitForSeconds (1f);
		Spawn ();
	}

	public void Spawn() {
		Log ("Spawning Player");
		loadingStat = "SPAWNING PLAYER";
		SpawnPoints = GameObject.FindGameObjectsWithTag ("Spawn");

		Transform point = SpawnPoints [UnityEngine.Random.Range (0, SpawnPoints.Length)].transform;
		GameObject player = PhotonNetwork.Instantiate ("Player", point.position, Quaternion.identity, 0) as GameObject;
		Player = player;
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
		isPlaying = true;
		Log ("Player Spawned");
		NetworkLog("New Player Joined!");
	}
}
