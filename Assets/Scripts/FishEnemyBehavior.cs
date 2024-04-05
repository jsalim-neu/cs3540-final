using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FishEnemyBehavior : MonoBehaviour
{

    //movement, player detection vars
    public AudioClip playerHitSFX;
    public Transform player;
    public float moveSpeed = 5f;
    public int damageAmount = 1;
    public float detectionRadius = 10f;

    bool isAggro;

    public static float bulletHeight = 2.1f;

    public EnemyHealth enemyHealth;

    NavMeshAgent agent;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        isAggro = false;

        enemyHealth = GetComponent<EnemyHealth>();

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
            if (enemyHealth.isGettingHit) {
                fishLookAt();
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
        }
    }

    void FollowPlayer() {
        // handle navmesh agent
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);

        //swim towards player
        /*Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        ); */

        // turn to face player
        fishLookAt();

        //correct y-position
        transform.position = new Vector3(transform.position.x, bulletHeight, transform.position.z);

        //transform.position = newPosition;
    }

    void DamagePlayer(Collider other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(damageAmount);
        var playerController = other.GetComponent<PlayerController>();
        
        Vector3 moveDirection = (other.gameObject.transform.position - transform.position).normalized;

        AudioSource.PlayClipAtPoint(playerHitSFX, Camera.main.transform.position);

        float knockbackTimer = 0.75f;
        while (knockbackTimer >= 0)
        {
            playerController.controller.Move(moveDirection * Time.deltaTime * 5);
            knockbackTimer -= Time.deltaTime;
        }
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
