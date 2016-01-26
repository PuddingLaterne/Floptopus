using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour 
{
    public static PlayerSound instance;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip wall;
    public AudioClip hurt;
    AudioSource source;
    AudioSource walkSource;
    AudioSource spraySource;
    public float walkPitch = 1.0f;
    public float sprayPitch = 0.5f;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        source = GetComponents<AudioSource>()[0];
        walkSource = GetComponents<AudioSource>()[1];
        spraySource = GetComponents<AudioSource>()[2];
	}


    public void Jump()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = jump;
        PlaySource();
    }

    public void Wall()
    {
        source.clip = wall;
        PlaySource();
    }

    public void Hurt()
    {
        source.clip = hurt;
        PlaySource();
    }

    public void Land()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = land;
        PlaySource();
    }

    public void Move(bool moving)
    {
        if (moving && !walkSource.isPlaying)
        {
            walkSource.pitch = walkPitch + Random.Range(-1f, 1f) * 0.2f;
            walkSource.Play();
        }
    }

    public void Spray(bool spraying)
    {
        if (spraying && !spraySource.isPlaying)
        {
            spraySource.pitch = sprayPitch + Random.Range(-1f, 1f) * 0.2f;
            spraySource.Play();
        }
        if (!spraying)
        {
            Debug.Log("stop spraying");
            spraySource.Stop();
        }
    }

    void PlaySource()
    {
        if (source.clip != null)
        {
            source.Play();
        }
    }

}
