using UnityEngine;
using System.Collections;

public class LevelBoundaries : MonoBehaviour 
{
    PlayerHealth health;
    PlayerMovement move;

    void Start()
    {
        health = PlayerHealth.instance;
        move = PlayerMovement.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            health.TakeDamage(1);
            move.Respawn();
        }
    }


}
