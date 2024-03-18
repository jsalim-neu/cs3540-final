using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public AudioClip hurtSFX, deathSFX;
    public Slider healthSlider;

    // health/state handler vars

    bool isDead;
        
    public bool isGettingHit;

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

        isDead = false;
        isGettingHit = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
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
        healthSlider.value = currentHealth;

        //destroy bullet
        Destroy(other.gameObject);

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
