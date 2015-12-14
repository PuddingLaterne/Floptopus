﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour 
{
    public Transform[] patrolPoints;
    public GameObject playerObject;
    PlayerHealth player;
    public GameObject collectable;
    float biteOffsetTime = 0.5f;
    float fallOffsetTime = 1.0f;
    NavMeshAgent nav;
    Animator anim;
    Transform target;
    int targetIndex;
    bool playerVisible;
    bool alive;
    bool fallen = false;
    bool confused = false;
    bool collectableDropped = false;
    float lastBite = 0;
    float lastFall = 0;
    float confusedTime;


    public float fieldOfView = 45;
    public float viewRange = 100;

	void Start () 
    {
        alive = true;
        playerVisible = false;
        player = PlayerHealth.instance;
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
        confusedTime += Time.deltaTime;
        if (lastFall >= fallOffsetTime && fallen)
        {
            fallen = false;
            nav.Resume();
        }
        if (confusedTime >= 2f && confused)
        {
            confused = false;
            nav.Resume();
        }
        if (playerVisible)
        {
            nav.SetDestination(playerObject.transform.position);
            nav.speed = 10;
        }
        else
        {
            if (!confused)
            {
                nav.SetDestination(patrolPoints[targetIndex].position);
                nav.speed = 5;
            }
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
            Invoke("Crunch", 0.15f);
            lastBite = 0;
        }
    }

    void Crunch()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < 7.5f && Mathf.Abs(AngleToPlayer()) < 60 )
        {
            player.TakeDamage(20);
        }
    }

    float AngleToPlayer()
    {
        return Vector3.Angle(player.transform.position - transform.position, transform.forward);
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
            if (Physics.Raycast(transform.position, playerObject.transform.position + Vector3.up - transform.position, out hit, Vector3.Distance(transform.position, playerObject.transform.position) - 5))
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
            fallen = true;
            nav.Stop();
            anim.SetTrigger("fall");
            lastFall = 0;
            if (!collectableDropped)
            {
                collectableDropped = true;
                Invoke("SpawnCollectable", 0.1f);
            }
        }
    }

    void SpawnCollectable()
    { 
        GameObject.Instantiate(collectable, transform.position + transform.forward * -5 + Vector3.up * 3, Quaternion.Euler(Vector3.zero));
    }

    void SetPlayerVisible(bool visible)
    {
        if (playerVisible && !visible) //Player was visible before
        {
            nav.Stop();
            anim.SetTrigger("confused");
            confused = true;
            confusedTime = 0.0f;
        }
        if (!playerVisible && visible) //Player was invisible before
            anim.SetTrigger("chase");
        anim.SetBool("chasing", visible);
        playerVisible = visible;
    }
}
