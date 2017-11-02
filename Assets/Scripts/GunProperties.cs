using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProperties : MonoBehaviour {
	public string name;
	[Header("Stats")]
	public int damage;
	public int ammoPerMag;
	public int ammoInMag;
	public int mags;
	public float reloadTime;
	public float timeBetweenShots;
	public AudioClip shoot;
	public GameObject muzzleLight;
	public GameObject muzzleFlash;
	public GameObject shellEject;
	public float maxDistance;
	[Header("Gun Shake Properties")]
	public float a;
	public float b;
	public float c;
	public float d;
}
