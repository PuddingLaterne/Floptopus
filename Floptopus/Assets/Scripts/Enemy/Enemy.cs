using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public Transform[] patrolPoints;
    GameObject playerObject;
    public GameObject collectable;
    float biteOffsetTime = 0.5f;
    float fallOffsetTime = 1.0f;
    Player player;
    NavMeshAgent nav;
    Animator anim;
    Transform target;
    int targetIndex;
    bool playerVisible;
    bool alive;
    float lastBite = 0;
    float lastFall = 0;
    int health = 100;

    public float fieldOfView = 45;
    public float viewRange = 100;

	void Start () 
    {
        alive = true;
        playerVisible = false;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        targetIndex = 0;
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(patrolPoints[0].position);
	}
	
	void Update () 
    {
        if (!alive)
            return;
        lastBite += Time.deltaTime;
        lastFall += Time.deltaTime;
        if (lastFall >= fallOffsetTime)
            nav.Resume();
        if (playerVisible)
        {
            nav.SetDestination(playerObject.transform.position);
            nav.speed = 10;
        }
        else
        {
            nav.SetDestination(patrolPoints[targetIndex].position);
            nav.speed = 5;
        }
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
            anim.SetTrigger("bite");
            player.health.TakeDamage(20);
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
        if (Vector3.Angle(transform.forward, direction) < 90) // jumped at from behind
            Fall();
        else
            Bite();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, playerObject.transform.position + Vector3.up - transform.position, out hit, Vector3.Distance(transform.position, player.transform.position)))
                SetPlayerVisible(false);
            else
            {
                if (Mathf.Abs(Vector3.Angle(transform.forward, playerObject.transform.position - transform.position)) < fieldOfView)
                    SetPlayerVisible(true);
                else
                    SetPlayerVisible(false);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            SetPlayerVisible(false);
    }

    void Fall()
    {
        if (lastFall >= fallOffsetTime)
        {
            nav.Stop();
            health -= 50;
            anim.SetTrigger("fall");
            lastFall = 0;
            if (health <= 0)
            {
                alive = false;
                nav.Stop();
                anim.SetBool("dead", true);
                GetComponent<CapsuleCollider>().enabled = false;
                Invoke("SpawnCollectable", 2);
                Invoke("Die", 3);
            }
        }
    }

    void SpawnCollectable()
    { 
        GameObject.Instantiate(collectable, transform.position + transform.forward * 10 + Vector3.up * 3, Quaternion.Euler(Vector3.zero));
    }

    void Die()
    {
        anim.SetBool("gone", true);
        GameObject.Destroy(this, 2);
    }

    void SetPlayerVisible(bool visible)
    {
        if (playerVisible && !visible) //Player was visible before
            anim.SetTrigger("confused");
        playerVisible = visible;
    }
}
