using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
	bool remScoped;

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


	public bool scoped;
	public bool running;

	// Use this for initialization
	void Start () {
		ad = gun.GetComponent<AudioSource> ();
		scoped = false;
	}
	
	// Update is called once per frame
	void Update () {
		shootTimer -= Time.deltaTime;
		if (Input.GetKey (KeyCode.LeftShift)) {
			running = true;
			if (scoped) {
				gun.GetComponent<WeaponSway> ().amount = gun.GetComponent<WeaponSway> ().amount * 2;
				gun.GetComponent<WeaponSway> ().maxAmount = gun.GetComponent<WeaponSway> ().maxAmount * 2;
				scoped = false;
			}
			pivot.GetComponent<Animator> ().SetBool ("Running", true);
		} else {
			running = false;
			pivot.GetComponent<Animator> ().SetBool ("Running", false);
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

		if (!reloading && shootTimer < 0) {
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
		pivot.GetComponent<ShakeCamera> ().GunShake ();
		int ran = Random.Range (0, shoot.Length - 1);
		gunAnim.SetTrigger ("Fire");
		yield return new WaitForSeconds (0.02f);
		ad.PlayOneShot (shoot [ran]);
		yield return new WaitForSeconds (0.02f);
		ammoInMag = ammoInMag - 1;
		muzzleLight.SetActive (true);
		muzzleFlash.SetActive (true);
		yield return new WaitForSeconds (0.16f);
		muzzleLight.SetActive (false);
		yield return new WaitForSeconds (0.04f);
		muzzleFlash.SetActive (false);
	}
}
