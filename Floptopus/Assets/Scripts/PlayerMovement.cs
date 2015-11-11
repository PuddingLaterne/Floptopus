using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
	public float jumpTime = 1.0f;
    public float jumpHoldThreshold = 0.5f;
	public float gravity = 10;

	Vector3 viewDirection;
    Vector3 direction;
	Vector3 jumpDirection;
    Vector3 wallJumpDirection;
    CharacterController controller;
	float dashHeldTime;
    float tilt;
    float jumpStrength;
    bool jumpPressed;
    bool dashPressed;
    bool jumpReleased;
    bool jumping;
	bool grounded;

	float timeSinceJump;
	float stickiness;
    bool stuck;

	// Use this for initialization
	void Start ()
    {
        jumpReleased = true;
        direction = Vector3.zero;
        wallJumpDirection = Vector3.zero;
		dashHeldTime = 0.0f;
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

        if (Input.GetButton("Shift"))
        {
            dashPressed = true;
            dashHeldTime += Time.deltaTime;

        }
		if (!Input.GetButton ("Shift") && dashPressed && grounded) //dash-button released
			Dash (); 

		if (Input.GetButton ("Jump") && (grounded || stuck) && jumpReleased)
			Jump ();

        if (jumpPressed && !Input.GetButton("Jump"))
            jumpReleased = true;

		direction = new Vector3 (direction.x, direction.y * stickiness, direction.z);
		controller.Move(new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime,  direction.z * Time.deltaTime));
		
		LookInDirection();
    }

    void LookInDirection()
    {
	    Vector3 view = new Vector3(direction.x, 0, direction.z);
        if (!grounded && !stuck)
            view.y = controller.velocity.y;
        if (view != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
        if (!jumping && (!stuck || grounded))
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
        if(stuck && !grounded)
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, -180, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
    }

	void Jump()
	{
        jumpPressed = true;
        jumpReleased = false;
		jumpDirection = Vector3.up * 25;
        if (stuck)
            jumpDirection = wallJumpDirection * 20 + Vector3.up * 20;
		timeSinceJump = 0.0f;
		jumping = true;
	}

	void Dash()
	{
        if(dashHeldTime >= jumpHoldThreshold)
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
        dashPressed = false;
		dashHeldTime = 0.0f;
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

    public void SetWallJumpDirection(Vector3 direction)
    {
        wallJumpDirection = direction;
    }
}
