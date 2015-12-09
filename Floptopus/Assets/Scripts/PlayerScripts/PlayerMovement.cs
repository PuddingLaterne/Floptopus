using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

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
    bool falling = false;
    float fallingTime = 0.0f;
    bool falltriggered;

    Animator anim;
    PlayerHealth health;

    void Awake()
    {
        instance = this;
    }

	void Start ()
    {
        jumpReleased = true;
        direction = Vector3.zero;
        wallJumpDirection = Vector3.zero;
		dashHeldTime = 0.0f;
		jumpPressed = false;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        health = PlayerHealth.instance;

        jumping = false;
		grounded = false;
        stuck = false;
		stickiness = 1;
	}

	void Update()
	{
        anim.SetFloat("speed", Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z));
        anim.SetBool("jumping", jumping);
        LookInDirection();
        grounded = controller.isGrounded;
        if (grounded)
            anim.SetTrigger("grounded");
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
        {
            falling = true;
            fallOrigin = transform.position;
        }

        if (controller.velocity.y == 0 || grounded)
        {
            falltriggered = false;
            falling = false;
            fallingTime = 0.0f;
        }

        if (falling)
            fallingTime =+ Time.deltaTime;
        if (fallingTime >= 0.01f && !falltriggered && !(stuck || onEdge))
        {
            falltriggered = true;
            anim.SetTrigger("fall");
        }

        if (stuck)
            fallOrigin = transform.position;
        if (controller.velocity.y == 0 && oldVelocity.y < 0)
        {
            if (Mathf.Abs(fallOrigin.y - transform.position.y) > 20)
            {
                anim.SetTrigger("crash");
                health.TakeDamage(50);
            }
            else
            {
                if(fallingTime >= 0.01f)
                    anim.SetTrigger("bounce");
            }
        }
        anim.SetBool("falling", falling);
                
    }

    void LookInDirection()
    {
	    Vector3 view = new Vector3(direction.x, 0, direction.z);
        if (!stuck)
        {
            if (view != Vector3.zero && !stuck)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
            if (!jumping && grounded)
                transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);
        }

        //if (stuck || onEdge)
        //{
            //Vector3 wallDirection = new Vector3(-wallJumpDirection.x, wallJumpDirection.y, -wallJumpDirection.z);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(wallDirection), Time.deltaTime * 10);
        //}
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
        anim.SetTrigger("jump");
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
        anim.SetTrigger("dash");
	}

	public void StickToSurface(bool stick)
	{
        if (stuck != stick)
        {
            anim.SetBool("sticking", stick);
            if (stick)
                anim.SetTrigger("stick");
        }
        stuck = stick;
		if (stick)
			stickiness = 0.05f;
		else
			stickiness = 1.0f;
	}

    public void HoldOntoEdge(bool hold, Vector3 edge, Edge edgeScript)
    {
        if (onEdge != hold)
        {
            anim.SetBool("hanging", hold);
            if (hold)
                anim.SetTrigger("hang");
            anim.ResetTrigger("stick");
        }

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

    public void SetGrounded(bool grounded)
    {
        this.grounded = grounded;
    }
}
