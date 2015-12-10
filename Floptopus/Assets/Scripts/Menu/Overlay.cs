using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour 
{
    Animator anim;

	void Start () 
    {
        anim = GetComponent<Animator>();
        Invoke("Disable", 0.5f);
	}

    void Disable()
    {
        this.gameObject.SetActive(false);
    }

    public void FadeOut()
    {
        this.gameObject.SetActive(true);
        anim.SetTrigger("fadeout");
    }

}
