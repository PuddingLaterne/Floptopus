using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float dashSpeedMultiplier = 1.5f;
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
        LookInDirection();
        grounded = IsGrounded();
        //IsGrounded();
		//grounded = controller.isGrounded;
	}

	void FixedUpdate ()
    {
        Vector3 directionV = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 directionH = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        direction = (directionV * Input.GetAxis("Vertical") + directionH * Input.GetAxis("Horizontal")) * speed;
        if (dashPressed)
            direction = direction * dashSpeedMultiplier;


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
    }

    void LookInDirection()
    {
	    Vector3 view = new Vector3(direction.x, 0, direction.z);
        if (!stuck)
        {
            if (!grounded)
                view.y = controller.velocity.y;
            if (view != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
            if (!jumping && grounded)
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
        }
        if (stuck && !grounded)
            transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, -90, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
    }

	void Jump()
	{
        jumpPressed = true;
        jumpReleased = false;
		jumpDirection = Vector3.up * 25;
        if (stuck && !grounded)
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
            jumpStrength = 25;
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("StickySurface"))
        {
            wallJumpDirection = hit.normal;
        }
        if (hit.collider.gameObject.CompareTag("Collectable"))
        {
            hit.gameObject.SetActive(false);
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + controller.center, - Vector3.up);
        if(Physics.Raycast(ray, out hit, 3.0f))
            return true;
        else
            return false;
    }

    public bool IsJumping() { return jumping;}
}
