using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
	public float jumpTime = 1.0f;
	public float minJumpHold = 0.1f;
	public float maxJumpHold = 1.0f;
    public float jumpStrength = 1;
    public float tilt = 1;

	private Vector3 viewDirection;
    private Vector3 direction;
	private Vector3 jumpDirection;
    private Rigidbody rb;
    private GroundContact ground;
	private float jumpHeldTime;
	private bool jumpPressed;
    private bool jumping;
	private float timeSinceJump;

	// Use this for initialization
	void Start ()
    {
		jumpHeldTime = 0.0f;
		jumpPressed = false;
        rb = GetComponent<Rigidbody>();
        ground = GetComponentInChildren<GroundContact>();
        jumping = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
		direction.y = 0;
		timeSinceJump += Time.deltaTime;
        if (timeSinceJump < jumpTime)
            direction += jumpDirection;
        else
        {
            //jumping = false;
            if (!ground.IsGrounded())
            {
                jumpDirection.y = 0;
                jumpDirection = jumpDirection * 0.95f;
                direction += jumpDirection;
            }
  
        }
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y + direction.y,  direction.z * speed);
        LookInDirection();

        if (ground.IsGrounded() && (timeSinceJump > jumpTime))
            jumping = false;

        if (Input.GetButton("Jump") && ground.IsGrounded())
			jumpPressed = true;
        if (jumpPressed)
        {
            jumpHeldTime += Time.deltaTime;
            if (jumpHeldTime > maxJumpHold)
                jumpHeldTime = maxJumpHold;
            if (jumpHeldTime < minJumpHold)
                jumpHeldTime = minJumpHold;
        }
		if (!Input.GetButton ("Jump") && jumpPressed && timeSinceJump > jumpTime && ground.IsGrounded()) // Jump-Button released;
			Jump ();

    }

    void LookInDirection()
    {
        if(direction!= Vector3.zero)	
        {
		    Vector3 view = new Vector3(direction.x, 0, direction.z);
            if (jumping)
                view.y = rb.velocity.y;
            if(view.y != 0)
               Debug.Log(view);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 5);
        }

    }

	void Jump()
	{
        jumpHeldTime = jumpHeldTime * 10;
        //jumpDirection = transform.forward + Vector3.up * jumpHeldTime / 2;
        //jumpDirection.Normalize ();
        //jumpDirection = jumpDirection * jumpHeldTime * jumpStrength;

        jumpDirection = transform.forward + Vector3.up * tilt;
        jumpDirection.Normalize();
        jumpDirection = jumpDirection * jumpStrength;

        //big jump: 15, 20, small jump 5, 3 , dash 10, 4

        jumpPressed = false;
		jumpHeldTime = 0.0f;
		timeSinceJump = 0.0f;
        jumping = true;
	}
}
