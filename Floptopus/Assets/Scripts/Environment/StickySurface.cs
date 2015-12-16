using UnityEngine;
using System.Collections;

public class StickySurface : MonoBehaviour 
{
    PlayerMovement player;
    GameObject playerObject;

	void Start () 
	{
		player = PlayerMovement.instance;
        playerObject = GameObject.FindGameObjectWithTag("Player");
	}

	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
            player.StickToSurface(true, transform.position);
	}

    void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player"))
            player.StickToSurface(false, transform.position);
	}
}
