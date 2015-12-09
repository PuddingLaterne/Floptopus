using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
    PlayerMovement player;

	void Start ()
    {
        player = PlayerMovement.instance;
	}
	
	void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.SetGrounded(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.SetGrounded(false);
        }
    }
}
