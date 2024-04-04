using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public AudioClip hurtSFX, deathSFX;
    public Slider healthSlider;

    UnityEngine.AI.NavMeshAgent agent;

    Transform enemyHUD_Transform;

    // health/state handler vars

    public bool isGettingHit, isDead;
    public float maxHealth = 3f;

    float currentHealth;

    // item drop vars

    public GameObject itemDrop;
    GameObject itemParent;
    public float dropChance = 1f;

    void Awake()
    {
        healthSlider = GetComponentInChildren<Slider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.value = currentHealth;
        itemParent = GameObject.FindGameObjectWithTag("PickupParent");
        enemyHUD_Transform = transform.GetChild(0);
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();


        isDead = false;
        isGettingHit = false;
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            StartCoroutine(GetHit(other.collider));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet") || other.CompareTag("Explosion"))
        {
            StartCoroutine(GetHit(other));
        }
        else if (other.CompareTag("ForceField"))
        {
            Die();
        }
    }

    private IEnumerator GetHit(Collider other)
    {
        if (other.CompareTag("Bullet")) {
            var damageScript = other.gameObject.GetComponent<BulletBehaviour>();
            currentHealth -= damageScript.damage;
        }
        else {
            var damageScript = other.gameObject.GetComponent<ExplosionBehavior>();
            currentHealth -= damageScript.damage;
        }

        //get hurt
        isGettingHit = true;
        AudioSource.PlayClipAtPoint(
            hurtSFX,
            Camera.main.transform.position
        );
        healthSlider.value = currentHealth;

        //destroy bullet
        Destroy(other);
        //stop movement
        agent.isStopped = true;

        //check whether enemy dies
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //play hit animation
            gameObject.GetComponent<Animator>().SetTrigger("fishHit");
            
        }
        yield return new WaitForSeconds(0.3f);
        isGettingHit = false;
        //restart movement
        agent.isStopped = false;
    }

    private void Die()
    {
        isDead = true;
        GetComponent<Collider>().enabled = false;
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
        GameObject.Instantiate(itemDrop, transform.position, Quaternion.identity, itemParent.transform);
    }
}
