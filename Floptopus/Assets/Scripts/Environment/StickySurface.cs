using UnityEngine;
using System.Collections;

public class StickySurface : MonoBehaviour 
{
    PlayerMovement player;

	void Start () 
	{
		player = PlayerMovement.instance;
	}

	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
            player.StickToSurface(true);
	}

    void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player"))
			player.StickToSurface (false);
	}
}
