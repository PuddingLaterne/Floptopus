using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour 
{
    Animator anim;
    PlayerMovement player;

	void Start () 
    {
        anim = GetComponent<Animator>();
        player = PlayerMovement.instance;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetTrigger("bounce");
            player.Bounce();
        }
    }
}
