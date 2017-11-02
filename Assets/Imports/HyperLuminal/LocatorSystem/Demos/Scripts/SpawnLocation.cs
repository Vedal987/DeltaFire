using UnityEngine;
using System.Collections;
using HyperLuminalGames;

public class SpawnLocation : MonoBehaviour 
{
	// the location prefab to spawn set in the inspector.
	public GameObject LocationMarker;

	// Update is called once per frame.
	void Update () 
	{
		// if we press the space bar.
		if(Input.GetKeyDown(KeyCode.Space))
		{
				// spawn a new location marker
				Instantiate(LocationMarker, new Vector3(Random.Range(-10.0f, 10.0f),Random.Range(-10.0f, 10.0f),Random.Range(0.0f, 10.0f)), Quaternion.identity);
		}
	}
}
