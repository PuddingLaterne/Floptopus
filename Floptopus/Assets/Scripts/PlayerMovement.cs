using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
	public float jumpTime = 1.0f;
	public float minJumpHold = 0.1f;
	public float maxJumpHold = 1.0f;
	public float minJumpStrength = 2;
	public float minJumpAngle = 30;
	public float maxJumpStrength = 10;
	public float maxJumpAngle = 90;

	private Vector3 viewDirection;
    private Vector3 direction;
	private Vector3 jumpDirection;
    private Rigidbody rb;
    private GroundContact ground;
	private float jumpHeldTime;
	private bool jumpPressed;
	private float timeSinceJump;

	// Use this for initialization
	void Start ()
    {
		jumpHeldTime = 0.0f;
		jumpPressed = false;
        rb = GetComponent<Rigidbody>();
        ground = GetComponentInChildren<GroundContact>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
		direction.y = 0;
		timeSinceJump += Time.deltaTime;
		if(timeSinceJump < jumpTime)
			direction += jumpDirection;
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y + direction.y,  direction.z * speed);
        LookInDirection();

		if (ground.IsGrounded ())
			rb.useGravity = false;
		else
			rb.useGravity = true;

		if(Input.GetButton("Jump") && ground.IsGrounded())
			jumpPressed = true;
		if (jumpPressed)
			jumpHeldTime += Time.deltaTime;
		if (!Input.GetButton ("Jump") && jumpPressed && timeSinceJump > jumpTime && ground.IsGrounded()) // Jump-Button released;
			Jump ();

    }

    void LookInDirection()
    {
        if(direction!= Vector3.zero)	
        {
			Vector3 view = new Vector3(direction.x, 0, direction.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(view), Time.deltaTime * 5);
        }
		//transform.eulerAngles  = new Vector3(0, transform.rotation.y, transform.rotation.z);
    }

	void Jump()
	{
		if (jumpHeldTime > maxJumpHold)
			jumpHeldTime = maxJumpHold;
		jumpDirection = transform.forward + Vector3.up;
		jumpDirection.Normalize ();
		jumpDirection = jumpDirection * 3;
		//jumpDirection += jumpHeldTime * transform.forward;
		//jumpDirection += jumpHeldTime * transform.up;
		//jumpDirection = jumpDirection.normalized * 5;
		jumpPressed = false;
		jumpHeldTime = 0.0f;
		timeSinceJump = 0.0f;
	}
}
