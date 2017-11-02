using UnityEngine;
using System.Collections;
using HyperLuminalGames;

public class ToggleLocationMarker : MonoBehaviour 
{
	// keep track of the time between toggles.
	private float time = 0.0f;

	// accessible variable for toggle time period.
	public float ToggleTimer = 2.5f;

	// local copy of this location script.
	public Location LocationScript;

	// Update is called once per frame.
	void Update () 
	{
		// track the amount of time it has been in one state.
		time += Time.deltaTime;

		// change state after a certain time period.
		if(time > ToggleTimer)
		{
			// toggle the location marker active/unactive.
			LocationScript.enabled = !LocationScript.enabled;
	
			// reset the timer.
			time = 0.0f;
		}
	}
}
