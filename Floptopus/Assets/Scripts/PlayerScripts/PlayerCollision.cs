using UnityEngine;
using System.Collections;

public class PlayerCollision : MonoBehaviour 
{
    Player player;

	void Start () 
    {
	    player = GetComponent<Player>();
	}
	
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch(hit.gameObject.tag)
        {
            case "StickySurface":
                player.movement.SetWallJumpDirection(hit.normal);
                break;
            case "Collectable":
                hit.gameObject.GetComponent<Collectable>().PlayerContact();
                break;
            case "Enemy":
                if (player.movement.IsJumping())
                    hit.gameObject.GetComponent<Enemy>().JumpedAt(-hit.normal);
                break;
            case "LooseObject":
                if (player.movement.IsJumping())
                    hit.gameObject.GetComponent<LooseObject>().FallOver(-hit.normal);
                break;
        }
    }
}
