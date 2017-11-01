using UnityEngine;
using System;
using System.Collections;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Location Tracking Display Script.
	/// This script is the base for both on screen and off screen display classes.
	/// </summary>
	public abstract class LocationTrackingDisplay : MonoBehaviour
	{
		// location marker display options.
		[HideInInspector]
		public Texture DisplayIcon;
		[HideInInspector]
		public string  Display_1_Text;
		[HideInInspector]
		public string  Display_2_Text;
		[HideInInspector]
		public bool    FadeOverDistance;
		[HideInInspector]
		public Color   IconColour;
		[HideInInspector]
		public Color   Text_1_Colour;
		[HideInInspector]
		public Color   Text_2_Colour;
		[HideInInspector]
		public Color   PointerColour;
		[HideInInspector]
		public float Distance;
		[HideInInspector]
		public float SmoothAlpha;

		// local copy of this locations display camera.
		protected Camera thisLocationsCamera;

		// local copy of this location.
		protected Location thisLocation;

		// local copy of this objects transform component.
		protected Transform thisTransform;

		// pulsing direction and initial scale sizes.
		private bool PulseIncrease = false;
		private Vector3 InitialScale;

		// initialise the setup for this location display object.
		public virtual void Init(Location location, LocationTrackingSystem thisTrackingSystem)
		{
			// get a local copy of the location object.
			thisLocation = location;

			// get a local copy of the objects transform.
			thisTransform = this.transform;

			// get a copy of this objects initial scale size.
			InitialScale = this.transform.localScale;

			// get a local copy of this locations tracking system camera.
			thisLocationsCamera = thisTrackingSystem.SystemCamera;

			// get the display option information from the location.
			UpdateDisplayInfo();
		}

		public void UpdateDisplayInfo()
		{
			// update any display option information for this location.
			DisplayIcon      = thisLocation.DisplayIcon;
			FadeOverDistance = thisLocation.FadeOverDistance;
			IconColour       = thisLocation.IconColour;
			Text_1_Colour    = thisLocation.Text_1_Colour;
			Text_2_Colour    = thisLocation.Text_2_Colour;
			PointerColour    = thisLocation.PointerColour;

			// calculate the distance from this location to the camera.
			float distance = Mathf.Abs(Vector3.Distance(thisLocationsCamera.transform.position, thisLocation.transform.position));

			// check if the fade over distance variable is required.
			if(FadeOverDistance)
			{
				// iterate over each of the graphs key frames.
				for(int i = 0; i < thisLocation.FadeDistances.keys.Length; i++)
				{
					// if we have past a key frame then start to lerp towards its value.
					if(distance > (thisLocation.FadeDistances[i].time * thisLocation.MaxFadeDistance))
					{
						// produce a smooth alpha transition.
						SmoothAlpha = thisLocation.FadeDistances[i].value;
					}
				}

				// reduce the alpha value of this location marker.
				float newAlpha = Mathf.Lerp(IconColour.a, SmoothAlpha, Time.deltaTime * 1000.0f);
				IconColour = new Color(IconColour.r, IconColour.g, IconColour.b, newAlpha);

				newAlpha = Mathf.Lerp(Text_1_Colour.a, SmoothAlpha, Time.deltaTime * 1000.0f);
				Text_1_Colour = new Color(Text_1_Colour.r, Text_1_Colour.g, Text_1_Colour.b, newAlpha);

				newAlpha = Mathf.Lerp(Text_2_Colour.a, SmoothAlpha, Time.deltaTime * 1000.0f);
				Text_2_Colour = new Color(Text_2_Colour.r, Text_2_Colour.g, Text_2_Colour.b, newAlpha);

				newAlpha = Mathf.Lerp(PointerColour.a, SmoothAlpha, Time.deltaTime * 1000.0f);
				PointerColour = new Color(PointerColour.r, PointerColour.g, PointerColour.b, newAlpha);
			}

			// assign the distance value.
			Distance = distance;

			// if we should pulse this location marker.
			if(thisLocation.PulsingAnimation)
			{
				// limit the scaling amount.
				if(this.transform.localScale.x > (InitialScale.x * 1.1f))
				{
					PulseIncrease = false;
				}

				// limit the scaling amount.
				if(this.transform.localScale.x < (InitialScale.x * 0.9f))
				{
					PulseIncrease = true;
				}

				// do the scaling pulse animation process.
				if(PulseIncrease)
				{
					this.transform.localScale += (this.transform.localScale * (thisLocation.PulsingSpeed * Time.deltaTime));
				}
				else
				{
					this.transform.localScale -= (this.transform.localScale * (thisLocation.PulsingSpeed * Time.deltaTime));
				}
			}
		
			// handle text display settings.
			if(thisLocation.Display_1_Type == Location.DisplayTypeEnum.None)
			{
				Display_1_Text = "";
			}
			else if(thisLocation.Display_1_Type == Location.DisplayTypeEnum.CustomText)
			{
				// display the custom text set by the user inside the Unity Inspector.
				Display_1_Text = thisLocation.Display_1_Text;
			}
			else if(thisLocation.Display_1_Type == Location.DisplayTypeEnum.Distance)
			{
				// display the distance between this camera and the location using the set units.
				Display_1_Text = Distance.ToString("f0") + " " + thisLocation.Distance_1_Units;
			}
			else if(thisLocation.Display_1_Type == Location.DisplayTypeEnum.Timer)
			{
				// if we run out of time.
				if(thisLocation.Display_1_Timer < 0.0f)
				{
					// disable this location marker.
					thisLocation.DisplayOffScreen = false;
					thisLocation.DisplayOnScreen = false;
				}

				// display the time left on the timer.
				Display_1_Text = thisLocation.Display_1_Timer.ToString("f0");
			}

			// handle text display settings.
			if(thisLocation.Display_2_Type == Location.DisplayTypeEnum.None)
			{
				Display_2_Text = "";
			}
			else if(thisLocation.Display_2_Type == Location.DisplayTypeEnum.CustomText)
			{
				// display the custom text set by the user inside the Unity Inspector.
				Display_2_Text = thisLocation.Display_2_Text;
			}
			else if(thisLocation.Display_2_Type == Location.DisplayTypeEnum.Distance)
			{
				// display the distance between this camera and the location using the set units.
				Display_2_Text = Distance.ToString("f0") + " " + thisLocation.Distance_2_Units;
			}
			else if(thisLocation.Display_2_Type == Location.DisplayTypeEnum.Timer)
			{
				// if we run out of time.
				if(thisLocation.Display_2_Timer < 0.0f)
				{
					// disable this location marker.
					thisLocation.DisplayOffScreen = false;
					thisLocation.DisplayOnScreen = false;
				}
				
				// display the time left on the timer.
				Display_2_Text = thisLocation.Display_2_Timer.ToString("f0");
			}
		}
			
		// get the current screen position of this display.
	    protected Vector3 GetScreenPosition()
	    {
			// convert the locations position to screen space using this locations system camera.
			Vector3 ScreenPosition = thisLocationsCamera.WorldToScreenPoint(thisLocation.transform.position);
			ScreenPosition.x = (ScreenPosition.x / thisLocationsCamera.pixelWidth) + thisLocationsCamera.pixelRect.x;
			ScreenPosition.y = (ScreenPosition.y / thisLocationsCamera.pixelHeight)+ thisLocationsCamera.pixelRect.y;
			ScreenPosition.z = thisTransform.position.z;

			// return the resulting position.
			return ScreenPosition;
	    }

	    protected bool OnScreen(Vector3 screenPos)
	    {
			// first check we have the component.
			if(thisLocation.gameObject.GetComponent<Renderer>() != null)
			{
				// check if we are on screen by checking the against the camera frustum 
				Plane[] planes = GeometryUtility.CalculateFrustumPlanes(thisLocationsCamera);
				if(GeometryUtility.TestPlanesAABB(planes, thisLocation.gameObject.GetComponent<Renderer>().bounds))
				{
					// mesh rendering component
					return true;
				}
				return false;
			}
			else if(thisLocation.gameObject.GetComponent<Collider>() != null)
			{
				// check if we are on screen by checking the against the camera frustum 
				Plane[] planes = GeometryUtility.CalculateFrustumPlanes(thisLocationsCamera);
				if(GeometryUtility.TestPlanesAABB(planes, thisLocation.gameObject.GetComponent<Collider>().bounds))
				{
					// collider component
					return true;
				}
				return false;
			}
			else
			{
				// we need a rendering or colliding component to check if we are on screen.
				Debug.LogException(new UnityException("LOCATION MANAGER: " + thisLocation.name + " must have a render or collider attached to use the location tracker."));
				return false;
			}
		}
	}
}