using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
	public float jumpTime = 1.0f;
    public float jumpHoldThreshold = 0.5f;

	private Vector3 viewDirection;
    private Vector3 direction;
	private Vector3 jumpDirection;
    private Rigidbody rb;
    private EnvironmentCheck environment;
	private float jumpHeldTime;
    private float tilt;
    private float jumpStrength;
    private bool jumpPressed;
    private bool jumping;
	private float timeSinceJump;

	// Use this for initialization
	void Start ()
    {
		jumpHeldTime = 0.0f;
		jumpPressed = false;
        rb = GetComponent<Rigidbody>();
        environment = GetComponentInChildren<EnvironmentCheck>();
        jumping = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
		direction.y = 0;
		timeSinceJump += Time.deltaTime;
        if (timeSinceJump < jumpTime)
            direction += jumpDirection * (1 - timeSinceJump);
        else
        {
            //jumping = false;
            if (!environment.IsGrounded())
            {
                jumpDirection.y = 0;
                jumpDirection = jumpDirection * 0.95f;
                direction += jumpDirection;
            }

        }
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y + direction.y,  direction.z * speed);
        LookInDirection();

        if (environment.IsGrounded() && (timeSinceJump > jumpTime))
            jumping = false;

        if (Input.GetButton("Jump") && environment.IsGrounded())
			jumpPressed = true;
        if (jumpPressed)
        {
            jumpHeldTime += Time.deltaTime;
        }
		if (!Input.GetButton ("Jump") && jumpPressed && timeSinceJump > jumpTime && environment.IsGrounded()) // Jump-Button released;
			Jump ();

    }

    void LookInDirection()
    {
        if(direction!= Vector3.zero)	
        {
		    Vector3 view = new Vector3(direction.x, 0, direction.z);
            if (jumping)
                view.y = rb.velocity.y;
            view = rb.velocity;
            if (view.y < 0)
                view.y = view.y / 3;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
        }
        if(!jumping)
          transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);

    }

	void Jump()
	{
        if(jumpHeldTime >= jumpHoldThreshold)
        {
            tilt = 2;
            jumpStrength = 11;
        }
        else
        {
            tilt = 1.5f;
            jumpStrength = 5;
        }

        jumpDirection = transform.forward + Vector3.up * tilt;
        jumpDirection.Normalize();
        jumpDirection = jumpDirection * jumpStrength;

        jumpPressed = false;
		jumpHeldTime = 0.0f;
		timeSinceJump = 0.0f;
        jumping = true;
	}
}
