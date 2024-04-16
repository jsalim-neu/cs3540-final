using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Animator anim;
    
    public AudioClip hurtSFX, deathSFX;
    public Slider healthSlider;

    EnemyFSM fsm;

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
        fsm = GetComponent<EnemyFSM>();


        isDead = false;
        isGettingHit = false;
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

        //stop movement
        agent.isStopped = true;

        //destroy bullet
        Destroy(other);


        //check whether enemy dies
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            //play hit animation
            anim.SetTrigger("fishHit");
            
        }
        yield return new WaitForSeconds(0.3f);
        isGettingHit = false;
        //restart movement
        if (currentHealth > 0)
        {
            agent.isStopped = false;
        }
    }

    private void Die()
    {
        isDead = true;
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = false;
        }
        AudioSource.PlayClipAtPoint(
            deathSFX,
            Camera.main.transform.position
        );
        fsm.currentState = FSMStates.Dead;
        anim.SetTrigger("fishDead");

        //check whether item (e.g. coin) is dropped
        System.Random r = new System.Random();
        if (r.NextDouble() <= dropChance)
        {
            DropItem();
        }
        Destroy(gameObject, 1.25f);


    }

    private void DropItem()
    {
        GameObject.Instantiate(itemDrop, transform.position, Quaternion.identity, itemParent.transform);
    }
}
