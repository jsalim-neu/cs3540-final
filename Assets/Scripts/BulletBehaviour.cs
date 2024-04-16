using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    //how long the bullet lasts without 
    public float timeToLive = 3f;

    //initial height of the bullet, used for height correction of both the bullet and enemies
    public float initialY;

    public int damage = 1;

    public bool canDamagePlayer = false;

    public GameObject sparksPrefab;

    void Start()
    {
        StartCoroutine(DestroyBullet());
        initialY = transform.position.y;
    }

    void Update()
    {
        //bullet moves forward, and has its height and rotation continually corrected
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);

        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0f;
        rotation.z = 0f;
        
        transform.rotation = Quaternion.Euler(rotation);
    }

    //using a Coroutine to destroy the bullet
    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        //destroy bullet if it hits target, deactivate if it hits a non-target entity that ISN'T what fired it
        if (canDamagePlayer && !LevelManager.isGameOver)
        {
            //fired by enemy
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
            if (collision.gameObject.tag != "Enemy")
            {
                Destroy(gameObject);
            }
        }
        else
        {
            //fired by player
            if (collision.gameObject.tag == "Enemy")
            {
                Destroy(gameObject);
            }
            else if (collision.gameObject.tag != "Player")
            {
                gameObject.SetActive(false);
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        //bullet destroys itself if it hits an enemy, deactivates otherwise
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
        }
        else if (other.tag != "Player")
        {
            gameObject.SetActive(false);
            //instantiate spark particle effect?
        }
        
    }

    void OnDestroy()
    {
        //add particle effect for bullet hit
        if (sparksPrefab != null)
        {
            GameObject sparks = Instantiate(sparksPrefab, transform.position, Quaternion.identity);
            sparks.transform.SetParent(
                GameObject.FindGameObjectWithTag("ParticleParent").transform
            );
        }
    }

}
