using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Location Tracking System Script.
	/// This script is the core system setup script it should be attached to your in-game display camera.
	/// You can choose to attach On Screen and/or Off Screen prefabs.
	/// </summary>
	public class LocationTrackingSystem : MonoBehaviour
	{
		// the camera that this waypoint system uses to display on screen. 
		// this is automatically by this script.
		[HideInInspector]
	    public Camera SystemCamera;

		// the display objects used to show tracking information on screen.
		// set these objects in the inspector.
		public GameObject OnScreenDisplayPrefab;
		public GameObject OffScreenDisplayPrefab;

		// is a minimap required?
		public bool Display_Minimap = true;
		
		// set the minimap prefab
		public GameObject Minimap_Prefab;

		// the publicly accessible minimap gameobject.
		[HideInInspector]
		public GameObject Minimap_GameObject;

		void Awake()
		{
			// flag to detect if any errors have occured.
			bool ErrorExists = false;

			// check we are a camera object.
			if(this.gameObject.GetComponent<Camera>() == null)
			{

				Debug.LogException(new UnityException("LOCATION MANAGER: Please give " + this.name + " a camera component to use the Location Tracking Asset."));
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
				ErrorExists = true;
			}
			else
			{
				// get this camera object if we are attached to one for displaying the UI.
				SystemCamera = this.gameObject.GetComponent<Camera>();

				// error handling for display prefabs setup (for this version they are required!).
				if(OnScreenDisplayPrefab == null)
				{
					Debug.LogException(new UnityException("LOCATION MANAGER: On Screen Display Prefab has not been set on: " + this.name));
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
					ErrorExists = true;
				}
				
				if(OffScreenDisplayPrefab == null)
				{
					Debug.LogException(new UnityException("LOCATION MANAGER: Off Screen Display Prefab has not been set on: " + this.name));
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
					ErrorExists = true;
				}

				if(Display_Minimap)
				{
					if(Minimap_Prefab == null)
					{
						Debug.LogException(new UnityException("LOCATION MANAGER: Mini Map Display Prefab has not been set on: " + this.name));
#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
#endif
						ErrorExists = true;
					}
					else
					{
						// spawn the minimap for this system.
						Minimap_GameObject = Instantiate(Minimap_Prefab) as GameObject;
					}
				}


				// check if we had any setup errors.
				if(ErrorExists == false)
				{
					// initialise the Location Manager Object if we had no errors.
					GameObject.FindObjectOfType<LocationManager>().Init(this.GetComponent<LocationTrackingSystem>());
				}
			}
		}
	}
}