using UnityEngine;
using System.Collections;

public class PlayerSound : MonoBehaviour 
{
    public static PlayerSound instance;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip crash;
    public AudioClip walk;
    public AudioClip wall;
    AudioSource source;
    AudioSource walkSource;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        source = GetComponents<AudioSource>()[0];
        walkSource = GetComponents<AudioSource>()[1];
	}


    public void Jump()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = jump;
        if (source.clip != null)
        {
            source.Play();
        }
    }

    public void Land()
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = land;
        if (source.clip != null)
        {
            source.Play();
        }
    }

    public void Move(bool moving)
    {
        if (moving && !walkSource.isPlaying)
        {
            walkSource.pitch = 1 + Random.Range(-1f, 1f) * 0.2f;
            walkSource.Play();
        }
        else
        {
          
            
        }

    }

}
