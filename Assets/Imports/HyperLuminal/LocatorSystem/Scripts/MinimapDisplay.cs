using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HyperLuminalGames;

public class MinimapDisplay : MonoBehaviour 
{
	// different distance and position calculation modes for 2D and 3D game worlds.
	// the display modes 2D and 3D are only suggestions please use the axis as the main factor.
	public enum DisplayModeEnum 
	{
		XY_Axis_For_2D = 0,
		XZ_Axis_For_3D = 1
	}
	public DisplayModeEnum DisplayMode = DisplayModeEnum.XZ_Axis_For_3D;

	// minimap position.
	public Vector2 Minimap_Position = new Vector2(10.0f, 475.0f);

	// minimap scale.
	public float Minimap_Scale = 150.0f;
	private float InitialYScale = 0.0f;

	// Distance scale.
	public float Distance_Scale = 1.0f;

	// the minimap background display.
	public GUITexture Minimap_Background;

	// the minimap background display.
	public GUITexture Minimap_Trim;

	// the minimap background texture.
	public Texture2D Minimap_Background_Texture;

	// the minimap background texture.
	public Texture2D Minimap_Trim_Texture;

	// dots marked on the minimap that represent locations.
	public GUITexture Minimap_Location;

	// minimap player display icon.
	public GUITexture Player_Display_Icon;

	// if we want to show locations that are out of scope on the edge of the map.
	public bool Show_OffScreen_On_Edge = true;
	
	// this minimap render layer for splitscreen.
	[HideInInspector]
	public string Minimap_Layer = "";

	// list of all the tracked locations on this minimap.
	[HideInInspector]
	public List<Location> Tracked_Locations = new List<Location>();

	// list of all the marker textures on the minimap.
	[HideInInspector]
	public List<GUITexture> Minimap_Markers = new List<GUITexture>();

	// the camera tracking these location.
	[HideInInspector]
	public Camera Tracking_Camera;

	// new position due to inverted coordinate system.
	private float ScaledYPosition = 0.0f;

	// Use this for initialization
	void Start () 
	{
		// store the initial Y scale for later calculations.
		InitialYScale = Minimap_Scale;

		// position the minimap for splitscreen displays.
		Minimap_Position = new Vector2(Minimap_Position.x, 
		(Minimap_Position.y * Tracking_Camera.rect.height) - (Minimap_Scale * (1.0f - Tracking_Camera.rect.height)));

		// rename this object to clean up.
		this.gameObject.layer = LayerMask.NameToLayer(Minimap_Layer);
		Minimap_Background.gameObject.layer = LayerMask.NameToLayer(Minimap_Layer);
		Minimap_Trim.gameObject.layer = LayerMask.NameToLayer(Minimap_Layer);
		Player_Display_Icon.gameObject.layer = LayerMask.NameToLayer(Minimap_Layer);
		this.name = "Minimap_Display";

	}
	
	// Update is called once per frame
	void Update () 
	{
		// calculate the new Y position to retain the offset during map scaling.
		ScaledYPosition = Minimap_Position.y - (Minimap_Scale - InitialYScale);

		// set the new pixel inset position and scale values.
		Minimap_Background.pixelInset = new Rect(Minimap_Position.x, ScaledYPosition , Minimap_Scale, Minimap_Scale);
		Minimap_Trim.pixelInset       = new Rect(Minimap_Position.x, ScaledYPosition , Minimap_Scale, Minimap_Scale);

		// iterate over the locations.
		for( int i = 0; i < Tracked_Locations.Count; i++)
		{
			// check we have a marker for this location.
			if(i < Minimap_Markers.Count)
			{
				// use the selected display calculation mode.
				if(DisplayMode == DisplayModeEnum.XZ_Axis_For_3D)
				{
					// colour the marker correctly.
					Minimap_Markers[i].color = Tracked_Locations[i].IconColour;

					// set the markers display shape correctly.
					Minimap_Markers[i].texture = Tracked_Locations[i].Minimap_Display_Icon;

					// position the new marker.
					Position_Minimap_Markers_XZ(Tracked_Locations[i], Minimap_Markers[i]);
				}
				else if(DisplayMode == DisplayModeEnum.XY_Axis_For_2D)
				{
					// colour the marker correctly.
					Minimap_Markers[i].color = Tracked_Locations[i].IconColour;

					// set the markers display shape correctly.
					Minimap_Markers[i].texture = Tracked_Locations[i].Minimap_Display_Icon;

					// position the new marker.
					Position_Minimap_Markers_XY(Tracked_Locations[i], Minimap_Markers[i]);
				}
			}
			else
			{
				// we don't have a marker so make one then position it.
				Minimap_Markers.Add(Instantiate(Minimap_Location, this.transform.position, Quaternion.identity) as GUITexture);

				// parent these markers under the minimap object to clean up.
				Minimap_Markers[i].name = "Minimap_Marker";
				Minimap_Markers[i].gameObject.layer = LayerMask.NameToLayer(Minimap_Layer);
				Minimap_Markers[i].transform.parent = this.transform;

				// use the selected display calculation mode.
				if(DisplayMode == DisplayModeEnum.XZ_Axis_For_3D)
				{
					// colour the marker correctly.
					Minimap_Markers[i].color = Tracked_Locations[i].IconColour;

					// set the markers display shape correctly.
					Minimap_Markers[i].texture = Tracked_Locations[i].Minimap_Display_Icon;

					// position the new marker.
					Position_Minimap_Markers_XZ(Tracked_Locations[i], Minimap_Markers[i]);
				}
				else if(DisplayMode == DisplayModeEnum.XY_Axis_For_2D)
				{
					// colour the marker correctly.
					Minimap_Markers[i].color = Tracked_Locations[i].IconColour;

					// set the markers display shape correctly.
					Minimap_Markers[i].texture = Tracked_Locations[i].Minimap_Display_Icon;

					// position the new marker.
					Position_Minimap_Markers_XY(Tracked_Locations[i], Minimap_Markers[i]);
				}
			}
		}

		// update the player position/rotation.
		Update_Rotation_Position();
	}

	public void Update_Rotation_Position()
	{
		// calculate the 2D map center position using the bottom left and taking away the width of the marker (10.0f / 2.0f = 5.0f).
		Vector2 Center2D = new Vector2(Minimap_Position.x, ScaledYPosition) + new Vector2((Minimap_Scale/2.0f) - 5.0f, (Minimap_Scale/2.0f) - 5.0f); 

		// set the player position.
		Player_Display_Icon.pixelInset = new Rect(Center2D.x, Center2D.y, 10.0f, 15.0f);
	}


	public void Position_Minimap_Markers_XY(Location thisLocation, GUITexture thisMiniMapMarker)
	{
		// calculate the distance from this location to the camera.
		float Distance2D = Mathf.Abs(Vector2.Distance(new Vector2(Tracking_Camera.transform.position.x, Tracking_Camera.transform.position.y),
		                                              new Vector2(thisLocation.transform.position.x, thisLocation.transform.position.y )));

		// if distance is zero we are directly ontop of the location. 
		// calculate the direction between the camera and the location, normalised it.
		Vector3 Direction3D = Tracking_Camera.transform.position - thisLocation.transform.position;

		// the vector perpendicular to referenceForward, 90 degrees clockwise, is used to determine if angle is positive or negative.
		Vector3 Right3D = Vector3.Cross(Vector3.up, Tracking_Camera.transform.forward);
	
		// get the angle in degrees between 0 and 180
		float Angle = Vector2.Angle(new Vector2(Direction3D.x, Direction3D.y), new Vector2(-Tracking_Camera.transform.forward.x,-Tracking_Camera.transform.forward.z));
		
		// determine if the degree value should be negative. 
		// a positive value from the dot product means that our vector is on the right of the reference vector.
		float Sign_Value = Mathf.Sign(Vector3.Dot(Direction3D, Right3D));

		// use the sign to get the final angle value.
		float Final_Angle = Sign_Value * Angle;

		// calculate the 2D map center position using the bottom left and taking away the width of the marker (10.0f / 2.0f = 5.0f).
		Vector2 Center2D = new Vector2(Minimap_Position.x, ScaledYPosition) + new Vector2((Minimap_Scale/2.0f) - 5.0f, (Minimap_Scale/2.0f) - 5.0f); 

		// calculate the new 2D position using the center, 3D direction and 3D distance.
		Vector2 Position2D = Center2D + new Vector2(Direction3D.normalized.x * Distance2D * Distance_Scale, Direction3D.normalized.y * Distance2D * Distance_Scale);

		// calculate the total distance using the distance scale variable.
		float Final_Distance = Distance2D * Distance_Scale;

		if(Show_OffScreen_On_Edge)
		{
			// limit the distance if required to show just off screen markers.
			if((Distance2D * Distance_Scale) >  Minimap_Scale * 0.425f)
			{
				// limit the scale. 
				Final_Distance = Minimap_Scale * 0.45f;
			}
		}

		// calculate the new direction using the 2D Angle.
		Vector3 RotatedPosition = new Vector2(Position2D.x = Center2D.x + Final_Distance * Mathf.Cos(Mathf.Deg2Rad * (Final_Angle + 90.0f)),
		                                      Position2D.y = Center2D.y + Final_Distance * Mathf.Sin(Mathf.Deg2Rad * (Final_Angle + 90.0f)));

		// slightly scale the icons based on their distance for a nice fade out to the edge.
		float Final_Scale = 10.0f * Mathf.Clamp((1.5f - (Final_Distance/(Minimap_Scale * 0.5f))), 0.25f, 1.0f);

		// display the markers differently depending on the options we select.
		if(Show_OffScreen_On_Edge)
		{
			// position this minimap marker using the locations specification.
			thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, Final_Scale, Final_Scale);
		}
		else
		{
			// check that the position is actually still on the display (if not then do not show it).
			if((Distance2D * Distance_Scale) < Minimap_Scale * 0.475f)
			{
				// position this minimap marker using the locations specification.
				thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, Final_Scale, Final_Scale);
			}
			else
			{
				thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, 0.0f, 0.0f);
			}
		}
	}

	public void Position_Minimap_Markers_XZ(Location thisLocation, GUITexture thisMiniMapMarker)
	{
		// calculate the distance from this location to the camera.
		float Distance2D = Mathf.Abs(Vector2.Distance(new Vector2(Tracking_Camera.transform.position.x, Tracking_Camera.transform.position.z),
		                                              new Vector2(thisLocation.transform.position.x, thisLocation.transform.position.z )));
		
		// if distance is zero we are directly ontop of the location. 
		// calculate the direction between the camera and the location, normalised it.
		Vector3 Direction3D = Tracking_Camera.transform.position - thisLocation.transform.position;
		
		// the vector perpendicular to referenceForward, 90 degrees clockwise, is used to determine if angle is positive or negative.
		Vector3 Right3D = Vector3.Cross(Vector3.up, Tracking_Camera.transform.forward);
		
		// get the angle in degrees between 0 and 180
		float Angle = Vector2.Angle(new Vector2(Direction3D.x, Direction3D.z), new Vector2(-Tracking_Camera.transform.forward.x,-Tracking_Camera.transform.forward.z));
		
		// determine if the degree value should be negative. 
		// a positive value from the dot product means that our vector is on the right of the reference vector.
		float Sign_Value = Mathf.Sign(Vector3.Dot(Direction3D, Right3D));
		
		// use the sign to get the final angle value.
		float Final_Angle = Sign_Value * Angle;
		
		// calculate the 2D map center position using the bottom left and taking away the width of the marker (10.0f / 2.0f = 5.0f).
		Vector2 Center2D = new Vector2(Minimap_Position.x, ScaledYPosition) + new Vector2((Minimap_Scale/2.0f) - 5.0f, (Minimap_Scale/2.0f) - 5.0f); 
		
		// calculate the new 2D position using the center, 3D direction and 3D distance.
		Vector2 Position2D = Center2D + new Vector2(Direction3D.normalized.x * Distance2D * Distance_Scale, Direction3D.normalized.z * Distance2D * Distance_Scale);
		
		// calculate the total distance using the distance scale variable.
		float Final_Distance = Distance2D * Distance_Scale;
		
		if(Show_OffScreen_On_Edge)
		{
			// limit the distance if required to show just off screen markers.
			if((Distance2D * Distance_Scale) >  Minimap_Scale * 0.425f)
			{
				// limit the scale. 
				Final_Distance = Minimap_Scale * 0.45f;
			}
		}
		
		// calculate the new direction using the 2D Angle.
		Vector3 RotatedPosition = new Vector2(Position2D.x = Center2D.x + Final_Distance * Mathf.Cos(Mathf.Deg2Rad * (Final_Angle + 90.0f)),
		                                      Position2D.y = Center2D.y + Final_Distance * Mathf.Sin(Mathf.Deg2Rad * (Final_Angle + 90.0f)));
		
		// slightly scale the icons based on their distance for a nice fade out to the edge.
		float Final_Scale = 10.0f * Mathf.Clamp((1.5f - (Final_Distance/(Minimap_Scale * 0.5f))), 0.25f, 1.0f);
		
		// display the markers differently depending on the options we select.
		if(Show_OffScreen_On_Edge)
		{
			// position this minimap marker using the locations specification.
			thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, Final_Scale, Final_Scale);
		}
		else
		{
			// check that the position is actually still on the display (if not then do not show it).
			if((Distance2D * Distance_Scale) < Minimap_Scale * 0.475f)
			{
				// position this minimap marker using the locations specification.
				thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, Final_Scale, Final_Scale);
			}
			else
			{
				thisMiniMapMarker.pixelInset = new Rect(Position2D.x, Position2D.y, 0.0f, 0.0f);
			}
		}
	}
}

