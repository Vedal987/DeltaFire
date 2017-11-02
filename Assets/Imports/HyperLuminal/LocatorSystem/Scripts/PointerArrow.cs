using UnityEngine;
using HyperLuminalGames;

namespace HyperLuminalGames
{
	/// <summary>
	/// Pointer Arrow Script.
	/// Part of the Off Screen Display Object this screen renders the rotated pointing arrow that indicats location direction.
	/// </summary>
	public class PointerArrow : MonoBehaviour
	{
		// display options for this arrow.
		[HideInInspector]
		public Texture ArrowTexture = null;
		[HideInInspector]
		public Color ArrowColor = Color.white;

		// rotation around the display icon (set by off screen display script).
		[HideInInspector]
		public float RotationValue = 0;

		// camera for this location used to calculate this objects position on screen.
		[HideInInspector]
	    public Camera thisCamera;

		// rendering positioning.
	    private Vector2 ScreenPivot;
		private Rect DisplayArea;

	    void Update()
	    {
			// find the screen position for this object.
			ScreenPivot = new Vector2((thisCamera.pixelWidth * transform.position.x) + ((thisCamera.pixelRect.x / thisCamera.pixelWidth) *  thisCamera.pixelWidth),
			                          (Screen.height - (thisCamera.pixelHeight * transform.position.y)) - ((thisCamera.pixelRect.y / thisCamera.pixelHeight) *  thisCamera.pixelHeight));

			// display area magic numbers explained (20 = width,                    35 = height, 55 = offset from center point).
			DisplayArea = new Rect((ScreenPivot.x - (20 * 0.5f)) , ScreenPivot.y - (35 * 0.5f) - 40, 20, 35);
	    }

	    void OnGUI()
	    {
			// render the object using Unity GUI.
			GUIUtility.RotateAroundPivot(RotationValue, ScreenPivot);
			GUI.color = ArrowColor;
			GUI.DrawTexture(DisplayArea, ArrowTexture);
	    }
	}
}