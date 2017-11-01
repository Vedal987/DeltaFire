using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Location Manager Script.
	/// This script should be attached to a gameobject in your game scene, simple drag in the prefab "LocationManager".
	/// </summary>
	public class LocationManager : MonoBehaviour 
	{
		// set the layer names you have choose in the editor.
		public List<string> DisplayLayerNames = new List<string>();

		// should the asset update in real-time or not?  
		// If you don't need to spawn any additional locations at run-time, set to false for extra performance.
		public bool Realtime_Updates = true;

		// keep track of the number of locations that have been activated.
		private int NumberOfActiveLocations = 0;

		// keep a list of all the location systems that have been activated so we can use them later
		private static List<LocationTrackingSystem> LocationSystems = new List<LocationTrackingSystem>();

		// tracking which system has been assigned which layer.
		private int LayerAssignment = 0;

		// a local copy of the minimap script if it is required.
		private MinimapDisplay Minimap;

		void Awake()
		{
			// when the level is loaded clear the list of systems.
			LocationSystems.Clear();
		}

		// Initializationof the Location Manager.
		public void Init (LocationTrackingSystem LocationSystem) 
		{
			// flag to detect if any errors have occured.
			bool ErrorExists = false;

			// add this location tracking system to our list if we don't already have it.
			if(LocationSystems.Contains(LocationSystem) == false)
			{
				LocationSystems.Add(LocationSystem);
			}

			// check layers have been set
			if(DisplayLayerNames.Count <= 0)
			{
				// layers are required to display the locations for specific cameras, even for single player mode a layer is required.
				// please add the layer name to this object in the inspector and also to the Unity Layers settings.
				Debug.LogException(new UnityException("LOCATION MANAGER: No Layers have been found please assign some layers to: " + this.name));
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
				ErrorExists = true;
			}
			else
			{
				// loop over each layer that has been set and ensure it exists.
				for(int i = 0; i < DisplayLayerNames.Count; i++)
				{
					// if layer int is returned as -1 it is not found.
					if(LayerMask.NameToLayer(DisplayLayerNames[i]) < 0)
					{
						// please add the layer name to the Unity Layers settings.
						// if this layer is unneeded then simply remove it from this object in the inspector.
						Debug.LogException(new UnityException("LOCATION MANAGER: Layer:  '" + DisplayLayerNames[i] + "' not found please add to layer settings."));
#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
#endif
						ErrorExists = true;
					}
				}

				// check we have not activated more systems than we have display layers.
				if(LocationSystems.Count > DisplayLayerNames.Count)
				{
					// not enough layers have been defined to support this many location systems.
					// Please define more layer names both on this object and in the layers settings.
					Debug.LogException(new UnityException("LOCATION MANAGER: Not enough layers have been defined to support this many Location systems: " + LocationSystems.Count + "."));
#if UNITY_EDITOR
					UnityEditor.EditorApplication.isPlaying = false;
#endif
					ErrorExists = true;
				}
					
				// if no errors exist then continue the location manager setup
				if(ErrorExists == false)
				{
					// increment the layer assignment number each time.
					LayerAssignment++;

					// turn off visibility for all other location markers using a layer mask
					for(int i = 0; i < DisplayLayerNames.Count; i++)
					{
						LocationSystem.SystemCamera.cullingMask &=  ~(1 << LayerMask.NameToLayer(DisplayLayerNames[i]));
					}

					// turn on visibility for only this players layer
					LocationSystem.SystemCamera.cullingMask |= 1 << LayerMask.NameToLayer( DisplayLayerNames[LayerAssignment - 1]);
					
					// get each of the scenes locations and check they are active.
					Location[] Locations = GameObject.FindObjectsOfType<Location>() as Location[];

					if(LocationSystem.Display_Minimap)
					{
						// get the minimap display if it is required.
						Minimap = LocationSystem.Minimap_GameObject.GetComponent<MinimapDisplay>();

						// assign the camera to this minimap.
						Minimap.Tracking_Camera = LocationSystem.SystemCamera;

						// get the later string and set it for this minimap.
						Minimap.Minimap_Layer = DisplayLayerNames[LayerAssignment - 1];
					}

					int ActiveLocations = 0;
					for(int i = 0; i < Locations.Length; i++)
					{
						if(Locations[i].enabled)
						{
							// initialise the location and set its system owner + layer ID
							Locations[i].Init(this.gameObject, LocationSystem.gameObject, DisplayLayerNames[LayerAssignment - 1]);

							if(LocationSystem.Display_Minimap)
							{
								// set this location in the minimap display if it is required.
								Minimap.Tracked_Locations.Add(Locations[i].GetComponent<Location>());
							}

							ActiveLocations++;
						}
					}
					
					// store the number of locations we currently have in the scene.
					NumberOfActiveLocations = ActiveLocations;
					
					// ensure this object is positioned at (0,0,0) at all times
					this.transform.position = Vector3.zero;
				}
			}
		}
		
		void Update()
		{
			// if we require dynamically updating location tracking.
			if(Realtime_Updates == true)
			{
				// check how many locations are currently active in the scene.
				Location[] Locations = GameObject.FindObjectsOfType<Location>() as Location[];
				
				int ActiveLocations = 0;
				foreach(Location loc in Locations)
				{
					if(loc.enabled)
					{
						// we found an active location.
						ActiveLocations++;
					}
				}
				
				// compare it against the last known count.
				if(ActiveLocations != NumberOfActiveLocations)
				{
					// delete all the children on this game object to clear all location markers.
					foreach(Location loc in Locations)
					{
						loc.DoDestroy();
					}

					// reset the layer assignment number.
					LayerAssignment = 0;

					// we have found a different number so update our system.
					foreach(LocationTrackingSystem system in LocationSystems)
					{
						// re-initialise each system.
						Init (system);				
					}
				}
			}
		}
	}
}