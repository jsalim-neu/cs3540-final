using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemyBehavior : MonoBehaviour
{

    //movement, player detection vars
    public Transform player;
    public float moveSpeed = 5f;

    public float detectionRadius = 10f;

    bool isAggro;

    public static float bulletHeight = 0;

    public EnemyHealth enemyHealth;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        isAggro = false;

        enemyHealth = GetComponent<EnemyHealth>();
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
        }
    }
    void FollowPlayer() {
        // turn to face player
        fishLookAt();
        
        //swim towards player
        Vector3 newPosition = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        //correct y-position
        newPosition.y = bulletHeight;

        transform.position = newPosition;
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
