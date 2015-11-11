using UnityEngine;
using System.Collections;

public class StickySurface : MonoBehaviour 
{
    PlayerMovement player;
    Vector3 faceDirection;

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerMovement> ();
        faceDirection = transform.forward;
	}

	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
        {
            player.StickToSurface(true);
            player.SetWallJumpDirection(faceDirection);
        }
	}

    void OnTriggerExit(Collider other)
	{
		if (other.CompareTag ("Player"))
			player.StickToSurface (false);
	}
}
