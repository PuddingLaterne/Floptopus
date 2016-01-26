using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour 
{
    AudioSource audio;
    PlayerHealth player;
    public float value;
    Animator anim;
    SphereCollider collider;
    bool collected = false;
    GameObject target;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        player = PlayerHealth.instance;
        collider = GetComponentInChildren<SphereCollider>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (collected)
        {
            transform.position = Vector3.Lerp(transform.position, target.transform.position, Time.deltaTime * 3);
            if (Vector3.Distance(transform.position, target.transform.position) <= 1f)
            {
                CancelInvoke();
                Deactivate();
            }
        }
    }

    public float Collect(GameObject player)
    {
        if (audio != null && audio.clip != null)
        {
            audio.Play();
        }
        target = player;
        collected = true;
        collider.enabled = false;
        Invoke("Deactivate", 1f);
        anim.SetTrigger("collected");
        return value;
    }

    void Deactivate()
    {
        player.CollectHealth(value);
        gameObject.SetActive(false);
    }
}
