using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour 
{
    Animator anim;
    PlayerMovement player;
    AudioSource audio;

	void Start () 
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        player = PlayerMovement.instance;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audio.Play();
            anim.SetTrigger("bounce");
            player.Bounce();
        }
    }
}
