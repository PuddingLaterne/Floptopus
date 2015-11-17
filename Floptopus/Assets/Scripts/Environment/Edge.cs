using UnityEngine;
using System.Collections;

public class Edge : MonoBehaviour 
{
    Player player;
    Collider collider;
    BoxCollider mainCollider;

	void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        mainCollider = transform.parent.gameObject.GetComponent<BoxCollider>();
        collider = GetComponent<Collider>();
	}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            player.movement.HoldOntoEdge(true, transform.position, this);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player.movement.HoldOntoEdge(false, Vector3.zero, this);
    }

    public void Release()
    {
        player.movement.HoldOntoEdge(false, Vector3.zero, this);
        collider.enabled = false;
        mainCollider.enabled = false;
        Invoke("Renable", 0.7f);
    }

    void Renable()
    {
        collider.enabled = true;
        mainCollider.enabled = true;
    }
}
