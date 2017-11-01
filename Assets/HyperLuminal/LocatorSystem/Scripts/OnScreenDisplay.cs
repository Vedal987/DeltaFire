using UnityEngine;
using System.Collections;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// On Screen Display Script.
	/// Inherits from Location Tracking Display Class.
	/// </summary>
	public class OnScreenDisplay : LocationTrackingDisplay
	{
		// secondary text display
		public GUIText Secondary_Text_Display;

		void Start()
		{
			// update this locations display information.
			UpdateDisplayInfo();
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
		}

	    protected void Update()
	    {
			if((thisLocation.DisplayOnScreen == false)||(thisLocation.DisplayStyle == Location.DisplayStyleEnum.MiniCompass))
			{
				// if we should not display on screen then hide us.
				Hide(!true);
				return;
			}

			// update any display option information for this location.
			UpdateDisplayInfo();


			// get the current screen position for this location.
			Vector3 ScreenPosition = GetScreenPosition();

			// calculate the current position for this display object on screen.
				// offset the marker by its current screen positioning.
			Vector2 newPosition = new Vector2(ScreenPosition.x - thisLocationsCamera.pixelRect.x , ScreenPosition.y - thisLocationsCamera.pixelRect.y);
			newPosition.x -= thisLocationsCamera.rect.x * 2.0f;
			newPosition.y -= thisLocationsCamera.rect.y * 2.0f;

			// set this display objects position for rendering.
			thisTransform.localPosition = new Vector3(newPosition.x, newPosition.y, ScreenPosition.z);

			// check to see if we are on screen or not.
			Hide(OnScreen(ScreenPosition));
	    }

		// display or hide this objects components.
		private void Hide(bool enable)
		{
			this.GetComponent<GUIText>().enabled = enable;
			this.GetComponent<GUITexture>().enabled = enable;
			Secondary_Text_Display.enabled = enable;
		}
	}
}