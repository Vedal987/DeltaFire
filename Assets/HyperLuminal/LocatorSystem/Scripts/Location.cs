using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Location Marker Script.
	/// This script should be attached to every gameobject you want to track in game.
	/// </summary>
	public class Location : MonoBehaviour
    {

#if UNITY_4_5
        // display options for this location, set these in the inspector.
		[Space(10)]
		[Tooltip("TIP: Set your display icon from the Indicators folder or create your own.")]
	    public Texture DisplayIcon;
		public Color   IconColour    = Color.white;
		[Space(20)]

		[Tooltip("TIP: Set your display type to different styles.")]
		public enum DisplayTypeEnum 
		{
			None = 0,
			CustomText = 1,
			Distance = 2,
			Timer = 3,
		}
		public DisplayTypeEnum  Display_1_Type;
		[Tooltip("TIP: Distance between this camera and the location in which units?.")]
		public string Distance_1_Units;
		[Tooltip("TIP: Amount of time to have this location active for in seconds.")]
		public float Display_1_Timer;

		[Tooltip("TIP: Set the display text to whatever you want to say or leave it blank.")]
		public string  Display_1_Text;
		public Color   Text_1_Colour    = Color.white;
		[Space(20)]

		public DisplayTypeEnum  Display_2_Type;
		[Tooltip("TIP: Distance between this camera and the location in which units?.")]
		public string Distance_2_Units;
		[Tooltip("TIP: Amount of time to have this location active for in seconds.")]
		public float Display_2_Timer;
		
		[Tooltip("TIP: Set the display text to whatever you want to say or leave it blank.")]
		public string  Display_2_Text;
		public Color   Text_2_Colour    = Color.white;
		[Space(20)]


		[Tooltip("TIP: Set your arrow icon from the Pointer Arrows folder or create your own.")]
		public Texture PointerArrowIcon;
		public Color   PointerColour = Color.white;
		[Space(20)]

		[Tooltip("TIP: Display style for the location markers, edge of screen, circular or mini compass.")]
		public enum DisplayStyleEnum 
		{
			EdgeOfScreen = 0,
			Circular = 1,
			MiniCompass = 2
		}
		public DisplayStyleEnum  DisplayStyle;
		[Space(20)]

		[Tooltip("TIP: Pulsing can be used to indicate a warning or time limited location.")]
		public bool    PulsingAnimation;
		[Tooltip("TIP: Pulsing speed determines the rate that the icon scales.")]
		[Range(0, 5)] 
		public float   PulsingSpeed;
		[Space(20)]

		[Tooltip("TIP: Fades the Indicator over distance using the graph below.")]
		public bool    FadeOverDistance;
		[Tooltip("TIP: Maximum fading distance (where the graphs X Axis = 1).")]
		public float   MaxFadeDistance;
		[Tooltip("TIP: Y Axis = Alpha of Inidcator, X Axis = Distance from Location.")]
		public AnimationCurve FadeDistances;
		[Space(20)]

		[Tooltip("TIP: Hide either on or off screen display styles.")]
		public bool DisplayOnScreen = true;
		[Tooltip("TIP: Hide either on or off screen display styles.")]
		public bool DisplayOffScreen = true;

		[Tooltip("TIP: The icon displayed on the mini-map for this Location.")]
		public Texture2D Minimap_Display_Icon;

#else
        // display options for this location, set these in the inspector.
        // spacing and tooltips are only available in Unity 4.5+ please review the section above for more information.
        public Texture DisplayIcon;
        public Color IconColour = Color.white;
		public enum DisplayTypeEnum 
		{
			None = 0,
			CustomText = 1,
			Distance = 2,
			Timer = 3,
		}
		public DisplayTypeEnum  Display_1_Type;
		public string Distance_1_Units;
		public float Display_1_Timer;
		public string Display_1_Text;
		public Color Text_1_Colour = Color.white;
		public DisplayTypeEnum  Display_2_Type;
		public string Distance_2_Units;
		public float Display_2_Timer;
		public string Display_2_Text;
		public Color Text_2_Colour = Color.white;
        public Texture PointerArrowIcon;
        public Color PointerColour = Color.white;
		public enum DisplayStyleEnum 
		{
			EdgeOfScreen = 0,
			Circular = 1,
			MiniCompass = 2
		}
		public DisplayStyleEnum  DisplayStyle;
        public bool PulsingAnimation;
        public float PulsingSpeed;
        public bool FadeOverDistance;
        public float MaxFadeDistance;
        public AnimationCurve FadeDistances;
		public bool DisplayOnScreen = true;
		public bool DisplayOffScreen = true;
		public Texture2D Minimap_Display_Icon;

#endif
		[HideInInspector]
		public float LastTimerUpdate = 0.0f;

        // store a local copy of the display objects to save update time.
		private List<GameObject> thisOnScreenDisplayList = new List<GameObject>();
		private List<GameObject> thisOffScreenDisplayList = new List<GameObject>();
		private GameObject thisOnScreenDisplay;
		private GameObject thisOffScreenDisplay;


		// initialise the screen display objects.
		public void Init(GameObject LocationManager, GameObject TrackingSystem, string layer)
	    {
			// some warning handling to ensure the user has the correct display type that they are expecting.
			if(DisplayIcon == null)
			{
				Debug.LogWarning("Display Icon has not been assigned on Location: " + this.name + ". This is not essential but may not be desired.");
			}

			if(Display_1_Text == null)
			{
				Debug.LogWarning("Display 1 Text has not been assigned on Location: " + this.name + ". This is not essential but may not be desired.");
			}

			if(Display_2_Text == null)
			{
				Debug.LogWarning("Display 2 Text has not been assigned on Location: " + this.name + ". This is not essential but may not be desired.");
			}

			if(Minimap_Display_Icon == null)
			{
				Debug.LogWarning("Minimap Display Icon has not been assigned on Location: " + this.name + ". This is not essential but may not be desired.");
			}

			// get the location tracking system for this waypoint setup.
			LocationTrackingSystem thisTrackingSystem = TrackingSystem.GetComponent<LocationTrackingSystem>();

			// create the onscreen display object.
			if(thisTrackingSystem.OnScreenDisplayPrefab != null)
			{
				thisOnScreenDisplay = Instantiate(thisTrackingSystem.OnScreenDisplayPrefab, TrackingSystem.transform.position, Quaternion.identity) as GameObject;
				thisOnScreenDisplay.transform.parent = LocationManager.transform;
				thisOnScreenDisplay.name += " - " + this.name;
				thisOnScreenDisplay.GetComponent<OnScreenDisplay>().Init(this, thisTrackingSystem);
				SetLayerOnAll(thisOnScreenDisplay, LayerMask.NameToLayer(layer));
			}

			// create the offscreen display object.
			if(thisTrackingSystem.OffScreenDisplayPrefab != null)
			{
				thisOffScreenDisplay = Instantiate(thisTrackingSystem.OffScreenDisplayPrefab, TrackingSystem.transform.position, Quaternion.identity) as GameObject;
				thisOffScreenDisplay.transform.parent = LocationManager.transform;
				thisOffScreenDisplay.name += " - " + this.name;
				thisOffScreenDisplay.GetComponent<OffScreenDisplay>().Init(this, thisTrackingSystem);
				SetLayerOnAll(thisOffScreenDisplay, LayerMask.NameToLayer(layer));
			}

			// add the display objects to their requred lists for tracking.
			thisOnScreenDisplayList.Add(thisOnScreenDisplay);
			thisOffScreenDisplayList.Add(thisOffScreenDisplay);
		}

		void Update()
		{
			// if we are using the timer display.
			if(Display_1_Type == DisplayTypeEnum.Timer)
			{
				// handle timer code.
				Display_1_Timer -= Time.deltaTime;
			}

			// if we are using the timer display.
			if(Display_2_Type == DisplayTypeEnum.Timer)
			{
				// handle timer code.
				Display_2_Timer -= Time.deltaTime;
			}
		}

		private void SetLayerOnAll(GameObject obj, LayerMask layer) 
		{
			// set the layer on every game object in the passed in objects hierarchy.
			foreach (Transform transform in obj.GetComponentsInChildren<Transform>(true)) 
			{
				// at this stage we trust the layer is correct and simply assign it.
				transform.gameObject.layer = layer;
			}
		}

		void OnDisable()
		{
			DoDestroy();
		}
		
		public void DoDestroy()
		{
			// check if we have any display objects and make sure they are destroyed. 
			while(thisOffScreenDisplayList.Count > 0)
			{
				Destroy(thisOffScreenDisplayList[0]);
				thisOffScreenDisplayList.RemoveAt(0);
			}
			
			// check if we have any display objects and make sure they are destroyed. 
			while(thisOnScreenDisplayList.Count > 0)
			{
				Destroy(thisOnScreenDisplayList[0]);
				thisOnScreenDisplayList.RemoveAt(0);
			}
		}
	}
}