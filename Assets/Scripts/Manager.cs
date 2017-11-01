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

	public string currentGamemode;

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

	public GameObject ContentScroll;
	public GameObject GamePrefab;

	public GameObject currentSettings;

	public string[] possibleHardpointMaps;
	public GameObject HardpointSettings;

	public string[] possibleFreeForAllMaps;
	public GameObject FreeForAllSettings;

	public string[] possibleTeamDeathmatchMaps;
	public GameObject TeamDeathmatchSettings;

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
					PhotonNetwork.Disconnect ();
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
		CancelInvoke ();
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
		InvokeRepeating ("RefreshServerList", 0f, 3f);
	}

	public void RefreshServerList()
	{
		for(int i = 0; i < ContentScroll.transform.GetChildCount(); i++)
		{
			GameObject.Destroy(ContentScroll.transform.GetChild(i).gameObject);
		}
		foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
			GameObject GameGo = Instantiate (GamePrefab, ContentScroll.transform);
			GameGo.transform.GetChild (0).GetComponent<Text> ().text = room.Name;
			GameGo.transform.GetChild (1).GetComponent<Text> ().text = (string)room.CustomProperties ["Gamemode"];
			GameGo.transform.GetChild (2).GetComponent<Text> ().text = (string)room.CustomProperties ["Map"];
			GameGo.transform.GetChild (3).GetComponent<Text> ().text = room.PlayerCount + "/" + room.MaxPlayers;
			GameGo.GetComponent<Button>().onClick.AddListener(delegate {JoinRoom(GameGo.transform.GetChild (0).gameObject);});

		}
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
		CancelInvoke ();
		GameModeChanged (0);
	}

	public void GameModeChanged(int v)
	{
		mapSelector.ClearOptions ();
		if (GameModeSelector.value == 0) {
			mapSelector.AddOptions (new List<string>(possibleFreeForAllMaps));
			FreeForAllSettings.SetActive (true);
			HardpointSettings.SetActive (false);
			TeamDeathmatchSettings.SetActive (false);
			currentSettings = FreeForAllSettings;
		} else if (GameModeSelector.value == 1) {
			mapSelector.AddOptions (new List<string>(possibleHardpointMaps));
			FreeForAllSettings.SetActive (false);
			HardpointSettings.SetActive (true);
			TeamDeathmatchSettings.SetActive (false);
			currentSettings = HardpointSettings;
		} else if (GameModeSelector.value == 2) {
			mapSelector.AddOptions (new List<string>(possibleTeamDeathmatchMaps));
			FreeForAllSettings.SetActive (false);
			HardpointSettings.SetActive (false);
			TeamDeathmatchSettings.SetActive (true);
			currentSettings = TeamDeathmatchSettings;
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
		if (kills >= (int)PhotonNetwork.room.CustomProperties ["KillsToWin"] && currentGamemode == "FreeForAll") {
			Log ("You won");
		}
		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			if (console.activeSelf) {
				if (Player != null) {
					Player.GetComponent<FirstPersonController> ().enabled = true;
					Player.GetComponent<Main> ().canShoot = true;
					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;
				}

				console.SetActive (!console.activeSelf);
			} else {
				
				console.SetActive (!console.activeSelf);
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.Confined;
				if (Player != null) {
					Player.GetComponent<FirstPersonController> ().enabled = false;
					Player.GetComponent<Main> ().canShoot = false;
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
		if (currentSettings.transform.GetChild (0).GetComponent<InputField> ().text != "0" && currentSettings.transform.GetChild (0).GetComponent<InputField> ().text != "" && Convert.ToInt32(currentSettings.transform.GetChild (0).GetComponent<InputField> ().text) <= 10) {
			if (currentSettings.transform.GetChild (1).GetComponent<InputField> ().text != "0" && currentSettings.transform.GetChild (1).GetComponent<InputField> ().text != "" && Convert.ToInt32(currentSettings.transform.GetChild (1).GetComponent<InputField> ().text) <= Convert.ToInt32(currentSettings.transform.GetChild (1).GetComponent<InputField> ().placeholder.GetComponent<Text> ().text)) {

				RoomOptions roomOptions = new RoomOptions ();
				roomOptions.MaxPlayers = Convert.ToByte(Convert.ToInt32 (currentSettings.transform.GetChild (0).GetComponent<InputField> ().text));
				roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable ();

				string mapName = "";
				string gamemode = "";
				if (GameModeSelector.value == 0) {
					mapName = possibleFreeForAllMaps [mapSelector.value];
					gamemode = "FreeForAll";
					roomOptions.CustomRoomProperties.Add ("KillsToWin", Convert.ToInt32 (currentSettings.transform.GetChild (1).GetComponent<InputField> ().text));
				}
				if (GameModeSelector.value == 1) {
					mapName = possibleHardpointMaps [mapSelector.value];
					gamemode = "Hardpoint";
					roomOptions.CustomRoomProperties.Add ("PointsToWin", Convert.ToInt32 (currentSettings.transform.GetChild (1).GetComponent<InputField> ().text));
					roomOptions.CustomRoomProperties.Add ("AlphaPoints", 0);
					roomOptions.CustomRoomProperties.Add ("BetaPoints", 0);
				}
				if (GameModeSelector.value == 2) {
					mapName = possibleTeamDeathmatchMaps [mapSelector.value];
					gamemode = "TeamDeathmatch";
					roomOptions.CustomRoomProperties.Add ("KillsToWin", Convert.ToInt32 (currentSettings.transform.GetChild (1).GetComponent<InputField> ().text));
					roomOptions.CustomRoomProperties.Add ("AlphaKills", 0);
					roomOptions.CustomRoomProperties.Add ("BetaKills", 0);
				}
				string name = gameName.text;
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


				roomOptions.CustomRoomProperties.Add ("Map", mapName);
				roomOptions.CustomRoomProperties.Add ("Gamemode", gamemode);

				roomOptions.CustomRoomPropertiesForLobby = new string[] {"Map", "Gamemode"};
				PhotonNetwork.CreateRoom(name, roomOptions, TypedLobby.Default);
			}
		}
	}
		
	public void JoinRoom(GameObject GameName)
	{
		CancelInvoke ();
		Log ("Joining Room");
		loadingStat = "JOINING ROOM";
		PhotonNetwork.JoinRoom (GameName.GetComponent<Text> ().text);
		loading.SetActive (true);
		isLoading = true;
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
		currentGamemode = (string)PhotonNetwork.room.CustomProperties ["Gamemode"];
		string map = "";
		map = (string)PhotonNetwork.room.CustomProperties ["Map"];
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
		player.transform.GetChild (1).GetChild(0).GetComponent<SkinnedMeshRenderer> ().enabled = false;
		player.transform.GetChild (0).gameObject.SetActive (true);

		if (!isImageEffectsEnabled) {
			player.transform.GetChild (0).GetChild (0).GetComponent<PostProcessingBehaviour> ().enabled = false;
		}

		loading.SetActive (false);
		isPlaying = true;
		Log ("Player Spawned");
		NetworkLog("New Player Joined!");
	}
}
