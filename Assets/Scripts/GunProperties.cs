using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProperties : MonoBehaviour {
	public int damage;
	public int ammoPerMag;
	public int ammoInMag;
	public int mags;
	public float reloadTime;
	public float timeBetweenShots;
	public AudioClip shoot;
	public GameObject muzzleLight;
	public GameObject muzzleFlash;
	public float maxDistance;
}
