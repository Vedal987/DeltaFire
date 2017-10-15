using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour {

	public Light l;


	// Use this for initialization
	void Start () {
		l = this.GetComponent<Light> ();
		Repeat ();
	}
	
	// Update is called once per frame
	void Update () {
	}


	public void Repeat()
	{
		int ran = Random.Range (0, 6);
		if (ran == 1) {
			StartCoroutine (Flicker ());
		} else if (ran == 2) {
			l.intensity -= 0.2f;
		} else if (ran == 3) {
			l.intensity += 0.2f;
		} else {
			StartCoroutine (Wait ());
		}
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds (4f);
		Repeat ();
	}

	IEnumerator Flicker()
	{
		l.enabled = false;
		int ran = Random.Range (0, 13);
		yield return new WaitForSeconds (ran);
		l.enabled = true;
		yield return new WaitForSeconds (4f);
		Repeat ();
	}
}
