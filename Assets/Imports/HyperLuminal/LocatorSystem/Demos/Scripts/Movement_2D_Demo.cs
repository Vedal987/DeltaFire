using UnityEngine;
using System.Collections;

public class Movement_2D_Demo : MonoBehaviour 
{
	/// <summary>
	/// Player movement speed
	/// </summary>
	private float movementSpeed = 400.0f;

	// Update is called once per frame
	void Update () 
	{
		// check for player exiting the game
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		
		// get the input this frame
		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");

		// reset the velocity each frame
		GetComponent<Rigidbody2D>().velocity =	new Vector2(0, 0);
		
		// horizontal movement, left or right, set animation type and speed 
		if (horizontal > 0)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(movementSpeed * Time.deltaTime, 0);
		}
		else if (horizontal < 0)
		{
			GetComponent<Rigidbody2D>().velocity =	new Vector2(-movementSpeed * Time.deltaTime, 0);
		}
		
		// vertical movement, up or down, set animation type and speed 
		if (vertical > 0)
		{
			GetComponent<Rigidbody2D>().velocity =	new Vector2(GetComponent<Rigidbody2D>().velocity.x, movementSpeed * Time.deltaTime);
		}
		else if (vertical < 0)
		{
			GetComponent<Rigidbody2D>().velocity =	new Vector2(GetComponent<Rigidbody2D>().velocity.x, -movementSpeed * Time.deltaTime);
		}
	}
}
