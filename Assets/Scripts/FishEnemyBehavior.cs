using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemyBehavior : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public AudioClip hurtSFX, deathSFX;

    public float detectionRadius = 10f;

    bool isAggro, isDead;

    public float maxHealth = 3f;

    float currentHealth;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        isAggro = false;
        isDead = false;

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver && !isDead)
        {
            //if player is close to enemy, it goes aggro
            if (Vector3.Distance(transform.position, player.position) <= detectionRadius) {
                isAggro = true;
            }

            //if enemy is activated, follow player
            if (isAggro) {
                FollowPlayer();
            }


            
        }

        
    }
    void FollowPlayer() {
        // turn to face player
        fishLookAt();
        
        //swim towards player
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void fishLookAt()
    {
        transform.LookAt(player);
        transform.Rotate(0,90,0);

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //get hurt
            AudioSource.PlayClipAtPoint(
                hurtSFX,
                Camera.main.transform.position
            );
            currentHealth -= 1;

            //destroy bullet
            Destroy(other.gameObject);
            
            //check whether enemy dies
            if (currentHealth <= 0) {
                Die();
            }
        }
    }

    private void Die()
    {

        isDead = true;
        GetComponent<MeshCollider>().enabled = false;
        AudioSource.PlayClipAtPoint(
            hurtSFX,
            Camera.main.transform.position
        );
        gameObject.GetComponent<Animator>().SetTrigger("fishDead");
        Destroy(gameObject, 0.75f);

    }
}
