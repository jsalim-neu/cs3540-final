using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonFSM : EnemyFSM
{
    /*
    FSM for new Skeleton enemy. Chase state lasts until the enemy is sufficiently close to the player, 
    at which point the Attack state will begin (i.e. shooting the enemy).
    */
    Animator anim;
    public Transform gunPoint;
    public GameObject bulletPrefab;

    public float bulletCooldown = 1f, bulletRefresh = 0;

    public float shootRange = 10f;

    float runSpeed;


    public override void Initialize()
    {
        anim = GetComponent<Animator>();
        currentState = FSMStates.Patrol;
        WanderAround();
        runSpeed = moveSpeed * 1.25f;
    }

    public override void UpdateIdleState()
    {

    }
    public override void UpdatePatrolState()
    {
        agent.stoppingDistance = 0;
        agent.speed = moveSpeed;
        anim.SetInteger("animState", 1);   

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
        anim.SetInteger("animState", 2);
        agent.speed = runSpeed;
        agent.stoppingDistance = 3;
        agent.SetDestination(player.position);


        if (!base.IsPlayerInLOS())
        {
            currentState = FSMStates.Patrol;
        }
        else if (Vector3.Distance(player.position, transform.position) < shootRange)
        {
            currentState = FSMStates.Attack;
        }
    }
    public override void UpdateAttackState()
    {
        agent.isStopped = true;
        anim.SetInteger("animState", 0);
        transform.LookAt(player.position);

        FireBullet();


        if (Vector3.Distance(player.position, transform.position) >= detectRange - 2)
        {
            currentState = FSMStates.Chase;
            agent.isStopped = false;
        }
        
    }
    public override void UpdateDeadState()
    {
        
    }

    void FireBullet()
    {
        if (bulletRefresh == 0)
        {
            //fire bullet
            Debug.Log("Enemy firing!");

            //instantiate new enemy bullet
            GameObject bulletClone = Instantiate(
              bulletPrefab,
              gunPoint.transform.position + gunPoint.transform.forward,
              gunPoint.transform.rotation
            ) as GameObject;


            //push bullet forward
            Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 5, ForceMode.VelocityChange);

            //script stuff: set the bullet damage and behaviour
            BulletBehaviour b = bulletClone.GetComponent<BulletBehaviour>();
            b.canDamagePlayer = true;
            b.damage = damageAmount;

            bulletClone.transform.SetParent(
                GameObject.FindGameObjectWithTag("BulletParent").transform
            );

            //reset cooldown
            bulletRefresh = bulletCooldown;
        }
        else 
        {
            bulletRefresh -= Time.deltaTime;
            bulletRefresh = Mathf.Clamp(bulletRefresh, 0,bulletCooldown);
        }
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Vector3 frontRayPoint = enemyEyes.position + (enemyEyes.forward * detectRange);
        Vector3 leftRayPoint = Quaternion.Euler(0, fov * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -fov * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(enemyEyes.position, frontRayPoint, Color.red);
        Debug.DrawLine(enemyEyes.position, leftRayPoint, Color.yellow);
        Debug.DrawLine(enemyEyes.position, rightRayPoint, Color.yellow);

    }
}