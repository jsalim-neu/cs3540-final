using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityBehaviour : MonoBehaviour
{
    public Transform player;
    public enum FSMState
    {
        Idle,
        Patrol,
        SeePlayer,
        Talk,
        Dead
    }

    public FSMState currentState;

    NavMeshAgent agent;

    Vector3 originalPos;

    Animator anim;

    void Start()
    {
        currentState = FSMState.Patrol;
        agent = GetComponent<NavMeshAgent>();
        originalPos = transform.position;
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        anim = GetComponentInChildren<Animator>();
        anim.SetInteger("animState", 0);
    }

    void Update()
    {
        switch (currentState)
        {
            case FSMState.Idle:
                UpdateIdleState();
                break;
            case FSMState.Patrol:
                UpdatePatrolState();
                break;
            case FSMState.SeePlayer:
                UpdateSeePlayerState();
                break;
            case FSMState.Talk:
                UpdateTalkState();
                break;
            case FSMState.Dead:
                UpdateDeadState();
                break;
        }
    }

    // stop moving
    void UpdateIdleState()
    {
        agent.isStopped = true;
        anim.SetInteger("animState", 0);
    }

    void UpdatePatrolState()
    {
        agent.isStopped = false;
        anim.SetInteger("animState", 1);

        if (agent.remainingDistance < 0.5f)
        {
            WanderAround();
        }

        if (Vector3.Distance(player.position, transform.position) <= 5f)
        {
            currentState = FSMState.SeePlayer;
        }
        else {Debug.Log(Vector3.Distance(player.position, transform.position));}
    }

    void UpdateSeePlayerState()
    {
        Debug.Log(Vector3.Distance(player.position, transform.position));
        agent.isStopped = true;
        anim.SetInteger("animState", 2);
        if (Vector3.Distance(player.position, transform.position) >= 10f)
        {
            currentState = FSMState.Patrol;
        }
    }

    void UpdateTalkState() 
    {
        agent.isStopped = true;
        anim.SetInteger("animState", 3);
    }

    void UpdateDeadState() {}

    // Find a random point within the NavMesh and set it as the destination
    void WanderAround()
    {
        Vector3 randomPos = Random.insideUnitSphere * 10 + originalPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, 10, 1);
        agent.SetDestination(hit.position);
    }

}
