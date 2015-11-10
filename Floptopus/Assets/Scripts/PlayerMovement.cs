using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
	public float jumpTime = 1.0f;
    public float jumpHoldThreshold = 0.5f;
	public float gravity = 10;

	private Vector3 viewDirection;
    private Vector3 direction;
	private Vector3 jumpDirection;
    private CharacterController controller;
	private float jumpHeldTime;
    private float tilt;
    private float jumpStrength;
    private bool jumpPressed;
    private bool jumping;
	private bool grounded;
	private float timeSinceJump;
	private float stickiness;
    private bool stuck;

	// Use this for initialization
	void Start ()
    {
        direction = Vector3.zero;
		jumpHeldTime = 0.0f;
		jumpPressed = false;
        controller = GetComponent<CharacterController>();
        jumping = false;
		grounded = false;
        stuck = false;
		stickiness = 1;
	}

	void Update()
	{
		grounded = controller.isGrounded;
	}

	void FixedUpdate ()
    {
		direction = (Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal")) * speed;
		direction.y = 0;

		float gravityInfluence = timeSinceJump;
		if(timeSinceJump > 1.0f)
			gravityInfluence = 1.0f;
		direction -= Vector3.up * gravity * gravityInfluence * Time.deltaTime;

		if (grounded && timeSinceJump > 0.1f) 
		{
			jumping = false;
			timeSinceJump = 0;
		}

		timeSinceJump += Time.deltaTime;

		if (jumping) 
			direction += jumpDirection * Time.deltaTime * 100;

        if (Input.GetButton("Jump") && Input.GetButton("Shift"))
			jumpPressed = true;
        if (jumpPressed)
            jumpHeldTime += Time.deltaTime;
		if (!Input.GetButton ("Jump") && jumpPressed && grounded) // Jump-Button released;
			Dash ();

		if (Input.GetButton ("Jump") && !Input.GetButton ("Shift") && (grounded || stuck) && !jumpPressed)
			Jump ();

		direction = new Vector3 (direction.x, direction.y * stickiness, direction.z);
		controller.Move(new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime,  direction.z * Time.deltaTime));
		
		LookInDirection();
    }

    void LookInDirection()
    {

	    Vector3 view = new Vector3(direction.x, 0, direction.z);
		if (!grounded)
			view.y = controller.velocity.y;
		if(view != Vector3.zero)
        	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
		if(!jumping)
          transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);

    }

	void Jump()
	{
		jumpDirection = Vector3.up * 25;
        if (stuck)
            jumpDirection = transform.forward * -30;
		timeSinceJump = 0.0f;
		jumping = true;
	}

	void Dash()
	{
        if(jumpHeldTime >= jumpHoldThreshold)
        {
            tilt = 2;
            jumpStrength = 30;
        }
        else
        {
            tilt = 0.75f;
            jumpStrength = 20;
        }
        jumpDirection = transform.forward + Vector3.up * tilt;
        jumpDirection.Normalize();
        jumpDirection = jumpDirection * jumpStrength;
        jumpPressed = false;
		jumpHeldTime = 0.0f;
		timeSinceJump = 0.0f;
        jumping = true;
	}

	public void StickToSurface(bool stick)
	{
        stuck = stick;
		if (stick)
			stickiness = 0.1f;
		else
			stickiness = 1.0f;
	}

    public bool IsJumping() { return jumping;}
}
