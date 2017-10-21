using UnityEngine;
using System.Collections;

public class Headshot : MonoBehaviour {

	public GameObject parent;
	public float multiplier;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void ApplyDamage(int damage)
	{
		parent.SendMessage ("ApplyDamage", damage * multiplier);
		Debug.Log ("HEADSHOT");
	}
}
