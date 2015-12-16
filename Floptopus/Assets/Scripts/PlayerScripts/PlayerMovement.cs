using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;
    public GameObject inkObject;
    ParticleSystem ink;
    public float fallHeight = 30;

    public float speed = 5;
    public float dashSpeedMultiplier = 1.5f;
	public float jumpTime = 1.0f;
	public float gravity = 10;

	Vector3 viewDirection;
    Vector3 direction;
	Vector3 jumpDirection;
    Vector3 wallJumpDirection = Vector3.zero;
    CharacterController controller;
    Vector3 fallOrigin;
    Vector3 edge;
    Vector3 groundNormal;

    bool jumpPressed;
    bool dashPressed;
    bool jumpReleased;
    

    bool jumping;
	bool grounded;
    float airTime;
    float groundTime;
    bool onEdge = false;
    Edge edgeScript;


	float timeSinceJump;
	float stickiness;
    bool stuck;
    bool falling = false;
    bool dashing = false;
    float fallingTime = 0.0f;
    bool falltriggered;

    Animator anim;
    PlayerHealth health;
    PlayerInk playerInk;
    PlayerSound sound;

    void Awake()
    {
        instance = this;
    }

	void Start ()
    {
        ink = inkObject.GetComponent<ParticleSystem>();
        ink.enableEmission = false;
        jumpReleased = true;
        direction = Vector3.zero;
        wallJumpDirection = Vector3.zero;
		jumpPressed = false;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        health = PlayerHealth.instance;
        playerInk = PlayerInk.instance;
        sound = PlayerSound.instance;

        jumping = false;
		grounded = false;
        stuck = false;
		stickiness = 1;
	}

	void Update()
	{
        
        anim.SetFloat("speed", Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z));
        if (Mathf.Abs(controller.velocity.x) + Mathf.Abs(controller.velocity.z) != 0 && grounded)
        {
            sound.Move(true);
        }
        else
        {
            sound.Move(false);
        }
        anim.SetBool("jumping", jumping);
        LookInDirection();
        grounded = IsGrounded();

	}

	void FixedUpdate ()
    {
        Vector3 directionV = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
        Vector3 directionH = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z).normalized;
        direction = (directionV * Input.GetAxis("Vertical") + directionH * Input.GetAxis("Horizontal")) * speed;

        HangingOnEdge();
        Gravity();
        HandleJump();
        HandleInput();     
        
		direction = new Vector3 (direction.x, direction.y * stickiness, direction.z);

        Vector3 oldVelocity = controller.velocity;
		controller.Move(new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime,  direction.z * Time.deltaTime));
        Fall(oldVelocity);              
    }

    void HandleInput()
    {
        if (Input.GetButton("Shift"))
        {
            dashPressed = true;
            if (onEdge)
            {
                ReleaseEdge();
            }
            if (Input.GetButton("Jump") && !jumping && playerInk.UseInk(20))
            {
                Dash();
            }

            if (!jumping && direction.x != 0 && direction.y != 0)
            {
                if (playerInk.UseInk(10 * Time.deltaTime))
                {
                    Run();
                }
                else
                {
                    ink.enableEmission = false;
                }
            }
        }
        else
        {
            if (Input.GetButton("Jump") && (grounded || stuck) && jumpReleased)
            {
                Jump();
            }
        }
        if (jumpPressed && !Input.GetButton("Jump"))
        {
            jumpReleased = true;
        }
        if (dashPressed && !Input.GetButton("Shift"))
        {
            if (!jumping)
                ink.enableEmission = false;
            dashPressed = false;
        }

    }

    void LookInDirection()
    {
	    Vector3 view = new Vector3(direction.x, 0, direction.z);
        if (!stuck)
        {
            Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z);
            Vector3 right = new Vector3(transform.right.x, 0, transform.right.z);
            float angleX = 90 - Vector3.Angle(groundNormal, forward);
            float angleZ = -(90 - Vector3.Angle(groundNormal, right));

            if (view != Vector3.zero && !stuck)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
            }

            if (grounded)
            {
                transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, angleX, Time.deltaTime * 10),
                    transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, angleZ, Time.deltaTime * 10));
            }
            else
            {
                transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, 0, Time.deltaTime * 5),
                    transform.localEulerAngles.y, Mathf.LerpAngle(transform.localEulerAngles.z, 0, Time.deltaTime * 5));
            }       
        }
        if (stuck || onEdge)
        {
            Vector3 wallDirection = new Vector3(-wallJumpDirection.x, 0, -wallJumpDirection.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(wallDirection), Time.deltaTime * 10);
        }
    }

	void Jump(float strength = 1f)
	{
        jumpPressed = true;
        jumpReleased = false;
		jumpDirection = Vector3.up * 25 * strength;
        if (stuck)
        {
            jumpDirection = wallJumpDirection * 20 + Vector3.up * 20;
        }
        if (onEdge)
        {
            jumpDirection = Vector3.up * 25 - wallJumpDirection * 5;
        }
		timeSinceJump = 0.0f;
		jumping = true;
        sound.Jump();
        anim.SetTrigger("jump");
        anim.ResetTrigger("stick");
        anim.SetBool("sticking", false);
	}

    void HandleJump()
    {

        if (grounded && timeSinceJump > 0.1f)
        {
            dashing = false;
            jumping = false;
            timeSinceJump = 0;
        }
        timeSinceJump += Time.deltaTime;
        if (jumping)
        {
            direction += jumpDirection * Time.deltaTime * 100;
        }
    }

    void Run()
    {
        ink.enableEmission = true;
        direction = direction * dashSpeedMultiplier;
    }

	void Dash()
	{
        jumpDirection = transform.forward + Vector3.up * 2;
        jumpDirection.Normalize();
        jumpDirection = jumpDirection * 30;

        if(!onEdge && !stuck)
        {
            groundTime = 0.0f;
            ink.enableEmission = true;
            Invoke("StopInk", 0.5f);
            anim.SetTrigger("dash");
            anim.ResetTrigger("stick");
        }
        sound.Jump();
		timeSinceJump = 0.0f;
        jumping = true;
        dashing = true;
	}

    void Fall(Vector3 oldVelocity)
    {
        if (controller.velocity.y < 0 && oldVelocity.y >= 0 && !stuck)
        {
            falling = true;
            fallOrigin = transform.position;
        }
        if (grounded && falling && !stuck)//hit the ground
        {
            if (Mathf.Abs(fallOrigin.y - transform.position.y) > fallHeight && !stuck)
            {
                sound.Land();
                anim.SetTrigger("crash");
                health.TakeDamage(50);
                fallOrigin = transform.position;
            }
            else
            {
                if (Mathf.Abs(fallOrigin.y - transform.position.y) >= 0.1f)
                {
                    sound.Land();
                    anim.SetTrigger("bounce");
                    fallOrigin = transform.position;
                }
            }
        }
        if (controller.velocity.y == 0 || grounded)
        {
            falltriggered = false;
            falling = false;
            fallingTime = 0.0f;
        }

        if (falling)
        {
            fallingTime = +Time.deltaTime;
        }

        if (fallingTime >= 0.01f && !falltriggered && !(stuck || onEdge))
        {
            falltriggered = true;
            anim.SetTrigger("fall");
            anim.ResetTrigger("jump");
        }
        anim.SetBool("falling", falling);
    }

    void Gravity()
    {
        float gravityInfluence = timeSinceJump;
        if (timeSinceJump > 1.0f)
        {
            gravityInfluence = 1.0f;
        }
        direction -= Vector3.up * gravity * gravityInfluence * Time.deltaTime;
    }

	public void StickToSurface(bool stick, Vector3 target)
	{
        if (stuck != stick)
        {
            anim.SetBool("sticking", stick);
            if (stick)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, target - transform.position, out hit))
                {

                    wallJumpDirection = hit.normal;
                }
                //jumpDirection = Vector3.zero;
                if (!anim.GetBool("hanging"))
                    anim.SetTrigger("stick");
            }
            else
            {
                anim.ResetTrigger("stick");
            }
        }
       
        stuck = stick;
        if (stick)
        {
            stickiness = 0.05f;
        }
        else
        {
            stickiness = 1.0f;
        }
	}

    public void HoldOntoEdge(bool hold, Vector3 edge, Edge edgeScript)
    {
        if (onEdge != hold)
        {
            anim.SetBool("hanging", hold);
            if (hold)
                anim.SetTrigger("hang");
            else
                anim.ResetTrigger("hang");
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

    void HangingOnEdge()
    {
         if (onEdge)
        {
            timeSinceJump = 0;
            grounded = false;
            direction += new Vector3(edge.x - transform.position.x, 0, edge.z - transform.position.z);
            if (edge.y - transform.position.y > 0.5f)
            {
                direction.y += Mathf.Lerp(direction.y, edge.y, Time.deltaTime * 10);
            }
        }
    }

    void ReleaseEdge()
    {
        jumpDirection = Vector3.zero;
        edgeScript.Release();
    }

    public bool IsDashing() { return dashing;}

    public void SetWallJumpDirection(Vector3 direction) 
    {
        wallJumpDirection = direction;
    }

    public void Turn()
    {
        jumpDirection = new Vector3(-jumpDirection.x, jumpDirection.y, -jumpDirection.z);
    }

    public void Bounce()
    {
        Jump(2.0f);
    }

    void StopInk()
    {
        ink.enableEmission = false;
    }

    bool IsGrounded()
    {
        if (grounded)
            anim.SetTrigger("ground");
        anim.SetBool("grounded", grounded);
        airTime += Time.deltaTime;
        if (controller.isGrounded)
            airTime = 0.0f;
        if (airTime >= 0.1f)
        {
            groundTime = 0.0f;
            return false;
        }
        else
        {
            groundTime += Time.deltaTime;
            if (groundTime >= 0.1f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, -Vector3.up, out hit))
                {
                    groundNormal = hit.normal;
                }
                else
                {
                    groundNormal = Vector3.up;
                }
            }
            return true;
        }
    }
}
