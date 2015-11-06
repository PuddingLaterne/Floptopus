using UnityEngine;
using System.Collections;

public class EnvironmentCheck : MonoBehaviour
{
	private bool grounded;
	void Start()
	{
		grounded = false;
	}
	
	void Update()
	{
	}
	
	public bool IsGrounded()
	{
		return grounded;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
			Debug.Log ("NOOOOOOOOO");
		if (other.CompareTag ("Floor")) 
		{
			Debug.Log ("AARGH");
				
			grounded = true;
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		Debug.Log (other + " exit");
		if (other.CompareTag("Floor"))
			grounded = false;
	}
}




