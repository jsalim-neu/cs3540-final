using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishFSM : EnemyFSM
{
    float runSpeed;
    float bulletHeight = 2.1f;

    //set cooldown for attacking player to prevent double-taps and corner combos; 
    //trigger collider is used to hurt player and thus will be deactivated for a short time afterward
    Collider triggerCollider; public float collisionCooldown = 0.5f; float collisionTimer = 0;

    // Update is called once per frame
    
    public override void EnemyUpdate()
    {
        if (collisionTimer <= 0 & !enemyHealth.isDead)
        {
            triggerCollider.enabled = true;
        }
        else
        {
            collisionTimer -= Time.deltaTime;
        }

        FishLookAt();
    }

    public override void Initialize()
    {
        currentState = FSMStates.Patrol;
        WanderAround();
        runSpeed = moveSpeed * 1.25f;
        triggerCollider = GetComponents<Collider>()[1];
        transform.position = new Vector3(transform.position.x, bulletHeight, transform.position.z);
    }

    public override void UpdateIdleState()
    {

    }

    public override void UpdatePatrolState()
    {
        agent.stoppingDistance = 0;
        agent.speed = moveSpeed;

        //if enemy successfully wanders around, find new random destination
        if (agent.remainingDistance < 0.5f)
        {
            base.WanderAround();
        }

        //if enemy sees player, transition to chase
        if (base.IsPlayerInLOS())
        {
            currentState = FSMStates.Chase;
        }
    }

    public override void UpdateChaseState()
    {
        agent.speed = runSpeed;
        agent.stoppingDistance = 0;
        agent.SetDestination(player.position);

        if (!base.IsPlayerInLOS())
        {
            currentState = FSMStates.Patrol;
        }
    }

    public override void UpdateAttackState()
    {
        
    }

    public override void UpdateDeadState()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer(other);
            if (enemyHealth.isGettingHit)
            {
                FishLookAt();
                transform.LookAt(player.position);

            }
            //if 
            if (collisionTimer <= 0 & !enemyHealth.isDead)
            {
                triggerCollider.enabled = true;
            }
            else
            {
                collisionTimer -= Time.deltaTime;
            }
        }
    }

    void DamagePlayer(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(damageAmount);

        //knockback wrapper is handled in PlayerHealth, which calls a function in PlayerController
        Vector3 moveDirection = (other.gameObject.transform.position - transform.position).normalized;
        playerHealth.TriggerKnockback(moveDirection);

        AudioSource.PlayClipAtPoint(playerHitSFX, Camera.main.transform.position);

        //deactivate player harming capabilities
        triggerCollider.enabled = false;
        collisionTimer = collisionCooldown;
    }

    private void FishLookAt()
    {
        /* transform.LookAt(agent.destination);
         transform.Rotate(0, 90, 0);
         Vector3 rotationCorrect = transform.eulerAngles;
         rotationCorrect.x = 0;
         rotationCorrect.z = 0;*/

        //transform.eulerAngles += new Vector3(0, 90, 0);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * detectRange);
        Vector3 leftRayPoint = Quaternion.Euler(0, fov * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -fov * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.red);
        Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
        Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);

    }
}
