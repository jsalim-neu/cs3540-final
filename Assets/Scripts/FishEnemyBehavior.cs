using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishEnemyBehavior : MonoBehaviour
{

    //movement, player detection vars
    public Transform player;
    public float moveSpeed = 5f;
    public AudioClip hurtSFX, deathSFX;

    public float detectionRadius = 10f;

    // health/state handler vars

    bool isAggro, isDead, isGettingHit;

    public float maxHealth = 3f;

    float currentHealth;

    // item drop vars

    public GameObject itemDrop, itemParent;
    public float dropChance = 1f;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        isAggro = false;
        isDead = false;
        isGettingHit = false;

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
            if (isAggro && !isGettingHit) {
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
            //if not aggro already, makes sure it begins to follow the player
            isAggro = true;


            StartCoroutine(GetHit(other));


        }
    }

    private IEnumerator GetHit(Collision other)
    {
        //get hurt
        isGettingHit = true;
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
        else {
            //play hit animation
            gameObject.GetComponent<Animator>().SetTrigger("fishHit");
        }
        yield return new WaitForSeconds(0.25f);
        isGettingHit = false;

    }

    private void Die()
    {

        isDead = true;
        GetComponent<MeshCollider>().enabled = false;
        AudioSource.PlayClipAtPoint(
            deathSFX,
            Camera.main.transform.position
        );
        gameObject.GetComponent<Animator>().SetTrigger("fishDead");

        //check whether item (e.g. coin) is dropped
        System.Random r = new System.Random();
        if (r.NextDouble() <= dropChance)
        {
            DropItem();
        }
        Destroy(gameObject, 0.75f);

    }

    private void DropItem()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("PickupParent");
        GameObject.Instantiate(itemDrop, transform.position, Quaternion.identity, parent.transform);

    }
}
