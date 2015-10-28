using UnityEngine;
using System.Collections;

public class GroundContact : MonoBehaviour
{
    private bool grounded;
	void Start ()
    {	
	}
	
	void Update ()
    {	
	}

    public bool IsGrounded()
    {
        return grounded;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
            grounded = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
            grounded = false;
    }
}


