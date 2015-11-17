using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    public PlayerHealth health;
    public PlayerMovement movement;
    public PlayerCollision collision;

	void Start () 
    {
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();
        collision = GetComponent<PlayerCollision>();
	}
	
	void Update () 
    {
	
	}
}
