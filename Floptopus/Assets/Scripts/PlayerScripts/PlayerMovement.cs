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
    Vector3 fallOrigin;
    Vector3 edge;
	float dashHeldTime;
    float tilt;
    float jumpStrength;
    bool jumpPressed;
    bool dashPressed;
    bool jumpReleased;

    bool jumping;
	bool grounded;
    bool onEdge = false;
    Edge edgeScript;

	float timeSinceJump;
	float stickiness;
    bool stuck;

    Player player;

    Animator anim;

	void Start ()
    {
        jumpReleased = true;
        direction = Vector3.zero;
        wallJumpDirection = Vector3.zero;
		dashHeldTime = 0.0f;
		jumpPressed = false;
        controller = GetComponent<CharacterController>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
        jumping = false;
		grounded = false;
        stuck = false;
		stickiness = 1;
	}

	void Update()
	{
        anim.SetFloat("speed", Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z));
        LookInDirection();
        //grounded = IsGrounded();
        grounded = controller.isGrounded;
	}

	void FixedUpdate ()
    {
        Vector3 directionV = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 directionH = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        direction = (directionV * Input.GetAxis("Vertical") + directionH * Input.GetAxis("Horizontal")) * speed;

        if (onEdge)
        {
            timeSinceJump = 0;
            grounded = false;
        }

        if (onEdge)
        {
            direction += new Vector3(edge.x - transform.position.x, 0, edge.z - transform.position.z) ;
        }

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

		if (!Input.GetButton ("Shift") && dashPressed && (grounded || onEdge)) //dash-button released
			Dash (); 

		if (Input.GetButton ("Jump") && (grounded || stuck) && jumpReleased)
			Jump ();

        if (jumpPressed && !Input.GetButton("Jump"))
            jumpReleased = true;

		direction = new Vector3 (direction.x, direction.y * stickiness, direction.z);

        Vector3 oldVelocity = controller.velocity;
		controller.Move(new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime,  direction.z * Time.deltaTime));

        if (controller.velocity.y < 0 && oldVelocity.y >= 0)
            fallOrigin = transform.position;
        if (stuck)
            fallOrigin = transform.position;
        if (controller.velocity.y == 0 && oldVelocity.y < 0)
        {
            if (Mathf.Abs(fallOrigin.y - transform.position.y) > 20)
                player.health.TakeDamage(50);
        }
                
    }

    void LookInDirection()
    {
	    Vector3 view = new Vector3(direction.x, 0, direction.z);
        if (!stuck)
        {
            if (!grounded && !stuck && !onEdge)
                view.y = controller.velocity.y;
            if (view != Vector3.zero && !stuck)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
            if (!jumping && grounded)
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
        }

        if (stuck || onEdge)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(-wallJumpDirection), Time.deltaTime * 10);
        }
    }

	void Jump()
	{
        jumpPressed = true;
        jumpReleased = false;
		jumpDirection = Vector3.up * 25;
        if (stuck)
            jumpDirection = wallJumpDirection * 20 + Vector3.up * 20;
        if(onEdge)
            jumpDirection = Vector3.up * 25 - wallJumpDirection * 5;
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
        if (onEdge)
        {
            jumpDirection = Vector3.zero;
            edgeScript.Release();
        }
        dashPressed = false;
		dashHeldTime = 0.0f;
		timeSinceJump = 0.0f;
        jumping = true;
	}

	public void StickToSurface(bool stick)
	{
        stuck = stick;
		if (stick)
			stickiness = 0.05f;
		else
			stickiness = 1.0f;
	}

    public void HoldOntoEdge(bool hold, Vector3 edge, Edge edgeScript)
    {
        this.edgeScript = edgeScript;
        this.edge = edge;
        if (!onEdge && hold)
        {
            direction = Vector3.zero;
            jumpDirection = Vector3.zero;
            stickiness = 1.0f;
        }
        if(!hold && stuck)
            stickiness = 0.1f;
        onEdge = hold;
    }

    public bool IsJumping() { return jumping;}

    public void SetWallJumpDirection(Vector3 direction) { wallJumpDirection = direction; }
}
