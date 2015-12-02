using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
    Player player;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player.movement.SetGrounded(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.movement.SetGrounded(false);
        }
    }
}
