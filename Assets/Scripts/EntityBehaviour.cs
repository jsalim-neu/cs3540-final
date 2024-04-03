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
        Attack,
        Dead
    }

    public FSMState currentState;

    NavMeshAgent agent;

    void Start()
    {
        currentState = FSMState.Patrol;
        agent = GetComponent<NavMeshAgent>();
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
            case FSMState.Attack:
                UpdateAttackState();
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
    }

    void UpdatePatrolState()
    {
        agent.isStopped = false;

        if (agent.remainingDistance < 0.5f)
        {
            WanderAround();
        }
    }

    void UpdateAttackState()
    {}

    void UpdateDeadState() {}

    // Find a random point within the NavMesh and set it as the destination
    void WanderAround()
    {
        Vector3 randomPos = Random.insideUnitSphere * 10;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, 10, 1);
        agent.SetDestination(hit.position);
    }

}
