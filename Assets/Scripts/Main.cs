using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	bool remScoped;

	public int health;
	public int maxHealth;

	public int bulletDamage;

	AudioSource ad;
	public AudioClip[] shoot;

	public AudioClip magIn;
	public AudioClip magOut;
	public AudioClip magBolt;

	public float shootTimer;
	public float timeBetweenShots;

	public int ammoPerMag;
	public int ammoInMag;
	public int mags;
	public bool reloading;

	public GameObject gun;
	public Animator gunAnim;
	public GameObject pivot;
	public GameObject cam;

	public GameObject muzzleLight;
	public GameObject muzzleFlash;

	public GameObject TracerRound;

	public GameObject bulletHole;
	public GameObject glassImpact;
	public GameObject woodImpact;
	public GameObject metalImpact;
	public GameObject bloodSplat;

	public bool scoped;
	public bool running;

	float bulletSpreadX;
	float bulletSpreadY;
	Vector3 spread;
	int timesPenetrated;
	public float distanceFired;

	// Use this for initialization
	void Start () {
		ad = gun.GetComponent<AudioSource> ();
		scoped = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (scoped) {
			Camera.main.fieldOfView = Mathf.Lerp (Camera.main.fieldOfView, 40f, 0.09f);
		} else {
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60f, 0.09f);
		}

		shootTimer -= Time.deltaTime;
		if (Input.GetKey (KeyCode.LeftShift) && this.GetComponent<CharacterController>().isGrounded && this.GetComponent<CharacterController>().velocity.magnitude != 0) {
			running = true;
			cam.transform.parent.GetComponent<ShakeCamera> ().running = true;
			if (scoped) {
				gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount * 2;
				gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount * 2;
				scoped = false;
			}
			pivot.GetComponent<Animator> ().SetBool ("Running", true);
		} else {
			running = false;
			pivot.GetComponent<Animator> ().SetBool ("Running", false);
			cam.transform.parent.GetComponent<ShakeCamera> ().running = false;
		}
		if (!reloading && !running) {
			if (Input.GetButtonDown ("Fire2")) {
				scoped = !scoped;
				if (scoped) {
					pivot.GetComponent<Animator> ().SetTrigger ("ScopeIn");
					gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount / 2;
					gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount / 2;
				} else {
					pivot.GetComponent<Animator> ().SetTrigger ("ScopeOut");
					gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount * 2;
					gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount * 2;
				}
			}
		}

		if (!reloading && shootTimer < 0 && !running) {
			if (Input.GetButtonDown ("Fire1")) {
				if (ammoInMag > 0) {
					shootTimer = timeBetweenShots;
					StartCoroutine (Shoot ());
				} else {
					StartCoroutine (Reload ());
				}
			}
			if (Input.GetKeyDown (KeyCode.R)) {
				StartCoroutine (Reload ());
			}
		}
	}

	public IEnumerator Reload()
	{
		if (!reloading) {
			if (scoped) {
				remScoped = true;
				scoped = false;
				pivot.GetComponent<Animator> ().SetTrigger ("ScopeOut");
				gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount * 2;
				gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount * 2;
			}
			reloading = true;
			gunAnim.SetTrigger ("Reload");
			yield return new WaitForSeconds (0.8f);
			ad.PlayOneShot (magOut);
			yield return new WaitForSeconds (1.2f);
			ad.PlayOneShot (magIn);
			yield return new WaitForSeconds (0.7f);
			ad.PlayOneShot (magBolt);
			yield return new WaitForSeconds (0.2f);
			mags--;
			ammoInMag = ammoPerMag;
			reloading = false;
			yield return new WaitForSeconds (0.2f);
			if (remScoped) {
				scoped = true;
				pivot.GetComponent<Animator> ().SetTrigger ("ScopeIn");
				gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount / 2;
				gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount / 2;
			}
		}
	}

	public IEnumerator Shoot()
	{
		cam.transform.parent.GetComponent<ShakeCamera> ().GunShake ();
		int ran = Random.Range (0, shoot.Length - 1);
		gunAnim.SetTrigger ("Fire");
		yield return new WaitForSeconds (0.02f);
		ad.PlayOneShot (shoot [ran]);
		yield return new WaitForSeconds (0.02f);
		Raycast ();
		ammoInMag = ammoInMag - 1;
		muzzleLight.SetActive (true);
		muzzleFlash.SetActive (true);
		yield return new WaitForSeconds (0.16f);
		muzzleLight.SetActive (false);
		yield return new WaitForSeconds (0.04f);
		muzzleFlash.SetActive (false);
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

	public void RaycastObject(GameObject penetrated) {
		timesPenetrated++;
		RaycastHit hit;
		Vector3 direction = muzzleLight.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast (penetrated.transform.position, direction, out hit, 50) && timesPenetrated < 3) {
			hasRaycast (hit);
		} else {
			timesPenetrated = 0;
		}
	}

	public void hasRaycast(RaycastHit hit)
	{
		var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		if(hit.transform.tag == "Metal" || hit.transform.tag == "Glass" || hit.transform.tag == "Wood")
		{
			GameObject hole = Instantiate(bulletHole, hit.point, hitRotation) as GameObject;
			hole.transform.SetParent (hit.transform, true);
			if (hit.transform.gameObject.GetComponent<explosive> ()) {
				hit.transform.SendMessage ("ApplyDamage", bulletDamage);
			}
		}
		if (hit.transform.tag == "Glass") {
			RaycastObject (hit.transform.gameObject);
			Instantiate(glassImpact, hit.point, hitRotation);
		}
		if (hit.transform.tag == "Wood") {
			RaycastObject (hit.transform.gameObject);
			Instantiate(woodImpact, hit.point, hitRotation);
		}
		if (hit.transform.tag == "Metal") {
			Instantiate(metalImpact, hit.point, hitRotation);
		}
		if (hit.transform.tag == "Enemy") {
			Instantiate(bloodSplat, hit.point, hitRotation);
			//StartCoroutine (HitEnemy ());
			//hit.transform.SendMessage ("ApplyDamage", bulletDamage);
			RaycastObject (hit.transform.gameObject);
		}
		if (hit.transform.gameObject.GetComponent<Rigidbody> ()) {
			hit.transform.gameObject.GetComponent<Rigidbody> ().AddForce(muzzleLight.transform.forward * 500);
		}
		//StartCoroutine (SFX (hit));
	}


}
