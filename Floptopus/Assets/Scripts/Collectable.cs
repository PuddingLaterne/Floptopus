using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour
{
    CollectableManager manager;
    bool collected;

	void Start ()
    {
        collected = false;
        manager = GameObject.FindGameObjectWithTag("CollectableManager").GetComponent<CollectableManager>();
	}
	
    public void PlayerContact()
    {
        if (!collected)
        {
            manager.CollectableFound();
            this.gameObject.SetActive(false);
            collected = true;
        }
    }
}
