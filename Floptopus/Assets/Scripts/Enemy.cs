using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public Transform[] patrolPoints;
    NavMeshAgent nav;
    Transform target;
    int targetIndex;

	// Use this for initialization
	void Start () 
    {
        targetIndex = 0;
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(patrolPoints[0].position);
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (!nav.pathPending)
         {
             if (nav.remainingDistance <= nav.stoppingDistance)
             {
                 if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
                 {
                     ApproachNextPatrolPoint();
                 }
             }
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
}
