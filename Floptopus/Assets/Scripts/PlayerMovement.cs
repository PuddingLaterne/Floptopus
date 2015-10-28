using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float jumpStrength = 5;
    public float maxJumpTime;
    private float jumpingTime;
    private Vector3 direction;
    private Rigidbody rb;
    private GroundContact ground;
    private bool falling;

	// Use this for initialization
	void Start ()
    {
        falling = false;
        rb = GetComponent<Rigidbody>();
        ground = GetComponentInChildren<GroundContact>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        direction = Camera.main.transform.forward * Input.GetAxis("Vertical") + Camera.main.transform.right * Input.GetAxis("Horizontal");
        direction.y = 0;
        rb.velocity = new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed);
        LookInDirection();

        if(Input.GetButton("Jump") && ground.IsGrounded())
        {

            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
        }
        if (!ground.IsGrounded() && jumpingTime >= maxJumpTime)
            falling = true;
        else
            falling = false;
        if(falling)
            rb.velocity = new Vector3(rb.velocity.x, -15, rb.velocity.z);
    }

    void LookInDirection()
    {
        if(direction.x != 0 || direction.z != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
        }
    }
}
