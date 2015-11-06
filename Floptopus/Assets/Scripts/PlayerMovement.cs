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
    private CharacterController rb;
    private EnvironmentCheck environment;
	private float jumpHeldTime;
    private float tilt;
    private float jumpStrength;
    private bool jumpPressed;
    private bool jumping;
	private bool grounded;
	private float timeSinceJump;

	// Use this for initialization
	void Start ()
    {
		jumpHeldTime = 0.0f;
		jumpPressed = false;
        rb = GetComponent<CharacterController>();
        jumping = false;
		grounded = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
		direction.y = 0;
		direction = direction * speed;

		if (!rb.isGrounded) 
		{
			float gravityInfluence = timeSinceJump;
			if(timeSinceJump > 1.0f)
				gravityInfluence = 1.0f;
			direction -= Vector3.up * gravity * gravityInfluence * 10 * Time.deltaTime;
		}
		if (rb.isGrounded && timeSinceJump > 0.2f)
			jumping = false;

		timeSinceJump += Time.deltaTime;
		if (jumping) 
			direction += jumpDirection * Time.deltaTime * 100;

		rb.Move(new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime,  direction.z * Time.deltaTime));

        LookInDirection();

        if (Input.GetButton("Jump"))
			jumpPressed = true;
        if (jumpPressed)
            jumpHeldTime += Time.deltaTime;
		if (!Input.GetButton ("Jump") && jumpPressed && rb.isGrounded) // Jump-Button released;
			Jump ();

    }

    void LookInDirection()
    {

	    Vector3 view = new Vector3(direction.x, 0, direction.z);
		if (jumping)
            view.y = rb.velocity.y;
		if(view != Vector3.zero)
        	transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 10);
		if(!jumping)
          transform.eulerAngles = new Vector3(Mathf.LerpAngle(transform.eulerAngles.x, 0, Time.deltaTime * 5), transform.eulerAngles.y, transform.eulerAngles.z);

    }

	void Jump()
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
}
