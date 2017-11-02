using UnityEngine;
using System.Collections;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Off Screen Display Script.
	/// Inherits from Location Tracking Display Class.
	/// </summary>
	public class OffScreenDisplay : LocationTrackingDisplay
	{
		// inspector hook ups for the GUI display objects.
		public PointerArrow DirectionArrow;

		// secondary text display
		public GUIText Secondary_Text_Display;

		void Start()
		{// flag to detect if any errors have occured.
			bool ErrorExists = false;

			// check our display objects have been hooked into the inspector correctly.
			if(DirectionArrow == null)
			{
				// please ensure the On Screen prefab has its direction arrow object attached in the inspector.
				Debug.LogException(new UnityException("LOCATION MANAGER: OffScreen Prefab doesn't have DirectionArrow hooked up in the inspector."));
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
				ErrorExists = true;
			}

			// if no errors exist then continue setup.
			if(ErrorExists == false)
			{
				// assign this locations display camera.
				DirectionArrow.thisCamera = thisLocationsCamera;

				// update this locations display information.
				UpdateDisplayInfo();
			}
		}

		public void UpdateDisplayInfo()
		{
			base.UpdateDisplayInfo();

			// get the icon and text to use for this display.
			this.GetComponent<GUITexture>().texture = DisplayIcon;
			this.GetComponent<GUITexture>().color = IconColour;
			this.GetComponent<GUIText>().text = Display_1_Text;
			this.GetComponent<GUIText>().color = Text_1_Colour;

			Secondary_Text_Display.text = Display_2_Text;
			Secondary_Text_Display.color = Text_2_Colour;

			DirectionArrow.ArrowColor = PointerColour;
			DirectionArrow.ArrowTexture = thisLocation.PointerArrowIcon;
		}

	    protected void Update()
	    {
			if(thisLocation.DisplayOffScreen == false)
			{
				// if we should not display on screen then hide us.
				Hide(!true);
				return;
			}

			// update any display option information for this location.
			UpdateDisplayInfo();

			// get the current screen position for this location.
			Vector3 ScreenPosition = GetScreenPosition();

			// choose display mode (circular or screen edge).
			if(thisLocation.DisplayStyle == Location.DisplayStyleEnum.Circular)
			{
				// calculate the current position for this display object on screen.
				Vector3 Direction = thisLocation.transform.position - thisLocationsCamera.transform.position;
				Vector3 CameraRelativeDirection = thisLocationsCamera.transform.InverseTransformDirection(Direction);
				CameraRelativeDirection.z = 0;
				CameraRelativeDirection = CameraRelativeDirection.normalized / 2;
				ScreenPosition.x = 0.5f + CameraRelativeDirection.x * 0.75f;
				ScreenPosition.y = 0.5f + CameraRelativeDirection.y * 0.75f;

				// set this display objects position for rendering.
				thisTransform.position = ScreenPosition;
				
				// calculate the rotation for the direction arrow object.
				Vector2 direction = new Vector2(thisTransform.position.x - 0.5f, thisTransform.position.y - 0.5f);
				DirectionArrow.transform.position = thisTransform.position;
				DirectionArrow.RotationValue = Vector2.Angle(Vector2.up, direction.normalized);
				if (direction.x < 0){DirectionArrow.RotationValue *= -1.0f;}  

				// check to see if we are on screen or not.
				Hide(!OnScreen(ScreenPosition)); 
			}
			else if(thisLocation.DisplayStyle == Location.DisplayStyleEnum.EdgeOfScreen)
			{
				// calculate the current position for this display object on screen.
				bool Xhigher = false;

				// clamp to within the screen but stick to the edges.
				if(Mathf.Abs(ScreenPosition.x -0.5f) > Mathf.Abs(ScreenPosition.y -0.5f)){Xhigher= true;}
				ScreenPosition.x = Mathf.Clamp(ScreenPosition.x, 0.1f, 0.9f);
				ScreenPosition.y = Mathf.Clamp(ScreenPosition.y, 0.15f, 0.85f);
				if(Xhigher)
				{
					if(ScreenPosition.x > 0.5f) {ScreenPosition.x = 0.9f;}
					if(ScreenPosition.x <= 0.5f){ScreenPosition.x = 0.1f;}
				}
				else
				{
					if(ScreenPosition.y > 0.5f) {ScreenPosition.y = 0.85f;}
					if(ScreenPosition.y <= 0.5f){ScreenPosition.y = 0.15f;}
				}

				// check which side we are on.
				bool SideTest = false;
				if(Vector3.Dot(thisLocationsCamera.transform.forward, thisLocationsCamera.transform.position - thisLocation.transform.position) <= 0)
				{
					SideTest = true;
				}

				// assign the screen position.
				ScreenPosition.x = SideTest ? ScreenPosition.x : 1 - ScreenPosition.x;
				ScreenPosition.y = SideTest ? ScreenPosition.y : 1 - ScreenPosition.y;

				// set this display objects position for rendering.
				thisTransform.position = ScreenPosition;
				
				// calculate the rotation for the direction arrow object.
				Vector2 direction = new Vector2(thisTransform.position.x - 0.5f, thisTransform.position.y - 0.5f);
				DirectionArrow.transform.position = thisTransform.position;
				DirectionArrow.RotationValue = Vector2.Angle(Vector2.up, direction.normalized);
				if (direction.x < 0){DirectionArrow.RotationValue *= -1.0f;} 

				// check to see if we are on screen or not.
				Hide(!OnScreen(ScreenPosition)); 
			}
			else if(thisLocation.DisplayStyle == Location.DisplayStyleEnum.MiniCompass)
			{
				// calculate the current position for this display object on screen.
				Vector3 Direction = thisLocation.transform.position - thisLocationsCamera.transform.position;
				Vector3 CameraRelativeDirection = thisLocationsCamera.transform.InverseTransformDirection(Direction);
				CameraRelativeDirection.z = 0;
				CameraRelativeDirection = CameraRelativeDirection.normalized / 2;
				ScreenPosition.x = (0.5f + CameraRelativeDirection.x * 0.085f);
				ScreenPosition.y = (0.5f + CameraRelativeDirection.y * 0.085f);

				// set this display objects position for rotation calculations.
				thisTransform.position = ScreenPosition;
				
				// calculate the rotation for the direction arrow object.
				Vector2 direction = new Vector2(thisTransform.position.x - 0.5f, thisTransform.position.y - 0.5f);
				DirectionArrow.transform.position = thisTransform.position;
				DirectionArrow.RotationValue = Vector2.Angle(Vector2.up, direction.normalized);
				if (direction.x < 0){DirectionArrow.RotationValue *= -1.0f;}  

				// set this display objects position for rendering.
				thisTransform.position = new Vector3(ScreenPosition.x - 0.4f, ScreenPosition.y - 0.35f, ScreenPosition.z);
			}
	    }

		// display or hide this objects components.
	    private void Hide(bool enable)
	    {
			if (DirectionArrow != null)
	        {
				DirectionArrow.gameObject.SetActive(enable);
	        }

			this.GetComponent<GUIText>().enabled = enable;
			this.GetComponent<GUITexture>().enabled = enable;
			Secondary_Text_Display.enabled = enable;
	    }
	}
}