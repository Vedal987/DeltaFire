using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

	AudioSource ad;
	public AudioClip[] shoot;

	public int ammoPerMag;
	public int ammoInMag;
	public int mags;

	public GameObject gun;
	public Animator gunAnim;

	public GameObject muzzleLight;
	public GameObject muzzleFlash;

	// Use this for initialization
	void Start () {
		ad = gun.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			if (ammoInMag > 0) {
				StartCoroutine (Shoot ());
			} else {
				StartCoroutine (Reload ());
			}
		}
	}

	public IEnumerator Reload()
	{
		yield return new WaitForSeconds (0.5f);
	}

	public IEnumerator Shoot()
	{
		int ran = Random.Range (0, shoot.Length - 1);
		gunAnim.SetTrigger ("Fire");
		yield return new WaitForSeconds (0.02f);
		ad.PlayOneShot (shoot [ran]);
		yield return new WaitForSeconds (0.02f);
		muzzleLight.SetActive (true);
		muzzleFlash.SetActive (true);
		yield return new WaitForSeconds (0.02f);
		muzzleFlash.SetActive (false);
		yield return new WaitForSeconds (0.02f);
		muzzleLight.SetActive (false);
	}
}
