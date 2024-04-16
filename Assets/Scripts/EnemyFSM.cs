using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public abstract class EnemyFSM : MonoBehaviour
{
    /*
    This abstract class is going to hold a lot of the general enemy behaviors, which will be built out more in child classes.

    Planning the FSM:
    
    Current Functionality:
    - Chase player
    - Damage player on contact
        - play SFX when hitting player
        - collision detected via OnTriggerEnter
    - Move via NavMeshAgent
    - correct for bullet height (can be deprecated)

    FSM Required Functionality:
    - State determined by NPC vision
    - Animations played according to state (may be dictated by EnemyHealth as well)
    - For Skeleton: shoot from a distance, rather than melee attack
        - also, need to adjust position between shots, possibly
    */

    //FSM variables

    public enum FSMStates 
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Dead
    }

    public FSMStates currentState;

    public Transform enemyEyes;

    //movement, attack variables
    public AudioClip playerHitSFX;
    public Transform player;
    public float moveSpeed = 5f;

    //patrol state information
    public float wanderDistance = 10f; Vector3 originalPos;
    public int damageAmount = 1;
    public float detectRange = 15f;

    public EnemyHealth enemyHealth;

    protected NavMeshAgent agent;

    public float fov = 45;



    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
        originalPos = transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        //initialize any values not contained in this method
        Initialize();
    }

    public abstract void Initialize();

    // Update is called once per frame
    void Update()
    {
        //general FSM behavior
        switch (currentState)
        {
            case FSMStates.Idle:
                UpdateIdleState();
                break;
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Dead:
                UpdateDeadState();
                break;
        }

        EnemyUpdate();
    }

    public abstract void EnemyUpdate();

    public abstract void UpdateIdleState();
    public abstract void UpdatePatrolState();
    public abstract void UpdateChaseState();
    public abstract void UpdateAttackState();
    public abstract void UpdateDeadState();

    // Find a random point within the NavMesh and set it as the destination; mostly useful for patrolling
    protected void WanderAround()
    {
        Vector3 randomPos = Random.insideUnitSphere * wanderDistance + originalPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, 10, 1);
        agent.SetDestination(hit.position);
    }

    //Check whether player is within the enemy's FOV using RaycastHits
    protected bool IsPlayerInLOS()
    {
        RaycastHit hit;

        Vector3 directionToPlayer = player.transform.position - enemyEyes.position;
        //if angle is less than/equal to FOV
        if (Vector3.Angle(directionToPlayer, enemyEyes.forward) <= fov)
        {
            //if player detected in sight raycast of length detectRange
            if (Physics.Raycast(enemyEyes.position, directionToPlayer, out hit, detectRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Debug.Log("Player in LOS!");
                    return true;
                }
                return false;
            }
            return false;
        } 
        return false;
    }
}
