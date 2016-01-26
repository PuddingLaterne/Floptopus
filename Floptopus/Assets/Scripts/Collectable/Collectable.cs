using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
    AudioSource audio;
    Animator anim;
    CollectableManager manager;
    bool collected;
    public ParticleSystem particles;

	void Start ()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        collected = false;
        manager = GameObject.FindGameObjectWithTag("CollectableManager").GetComponent<CollectableManager>();
	}

    public void Spawn()
    {
        anim.SetTrigger("spawn");
    }
	
    public void PlayerContact()
    {
        if (!collected)
        {
            if (audio != null && audio.clip != null)
            {
                audio.Play();
            }
            manager.CollectableFound();
            Invoke("Collect", 1f);
            collected = true;
            GetComponent<SphereCollider>().enabled = false;
            //particles.enableEmission = true;
            particles.Play();
            
        }
    }

    void Collect()
    {
       
        this.gameObject.SetActive(false);
    }
}
