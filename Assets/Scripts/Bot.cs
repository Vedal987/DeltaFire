using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bot : MonoBehaviour {

	public GameObject target;
	UnityEngine.AI.NavMeshAgent agent;
	public int health;
	public float distanceToTarget;
	public float distanceToCover;
	public float timeBetweenShot;
	public int damage;
	public ParticleEmitter muzzleFlash;
	public GameObject muzzleLight;
	public AudioClip shoot;
	public AudioSource audio;
	public Animator anim;
	public bool isMoving = false;
	public GameObject player;
	public Transform shootFrom;
	public bool tryShoot = false;
	public bool isPlayerInSight = false;
	public Vector3 wayPoint;
	public GameObject nearbyFriendly;
	public float timeBetweenMove;
	public bool isInCover = false;
	public bool isFindingCover = false;
	public List<GameObject> Cover;
	public GameObject currentCover = null;
	public GameObject ragdoll;

	public GameObject bulletHole;
	public GameObject woodImpact;
	public GameObject metalImpact;
	public GameObject glassImpact;
	public GameObject bulletTracer;

	public AudioClip metal1;
	public AudioClip metal2;
	public AudioClip metal3;

	public AudioClip wood1;
	public AudioClip wood2;
	public AudioClip wood3;

	public AudioClip glass1;
	public AudioClip glass2;
	public AudioClip glass3;

	public AudioSource sfx;

	void Start () {
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		audio = GetComponent<AudioSource> ();
		anim = transform.GetChild (0).GetComponent<Animator> ();
		health = 100;
		timeBetweenMove = Random.Range(0, 10);
	}

	void Update () {
		if (agent.hasPath) {
			isMoving = true;
			canSeePlayer ();
		} else {
			isMoving = false;
			isInCover = false;
		}
		if (target != null) {
			canSeePlayer ();
			if (agent.hasPath) {
				tryShoot = true;
			}
			distanceToTarget = Vector3.Distance (transform.position, target.transform.position);
			if (currentCover != null) {
				distanceToCover = Vector3.Distance (transform.position, currentCover.transform.position);
				if (distanceToCover < 1) {
					agent.Stop ();
					isMoving = false;
					isInCover = true;
					isFindingCover = false;
				}
				if (isInCover) {
					tryShoot = true;
				}
			}
			if (isInCover) {
				transform.LookAt (target.transform.position);
			}
			if (distanceToTarget > 20) {
				foreach (GameObject cover in Cover) {
					if (Vector3.Distance (transform.position, cover.transform.position) < distanceToTarget) {
						agent.SetDestination (cover.transform.position);
						currentCover = cover;
						isFindingCover = true;
					} else {
						if (isFindingCover = false) {
							agent.SetDestination (target.transform.position);
						}
					}
				}
			} else {
				agent.SetDestination (target.transform.position);
			}
		} else if (agent.hasPath == false && !isInCover) {
			if (timeBetweenMove < 0) {
				wayPoint = Random.insideUnitSphere * 50;
				wayPoint.y = 6f;
				agent.SetDestination (wayPoint);
				timeBetweenMove = Random.Range(0, 10);
			}
		}
		if (timeBetweenShot < 0) {
			if (tryShoot) {
				StartCoroutine (Shoot ());
				tryShoot = false;
			}
		}
		timeBetweenShot -= Time.deltaTime;
		if (agent.hasPath == false) {
			timeBetweenMove -= Time.deltaTime;
		}
		anim.SetBool ("isMoving", isMoving);
		if (nearbyFriendly != null) {
			if (nearbyFriendly.GetComponent<Bot>().target != null) {
				target = nearbyFriendly.GetComponent<Bot> ().target;
			}
		}
	}

	public void canSeePlayer ()
	{
		RaycastHit hit;
		Vector3 direction = shootFrom.transform.TransformDirection(Vector3.forward);
		if (target != null) {
			transform.LookAt (target.transform.position);
		}
		if (Physics.Raycast (shootFrom.position, direction, out hit, 500)) {
			if (hit.transform.gameObject.tag == "Player") {
				tryShoot = true;
				target = hit.transform.gameObject;
			}
		}
	}

	void ApplyDamage(int damage)
	{
		health -= damage;
		target = player.gameObject;
		this.gameObject.transform.LookAt (player.transform.position);
		StartCoroutine (Alarm ());
		if (health < 1) {
			Instantiate (ragdoll, this.transform.position, Quaternion.identity);
			Destroy (this.gameObject);
		}
	}

	public IEnumerator Alarm()
	{
		yield return new WaitForSeconds (7);
		GameObject alarmSound = GameObject.FindGameObjectWithTag ("Siren");
		if (alarmSound.GetComponent<AudioSource> ().isPlaying == false) {
			alarmSound.GetComponent<AudioSource> ().Play ();
		}
	}

	public void OnTriggerEnter(Collider col)
	{
		if (col.transform.gameObject.tag == "Player") {
			target = col.transform.gameObject;
		}
		if (col.transform.gameObject.tag == "Enemy") {
			nearbyFriendly = col.transform.gameObject;
		}
	}
	public void OnTriggerExit(Collider col)
	{
		if (col.transform.gameObject.tag == "Enemy") {
			nearbyFriendly = null;
		}
	}

	IEnumerator Shoot()
	{
		timeBetweenShot = Random.Range(0.2f, 0.8f);
		yield return new WaitForSeconds (0.1f);
		int miss = Random.Range (0, 10);
		if (miss == 2) {
		} else {
			RaycastHit hit;
			Vector3 direction = shootFrom.transform.TransformDirection(Vector3.forward);
			Vector3 spread = new Vector3((Random.Range(0, 5) / 10),(Random.Range(0, 5) / 10), 0);
			if (Physics.Raycast (shootFrom.position + spread, direction, out hit, 500)) {
				hasRaycast (hit);
			}
			StartCoroutine (HasShot ());
		}
	}

	IEnumerator HasShot()
	{
		audio.PlayOneShot (shoot);
		yield return new WaitForSeconds (0.2f);
		muzzleFlash.Emit ();
		muzzleLight.SetActive (true);
		yield return new WaitForSeconds (0.2f);
		muzzleLight.SetActive (false);
	}

	public void RaycastObject(GameObject penetrated) {
		RaycastHit hit;
		Vector3 direction = shootFrom.transform.TransformDirection(Vector3.forward);
		if (Physics.Raycast (penetrated.transform.position, direction, out hit, 50)) {
			hasRaycast (hit);
		}
	}

	public void hasRaycast(RaycastHit hit)
	{
		var hitRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		if (hit.transform.gameObject.tag == "Player") {
			hit.transform.GetChild (0).GetChild (0).GetChild (0).SendMessage ("ApplyDamage", damage);
			Instantiate (bulletTracer, shootFrom.position, shootFrom.rotation);
		}
		if(hit.transform.tag == "Metal" || hit.transform.tag == "Glass" || hit.transform.tag == "Wood")
		{
			Instantiate(bulletHole, hit.point, hitRotation);
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
		StartCoroutine (SFX (hit));
	}

	IEnumerator SFX(RaycastHit hit)
	{
		yield return new WaitForSeconds (0.3f);
		if (hit.transform) {
			if (hit.transform.tag == "Glass") {
				int ran = Random.Range (1, 3);
				if (ran == 1) {
					sfx.PlayOneShot (glass1);
				}
				if (ran == 2) {
					sfx.PlayOneShot (glass2);
				}
				if (ran == 3) {
					sfx.PlayOneShot (glass3);
				}
			}
			if (hit.transform.tag == "Wood") {
				int ran = Random.Range (1, 3);
				if (ran == 1) {
					sfx.PlayOneShot (wood1);
				}
				if (ran == 2) {
					sfx.PlayOneShot (wood2);
				}
				if (ran == 3) {
					sfx.PlayOneShot (wood3);
				}
			}
			if (hit.transform.tag == "Metal") {
				int ran = Random.Range (1, 3);
				if (ran == 1) {
					sfx.PlayOneShot (metal1);
				}
				if (ran == 2) {
					sfx.PlayOneShot (metal2);
				}
				if (ran == 3) {
					sfx.PlayOneShot (metal3);
				}
			}
		}
	}
}
