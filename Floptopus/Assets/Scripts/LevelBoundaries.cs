using UnityEngine;
using System.Collections;

public class LevelBoundaries : MonoBehaviour 
{
    PlayerHealth health;
    PlayerMovement move;
    public int damage = 40;
    Transform[] respawnPoints;

    void Start()
    {
        respawnPoints = GetComponentsInChildren<Transform>();
        health = PlayerHealth.instance;
        move = PlayerMovement.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            health.TakeDamage(damage);
            int closestPoint = 0;
            for (int i = 0; i < respawnPoints.Length; i++)
            {
                if(Vector3.Distance(move.transform.position, respawnPoints[i].position)
                    < Vector3.Distance(move.transform.position, respawnPoints[closestPoint].position))
                {
                    closestPoint = i;
                }
            }
            move.Respawn(respawnPoints[closestPoint].position);
        }
    }


}
