using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour 
{
    PlayerMovement player;

    PlayerInk ink;
    PlayerHealth health;
    public static PlayerCollision instance;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
	    player = PlayerMovement.instance;
        ink = PlayerInk.instance;
        health = PlayerHealth.instance;
	}
	
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch(hit.gameObject.tag)
        {
            case "StickySurface":
                player.SetWallJumpDirection(hit.normal);
                break;
            case "Collectable":
                hit.gameObject.GetComponent<Collectable>().PlayerContact();
                break;
            case "Enemy":
                player.Turn();
                if (player.IsDashing())
                {
                    hit.gameObject.GetComponent<Enemy>().JumpedAt(-hit.normal);
                }
                break;
            case "LooseObject":
                if (player.IsDashing())
                    hit.gameObject.GetComponent<LooseObject>().FallOver(-hit.normal);
                break;
            case "Ink":
                hit.gameObject.GetComponentInParent<Ink>().Collect(this.gameObject);
                break;
            case "Health":
                hit.gameObject.GetComponentInParent<Health>().Collect(this.gameObject);
                break;
        }
    }
}
