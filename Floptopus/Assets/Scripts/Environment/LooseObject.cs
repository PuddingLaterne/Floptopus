using UnityEngine;
using System.Collections;

public class LooseObject : MonoBehaviour 
{
    AudioSource audio;
    public AudioClip fall;
    public AudioClip crash;
    bool falling;
    bool fallen;
    MeshCollider meshCollider;
    BoxCollider boxCollider;
    float fallingTime;
    float fallAngle;

	void Start () 
    {
        audio = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
        meshCollider = GetComponentInChildren<MeshCollider>();
        fallingTime = 0;
        falling = false;
        fallen = false;
	}

    void Update()
    {
        if (falling && !fallen)
        {
            float timeFactor;
            if (fallingTime >= 1.0f)
                timeFactor = 5.0f;
            else
                timeFactor = 2.0f;
            fallingTime += Time.deltaTime * timeFactor;

            transform.localEulerAngles = new Vector3(Mathf.LerpAngle(transform.localEulerAngles.x, fallAngle, Time.deltaTime * Mathf.Pow(fallingTime, 2)),
                transform.localEulerAngles.y, transform.localEulerAngles.z);

            if (Mathf.Abs(transform.localEulerAngles.x - 90) <= 0.1f)
            {
                if (crash != null)
                {
                    audio.clip = crash;
                    audio.Play();
                }
                fallen = true;
            }
        }
    }

    public void FallOver(Vector3 forceDirection)
    {
        if (fall != null)
        {
            audio.clip = fall;
            audio.Play();
        }
        boxCollider.enabled = false;
        meshCollider.enabled = true;
        if (Vector3.Angle(transform.forward, forceDirection) < 90)
            fallAngle = 90;
        else
            fallAngle = -90;
        falling = true;
    }
}
