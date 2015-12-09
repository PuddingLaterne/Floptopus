using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour 
{
    PlayerMovement player;
    public static PlayerCollision instance;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
	    player = PlayerMovement.instance;
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
                if (player.IsJumping())
                    hit.gameObject.GetComponent<Enemy>().JumpedAt(-hit.normal);
                break;
            case "LooseObject":
                if (player.IsJumping())
                    hit.gameObject.GetComponent<LooseObject>().FallOver(-hit.normal);
                break;
        }
    }
}
