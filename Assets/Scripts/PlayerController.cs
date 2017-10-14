using UnityEngine;
using System.Collections;

[AddComponentMenu("FPS/Player Controller")]
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
        /**
        *  Script written by Kronus.
        **/

	public enum STATE { STANDED, CROUCHED }

	public float baseSpeed;
	public float crouchSpeedMultiplier;
	public float sprintSpeedMultiplier;
	public float fallingDamageThreshold;
	public float standedHeight;
	public float crouchedHeight;
	public float stateTrasitionSpeed;
	public float maxVelocityChange;
	public float jumpHeight;
	public float customGravity;
	public float inAirControl;
	public Transform head;

	private float hor, ver;
	private float inputDiagLimiter;
	private float sprintSpeed;
	private float crouchSpeed;
	private float speed;
	private float height;
	private float fallStartLevel;
	private bool grounded;
	private bool sliding;
	private bool falling;
	private STATE state;
	private RaycastHit hit;
	private Vector3 targetVelocity;
	private Rigidbody body;
	private CapsuleCollider col;

	public bool IS_STANDED {
		get {
			return (state == STATE.STANDED);
		}
	}
	
	public bool IS_CROUCHED {
		get {
			return (state == STATE.CROUCHED);
		}
	}
	
	public bool IS_SPRINTING {
		get {
			return (Input.GetKey (KeyCode.LeftShift) && ver > 0 && IS_STANDED && !falling && !sliding);
		}
	}
	
	float CalculateJumpVerticalSpeed() {
		return Mathf.Sqrt(2 * jumpHeight * customGravity);
	}


	void Awake()
	{
		col = GetComponent<CapsuleCollider>();
		body = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		hor = Input.GetAxis("Horizontal");
		ver = Input.GetAxis("Vertical");
	    inputDiagLimiter = (hor != 0 && ver != 0) ? .7071f : 1.0f;
		
		if (grounded)
		{
			if (falling) 
			{
				falling = false;
				if (transform.position.y < fallStartLevel - fallingDamageThreshold) {
					print ("Ouch! Fell " + (fallStartLevel - transform.position.y) + " units!");
				}
			}
			
			if (IS_STANDED)
			{
				if (Physics.Raycast(transform.position, -Vector3.up, out hit, (col.height * .5f + col.radius))) {
					sliding = (Vector3.Angle(hit.normal, Vector3.up) > 30);
				}
				
				if(sliding) {
					body.AddRelativeForce(-Vector3.up * 500);
				}
			}


			targetVelocity = new Vector3(hor * inputDiagLimiter, 0, ver * inputDiagLimiter);
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;

			Vector3 velocity = body.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);

			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;

			body.AddForce(velocityChange, ForceMode.VelocityChange);

			if (Input.GetKeyDown(KeyCode.Space) && IS_STANDED) {
				body.velocity = new Vector3(body.velocity.x, CalculateJumpVerticalSpeed(), body.velocity.z);
			}
		}
		else
		{
			if (!falling) 
			{
				falling = true;
				fallStartLevel = transform.position.y;
			}

			targetVelocity = new Vector3(hor, 0, ver);
			targetVelocity = transform.TransformDirection(targetVelocity) * inAirControl;
			body.AddForce(targetVelocity, ForceMode.VelocityChange);
		}

		body.AddForce(new Vector3(0, (-customGravity * body.mass), 0));
		grounded = false;
	}
	
	void OnCollisionStay(Collision col)
	{
		foreach (ContactPoint contact in col.contacts)
		{
			if (Vector3.Angle(contact.normal, Vector3.up) < 45)
			{
				grounded = true;
			}
		}
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			state = (IS_STANDED) ? STATE.CROUCHED : STATE.STANDED;
		}
		
		sprintSpeed = (baseSpeed * sprintSpeedMultiplier);
		crouchSpeed = (baseSpeed * crouchSpeedMultiplier);
		speed  = (IS_STANDED) ? ((IS_SPRINTING) ? sprintSpeed : baseSpeed) : crouchSpeed;
		height = (IS_STANDED) ? standedHeight : crouchedHeight;
		head.localPosition = Vector3.Lerp (head.localPosition, new Vector3(0, (height - .2f), 0), Time.deltaTime * stateTrasitionSpeed);
		col.height = Mathf.Lerp (col.height, height, Time.deltaTime * stateTrasitionSpeed);
		col.center = Vector3.Lerp (col.center, new Vector3(0, (height / 2), 0), Time.deltaTime * stateTrasitionSpeed);
	}
}
