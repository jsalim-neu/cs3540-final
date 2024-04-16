using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FishEnemyBehavior : MonoBehaviour
{

    //movement, player detection vars
    //public AudioClip playerHitSFX;
    public Transform player;
    public float moveSpeed = 5f;
    public int damageAmount = 1;
    public float detectionRadius = 10f;

    bool isAggro;

    public static float bulletHeight = 2.1f;

    public EnemyHealth enemyHealth;

    NavMeshAgent agent;

    //set cooldown for attacking player to prevent double-taps and corner combos; 
    //trigger collider is used to hurt player and thus will be deactivated for a short time afterward
    Collider triggerCollider; public float collisionCooldown = 0.5f; float collisionTimer = 0;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        isAggro = false;

        enemyHealth = GetComponent<EnemyHealth>();
        triggerCollider = GetComponents<Collider>()[1];

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.speed = moveSpeed;

        transform.position = new Vector3(transform.position.x, bulletHeight, transform.position.z);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver)
        {
            //if player is close to enemy, it goes aggro
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius) {
                isAggro = true;
            }

            //if enemy is activated, follow player
            if (isAggro && !enemyHealth.isGettingHit && !enemyHealth.isDead) {
                FollowPlayer();
            }
            fishLookAt();
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
        else {
            agent.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer(other);
            if (enemyHealth.isGettingHit) {
                fishLookAt();
            }
            //if 
            if (collisionTimer <= 0 & !enemyHealth.isDead)
            {
                triggerCollider.enabled = true;
            }
            else {
                collisionTimer -= Time.deltaTime;
            }
        }
    }

    void FollowPlayer() {
        // handle navmesh agent
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);

    }

    void DamagePlayer(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(damageAmount);

        //knockback wrapper is handled in PlayerHealth, which calls a function in PlayerController
        Vector3 moveDirection = (other.gameObject.transform.position - transform.position).normalized;
        playerHealth.TriggerKnockback(moveDirection);

        //AudioSource.PlayClipAtPoint(playerHitSFX, Camera.main.transform.position);

        //deactivate player harming capabilities
        triggerCollider.enabled = false;
        collisionTimer = collisionCooldown;        
    }

    private void fishLookAt()
    {
        transform.LookAt(player);
        transform.Rotate(0,90,0);
        Vector3 rotationCorrect = transform.eulerAngles;
        rotationCorrect.x = 0;
        rotationCorrect.z = 0;

        transform.eulerAngles = rotationCorrect;
    }
}
