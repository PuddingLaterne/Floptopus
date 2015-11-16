using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public Transform[] patrolPoints;
    GameObject player;
    float biteOffsetTime = 0.5f;
    PlayerMovement playerMovement;
    PlayerHealth playerHealth;
    NavMeshAgent nav;
    Transform target;
    int targetIndex;
    bool playerVisible;
    float lastBite;

    public float fieldOfView = 45;
    public float viewRange = 100;

	void Start () 
    {

        playerVisible = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerHealth = player.GetComponent<PlayerHealth>();
        targetIndex = 0;
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(patrolPoints[0].position);
	}
	
	void Update () 
    {
        lastBite += Time.deltaTime;
        if (playerVisible)
            nav.SetDestination(player.transform.position);
        else
            nav.SetDestination(patrolPoints[targetIndex].position);
	    if (!nav.pathPending)
         {
             if (nav.remainingDistance <= nav.stoppingDistance)
             {
                 if (playerVisible)
                     Bite();
                 if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                 {
                     ApproachNextPatrolPoint();
                 }
             }
         }
	}

    void Bite()
    {
        if (lastBite >= biteOffsetTime)
        {
            playerHealth.TakeDamage(20);
            lastBite = 0;
        }
    }

    void ApproachNextPatrolPoint()
    {
        if (targetIndex < patrolPoints.Length - 1)
            targetIndex ++;
        else
            targetIndex = 0;
        nav.SetDestination(patrolPoints[targetIndex].position);
    }

    public void JumpedAt(Vector3 direction)
    {
        if (Vector3.Angle(transform.forward, direction) < 90)
            Debug.Log("jumped at from behind");
        else
            Debug.Log("jumped at from front");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, player.transform.position + Vector3.up - transform.position, out hit, Vector3.Distance(transform.position, player.transform.position)))
                playerVisible = false;
            else
            {
                if (Mathf.Abs(Vector3.Angle(transform.forward, player.transform.position - transform.position)) < fieldOfView)
                    playerVisible = true;
                else
                    playerVisible = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerVisible = false;
    }
}
