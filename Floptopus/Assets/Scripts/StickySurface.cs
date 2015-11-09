using UnityEngine;
using System.Collections;

public class StickySurface : MonoBehaviour 
{
	PlayerMovement player;
	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Player"))
			player.StickToSurface (true);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player"))
			player.StickToSurface (false);
	}



}
