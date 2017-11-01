using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using Photon;

public class Main : Photon.MonoBehaviour {
	bool remScoped;

	public GameObject manager;
	public FirstPersonController controller;

	public int health;
	public int maxHealth;


	AudioSource ad;
	public AudioSource ad2;
	public AudioSource hb;

	public AudioClip magIn;
	public AudioClip magOut;
	public AudioClip magBolt;

	public float shootTimer;
	public bool reloading;

	public GameObject currentGun;
	public int gunIndex;
	public GameObject[] guns;
	public GameObject pivot;
	public GameObject cam;
	public Animator modelAnim;
	public GameObject ragdoll;
	public GameObject gunDrop;
	public GameObject deathCam;

	public GameObject muzzleLight;
	public GameObject muzzleFlash;
	public GameObject modelMuzzleLight;
	public GameObject modelMuzzleFlash;

	public GameObject TracerRound;
	public GameObject bulletHole;

	public bool scoped;
	public bool running;
	public bool canShoot = true;

	float bulletSpreadX;
	float bulletSpreadY;
	Vector3 spread;
	int timesPenetrated;
	public float distanceFired;

	bool isAnimatingRun;
	bool isAnimatingWalk;
	bool isStrafing;
	bool isRight;
	bool isLeft;
	bool wantsToShoot;
	bool isBackwards;
	bool regen;

	float respawnTimer;

	// Use this for initialization
	void Start () {
		ad = currentGun.GetComponent<AudioSource> ();
		if (photonView.isMine) {
			manager = GameObject.FindGameObjectWithTag ("Manager");
		}
	}

	IEnumerator SwitchGuns(int index)
	{
		//currentGun.GetComponent<Animator> ().SetTrigger ("Hide");
		canShoot = false;
		currentGun.SetActive (false);
		currentGun = guns[index];
		ad = currentGun.GetComponent<AudioSource> ();
		gunIndex = index;
		currentGun.SetActive (true);
		yield return new WaitForSeconds (0.4f);
		canShoot = true;
	}

	[PunRPC]
	public void Animate(string type)
	{
		if (photonView.isMine) {
		} else {
			if (type == "startRun") {
				modelAnim.SetBool ("isRunning", true);
			}
			if (type == "stopRun") {
				modelAnim.SetBool ("isRunning", false);
			}
			if (type == "startWalk") {
				modelAnim.SetBool ("isWalking", true);
			}
			if (type == "stopWalk") {
				modelAnim.SetBool ("isWalking", false);
			}
			//if (type == "startScope") {
			//	modelAnim.SetBool ("isAiming", true);
			//}
			//if (type == "stopScope") {
			//	modelAnim.SetBool ("isAiming", false);
			//}
			if (type == "startBack") {
				modelAnim.SetBool ("isBackwards", true);
			}
			if (type == "stopBack") {
				modelAnim.SetBool ("isBackwards", false);
			}
			if (type == "startFire") {
				modelAnim.SetBool ("isFiring", true);
				StartCoroutine (ModelMuzzle ());
			}
			if (type == "stopFire") {
				modelAnim.SetBool ("isFiring", false);
			}
			if (type == "startLeft") {
				modelAnim.SetBool ("Left", true);
				if (isAnimatingRun) {
					modelAnim.SetFloat ("StrafeSpeed", 2f);
				}
			}
			if (type == "stopLeft") {
				modelAnim.SetBool ("Left", false);
				modelAnim.SetFloat ("StrafeSpeed", 1f);
			}
			if (type == "startRight") {
				modelAnim.SetBool ("Right", true);
				if (isAnimatingRun) {
					modelAnim.SetFloat ("StrafeSpeed", 2f);
				}
			}
			if (type == "stopRight") {
				modelAnim.SetBool ("Right", false);
				modelAnim.SetFloat ("StrafeSpeed", 1f);
			}
			if (type == "jump") {
				modelAnim.SetTrigger ("Jump");
				isStrafing = false;
			}
			if (type == "startReload") {
				modelAnim.SetBool ("Reloading", true);
			}
			if (type == "stopReload") {
				modelAnim.SetBool ("Reloading", false);
			}
		}

	}

	IEnumerator ModelMuzzle()
	{
		ad2.PlayOneShot (currentGun.GetComponent<GunProperties>().shoot);
		yield return new WaitForSeconds (0.04f);
		modelMuzzleLight.SetActive (true);
		modelMuzzleFlash.SetActive (true);
		Instantiate (TracerRound, modelMuzzleLight.transform.position + spread, modelMuzzleLight.transform.localToWorldMatrix.rotation);
		yield return new WaitForSeconds (0.11f);
		modelMuzzleLight.SetActive (false);
		yield return new WaitForSeconds (0.04f);
		modelMuzzleFlash.SetActive (false);
	}

	void CheckAnimation()
	{
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		bool movingFor;
		bool movingBack;
		bool strafingLeft;
		bool strafingRight;

		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			isAnimatingRun = true;
			photonView.RPC ("Animate", PhotonTargets.Others, "startRun");
		}
		if (Input.GetKeyUp (KeyCode.LeftShift)) {
			isAnimatingRun = false;
			photonView.RPC ("Animate", PhotonTargets.Others, "stopRun");
		}

		if (vertical > 0) {
			movingFor = true;
			movingBack = false;
			if (!isAnimatingWalk) {
				photonView.RPC ("Animate", PhotonTargets.Others, "startWalk");
				isAnimatingWalk = true;
			}
			if (isBackwards) {
				photonView.RPC ("Animate", PhotonTargets.Others, "stopBack");
				isBackwards = false;
			}
		}
		if (vertical < 0) {
			movingFor = true;
			movingBack = false;
			if (!isAnimatingWalk) {
				photonView.RPC ("Animate", PhotonTargets.Others, "startWalk");
				isAnimatingWalk = true;
			}
			if (!isBackwards) {
				photonView.RPC ("Animate", PhotonTargets.Others, "startBack");
				isBackwards = true;
			}
		}
		if (vertical == 0) {
			movingFor = false;
			movingBack = false;
			if (isAnimatingWalk) {
				photonView.RPC ("Animate", PhotonTargets.Others, "stopWalk");
				isAnimatingWalk = false;
			}
			if (isBackwards) {
				photonView.RPC ("Animate", PhotonTargets.Others, "stopBack");
				isBackwards = false;
			}
		}

		if (horizontal > 0) {
			strafingRight = true;
			strafingLeft = false;
			if (isLeft) {
				photonView.RPC ("Animate", PhotonTargets.Others, "stopLeft");
				isLeft = false;
			}
			if(!isRight)
			{
				photonView.RPC ("Animate", PhotonTargets.Others, "startRight");
				isRight = true;
			}
		}
		if (horizontal < 0) {
			strafingLeft = true;
			strafingRight = false;
			if (!isLeft) {
				photonView.RPC ("Animate", PhotonTargets.Others, "startLeft");
				isLeft = true;
			}
			if(isRight)
			{
				photonView.RPC ("Animate", PhotonTargets.Others, "stopRight");
				isRight = false;
			}
		}
		if (horizontal == 0) {
			strafingLeft = false;
			strafingRight = false;
			if (isLeft) {
				photonView.RPC ("Animate", PhotonTargets.Others, "stopLeft");
				isLeft = false;
			}
			if(isRight)
			{
				photonView.RPC ("Animate", PhotonTargets.Others, "stopRight");
				isRight = false;
			}
		}

		if (Input.GetKeyDown (KeyCode.Space) && this.GetComponent<CharacterController> ().isGrounded) {
			photonView.RPC ("Animate", PhotonTargets.Others, "jump");
		}
			
	}
		
	[PunRPC]
	public void ExecuteCommand(string com, int caller)
	{

		if (com.StartsWith ("m SetHealth ")) {
			GameObject callerGO = PhotonView.Find (caller).gameObject;
			int mod;
			com = com.Replace("m SetHealth ", "");
			if (int.TryParse (com, out mod)) {
				callerGO.GetComponent<Main> ().health = mod;
				manager.GetComponent<Manager>().Log ("Health of player " + caller + " set to " + com);
			} else {
				manager.GetComponent<Manager>().Log ("Failed to execute command. Parameter must be an integer.");
			}
		}

	}

	[PunRPC]
	public void ExecuteNetworkLog(string m)
	{
		manager.GetComponent<Manager> ().Log (m);
	}

	public void InitHealthRegen()
	{
		photonView.RPC ("HealthRegen", PhotonTargets.AllBuffered, null);
	}

	[PunRPC]
	void HealthRegen()
	{
		health += 10;
		if (health > 100) {
			health = 100;
		}
		if (health == 100) {
			if (photonView.isMine) {
				CancelInvoke ("InitHealthRegen");
				regen = false;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {

			if (health < 60) {
				hb.volume = 1f - (health / 40f);
				hb.pitch = 1f + health / 120f;
			} else {
				hb.volume = 0f;
				hb.pitch = 1f;
			}


			if (Input.GetKey (KeyCode.Mouse0)) {
				wantsToShoot = true;
			} else {
				wantsToShoot = false;
			}
			if (Input.GetAxis ("Mouse ScrollWheel") > 0f && canShoot && !reloading) {
				if (gunIndex + 1 > guns.Length - 1) {
					StartCoroutine(SwitchGuns (0));
				} else {
					StartCoroutine(SwitchGuns (gunIndex + 1));
				}
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < 0f && canShoot && !reloading) {
				if (gunIndex - 1 < 0) {
					StartCoroutine(SwitchGuns (guns.Length - 1));
				} else {
					StartCoroutine(SwitchGuns (gunIndex - 1));
				}
			}
			if (isStrafing || isAnimatingWalk) {
				if (!Input.GetKey (KeyCode.LeftShift)) {
					pivot.GetComponent<Crosshair> ().doSpread = true;
				} else {
					pivot.GetComponent<Crosshair> ().doSpread = false;
				}
			} else {
				pivot.GetComponent<Crosshair> ().doSpread = false;
			}
			if (respawnTimer > 0) {
				respawnTimer -= Time.deltaTime;
				if (respawnTimer <= 0) {
					GameObject.Find ("_Manager").GetComponent<Manager> ().Spawn ();
					PhotonNetwork.Destroy (this.gameObject);
				}
			}
			CheckAnimation ();
			if (health > 0) {
				if (scoped) {
					pivot.GetComponent<Crosshair> ().showCrosshair = false;
					Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 40f, 0.09f);
				} else {
					pivot.GetComponent<Crosshair> ().showCrosshair = true;
					Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 60f, 0.09f);
				}
			}
			shootTimer -= Time.deltaTime;

			if (Input.GetKey (KeyCode.LeftShift) && this.GetComponent<CharacterController> ().isGrounded && this.GetComponent<CharacterController> ().velocity.magnitude != 0) {
				pivot.GetComponent<Crosshair> ().showCrosshair = false;
				running = true;
				cam.transform.parent.GetComponent<ShakeCamera> ().running = true;
				if (scoped) {
					currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount * 2;
					currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount * 2;
					controller.m_WalkSpeed = controller.m_WalkSpeed * 2;
					photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopScope");
					scoped = false;
				}
				pivot.GetComponent<Animator> ().SetBool ("Running", true);
			} else {
				running = false;
				pivot.GetComponent<Animator> ().SetBool ("Running", false);
				cam.transform.parent.GetComponent<ShakeCamera> ().running = false;
			}
			if (canShoot) {
				if (!reloading && !running) {
					if (Input.GetButtonDown ("Fire2")) {
						scoped = !scoped;
						if (scoped) {
							pivot.GetComponent<Animator> ().SetTrigger ("ScopeIn");
							currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount / 2;
							currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount / 2;
							controller.m_WalkSpeed = controller.m_WalkSpeed / 2;
							photonView.RPC ("Animate", PhotonTargets.AllBuffered, "startScope");
						} else {
							pivot.GetComponent<Animator> ().SetTrigger ("ScopeOut");
							currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount * 2;
							currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount * 2;
							controller.m_WalkSpeed = controller.m_WalkSpeed * 2;
							photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopScope");
						}
					}
				}

				if (!reloading && shootTimer < 0) {
					if (Input.GetKeyDown (KeyCode.R)) {
						StartCoroutine (Reload ());
					}
				}

				if (!reloading && shootTimer < 0 && !running) {
					if (Input.GetButtonDown ("Fire1")  || wantsToShoot) {
						if (currentGun.GetComponent<GunProperties>().ammoInMag > 0) {
							shootTimer = currentGun.GetComponent<GunProperties>().timeBetweenShots;
							StartCoroutine (Shoot ());
							photonView.RPC ("Animate", PhotonTargets.AllBuffered, "startFire");
						} else {
							StartCoroutine (Reload ());
						}
					}

				}
			}
		}
	}
	

	public void OnLadder()
	{
		canShoot = false;
		currentGun.GetComponent<Animator> ().SetTrigger ("Ladder");
		if (scoped) {
			remScoped = true;
			scoped = false;
			pivot.GetComponent<Animator> ().SetTrigger ("ScopeOut");
			controller.m_WalkSpeed = controller.m_WalkSpeed * 2;
			currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount * 2;
			currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount * 2;
			photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopScope");
		}
	}
	public void OffLadder()
	{
		currentGun.GetComponent<Animator> ().SetTrigger ("Draw");
		StartCoroutine (Draw ());
	}

	IEnumerator Draw()
	{
		yield return new WaitForSeconds (0.8f);
		canShoot = true;
		if (remScoped) {
			scoped = true;
			pivot.GetComponent<Animator> ().SetTrigger ("ScopeIn");
			currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount / 2;
			currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount / 2;
			controller.m_WalkSpeed = controller.m_WalkSpeed / 2;
			photonView.RPC ("Animate", PhotonTargets.AllBuffered, "startScope");
		}
	}

	public IEnumerator Reload()
	{
		if (!reloading) {
			if (scoped) {
				remScoped = true;
				scoped = false;
				pivot.GetComponent<Animator> ().SetTrigger ("ScopeOut");
				controller.m_WalkSpeed = controller.m_WalkSpeed * 2;
				currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount * 2;
				currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount * 2;
				photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopScope");
			}
			reloading = true;
			currentGun.GetComponent<Animator>().SetTrigger ("Reload");
			photonView.RPC ("Animate", PhotonTargets.AllBuffered, "startReload");
			yield return new WaitForSeconds (0.8f);
			ad.PlayOneShot (magOut);
			yield return new WaitForSeconds (1.2f);
			ad.PlayOneShot (magIn);
			yield return new WaitForSeconds (0.7f);
			ad.PlayOneShot (magBolt);
			yield return new WaitForSeconds (0.2f);
			currentGun.GetComponent<GunProperties>().mags--;
			currentGun.GetComponent<GunProperties>().ammoInMag = currentGun.GetComponent<GunProperties>().ammoPerMag;
			reloading = false;
			yield return new WaitForSeconds (0.2f);
			if (remScoped) {
				scoped = true;
				pivot.GetComponent<Animator> ().SetTrigger ("ScopeIn");
				currentGun.GetComponent<WeaponSway> ().amount = currentGun.GetComponent<WeaponSway> ().amount / 2;
				currentGun.GetComponent<WeaponSway> ().maxAmount = currentGun.GetComponent<WeaponSway> ().maxAmount / 2;
				controller.m_WalkSpeed = controller.m_WalkSpeed / 2;
				photonView.RPC ("Animate", PhotonTargets.AllBuffered, "startScope");
			}
			photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopReload");
		}
	}

	public IEnumerator Shoot()
	{
		cam.transform.parent.GetComponent<ShakeCamera> ().GunShake ();
		currentGun.GetComponent<Animator>().SetTrigger ("Fire");
		yield return new WaitForSeconds (0.01f);
		ad.PlayOneShot (currentGun.GetComponent<GunProperties>().shoot);
		yield return new WaitForSeconds (0.08f);
		Raycast ();
		currentGun.GetComponent<GunProperties>().ammoInMag = currentGun.GetComponent<GunProperties>().ammoInMag - 1;
		muzzleLight.SetActive (true);
		muzzleFlash.SetActive (true);
		yield return new WaitForSeconds (0.11f);
		muzzleLight.SetActive (false);
		yield return new WaitForSeconds (0.04f);
		muzzleFlash.SetActive (false);
		photonView.RPC ("Animate", PhotonTargets.AllBuffered, "stopFire");
	}

	public void Raycast() {
		RaycastHit hit;
		Vector3 direction = muzzleLight.transform.TransformDirection(Vector3.forward);
		bulletSpreadX = Random.Range(-0.01f, 0.01f);
		bulletSpreadY = Random.Range(-0.01f, 0.01f);
		if (scoped) {
			bulletSpreadX = 0;
			bulletSpreadY = 0;
		}
		Instantiate (TracerRound, muzzleLight.transform.position + spread, cam.transform.rotation);
		spread = new Vector3 (bulletSpreadX, bulletSpreadY, 0);
		if (Physics.Raycast (muzzleLight.transform.position + spread, direction, out hit, distanceFired)) {
			hasRaycast (hit);
		}
	}

	public void RaycastObject(RaycastHit penetrated) {
		timesPenetrated++;
		RaycastHit hit;
		Vector3 direction = muzzleLight.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast (penetrated.point, direction, out hit, 50) && timesPenetrated < 3) {
			hasRaycast (hit);
		} else {
			timesPenetrated = 0;
		}
	}

	[PunRPC]
	public void BulletHoleInst(string name, Quaternion hitRotation, Vector3 point)
	{
		GameObject hole = Instantiate(bulletHole, point, hitRotation) as GameObject;
		hole.transform.SetParent (GameObject.Find(name).transform, true);
	}

	public void hasRaycast(RaycastHit hit)
	{
		var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		if(hit.transform.tag == "Metal" || hit.transform.tag == "Glass" || hit.transform.tag == "Wood" || hit.transform.tag == "Dirt" || hit.transform.tag == "Brick")
		{
			PhotonView pv = transform.GetComponent<PhotonView> ();
			//hit.transform.gameObject.name += Random.Range (0, 999999999);
			pv.RPC ("BulletHoleInst", PhotonTargets.All, hit.transform.gameObject.name, hitRotation, hit.point);


			if (hit.transform.gameObject.GetComponent<explosive> ()) {
				pv = hit.transform.GetComponent<PhotonView> ();
				pv.RPC("ApplyDamage", PhotonTargets.AllBuffered, currentGun.GetComponent<GunProperties>().damage);
			}
		}
		if (hit.transform.tag == "Glass") {
			RaycastObject (hit);
			PhotonNetwork.Instantiate("GlassImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Wood") {
			RaycastObject (hit);
			PhotonNetwork.Instantiate("WoodImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Dirt") {
			PhotonNetwork.Instantiate("GroundImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Water") {
			PhotonNetwork.Instantiate("WaterImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Brick") {
			PhotonNetwork.Instantiate("GroundImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Metal") {
			PhotonNetwork.Instantiate("MetalImpact", hit.point, hitRotation, 0);
		}
		if (hit.transform.tag == "Enemy") {
			PhotonNetwork.Instantiate("BloodSplat", hit.point, hitRotation, 0);
			Headshot hs = hit.transform.GetComponent<Headshot>();
			GameObject gm = hs.parent;
			PhotonView pv = gm.GetComponent<PhotonView>();
			int dmg = Mathf.RoundToInt(currentGun.GetComponent<GunProperties>().damage * hit.transform.GetComponent<Headshot> ().multiplier);
			pv.RPC("ApplyDamage", PhotonTargets.AllBuffered, dmg, photonView.ownerId);
			RaycastObject (hit);
		}
		if (hit.transform.gameObject.GetComponent<Rigidbody> ()) {
			hit.transform.gameObject.GetComponent<Rigidbody> ().AddForce(muzzleLight.transform.forward * 500);
		}
		//StartCoroutine (SFX (hit));
	}

	[PunRPC]
	public void Kill(int KillerID, int VictimID)
	{
		PhotonPlayer killer = null;
		PhotonPlayer victim = null;
		foreach (PhotonPlayer p in PhotonNetwork.playerList) {
			if(p.ID == KillerID)
			{
				killer = p;
			}
			if(p.ID == VictimID)
			{
				victim = p;
			}
		}
		GameObject.Find ("_Manager").GetComponent<Manager> ().Log (killer.NickName + " has killed " + victim.NickName);
		if(killer == PhotonNetwork.player)
		{
			GameObject.Find ("_Manager").GetComponent<Manager> ().kills++;
			if (GameObject.Find ("_Manager").GetComponent<Manager> ().currentGamemode == "TeamDeathmatch") {
				
			}
		}
		if(victim == PhotonNetwork.player)
		{
			GameObject.Find ("_Manager").GetComponent<Manager> ().deaths++;

		}
	}

	[PunRPC]
	public void ApplyDamage(int dmg, int id)
	{
		health -= dmg;
		if (photonView.isMine) {
			cam.transform.parent.GetComponent<ShakeCamera> ().GunShake ();
			if (!regen) {
				InvokeRepeating ("InitHealthRegen", 10f, 7f);
				regen = true;
			}
		}
		if (health < 0) {
			if (photonView.isMine) {
				hb.enabled = false;
				scoped = false;
				//GameObject.Find ("_Manager").GetComponent<Manager> ().loading.SetActive (true);
				//PhotonNetwork.Destroy (this.gameObject);
				canShoot = false;
				controller.enabled = false;
				photonView.RPC ("Kill", PhotonTargets.All, id, photonView.ownerId);
				cam.SetActive (false);
				deathCam.SetActive (true);
				ragdoll.SetActive (true);
				gunDrop.SetActive (true);
				respawnTimer = 5f;
			} else {
				modelAnim.gameObject.SetActive (false);
				ragdoll.SetActive (true);
				gunDrop.SetActive (true);
			}
		}
	}

}
