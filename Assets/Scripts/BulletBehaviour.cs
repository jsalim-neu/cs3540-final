using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    //how long the bullet lasts without 
    public float timeToLive = 3f;

    //initial height of the bullet, used for height correction of both the bullet and enemies
    public float initialY;

    public float damage = 1;

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
        //bullet destroys itself if it hits an enemy, deactivates otherwise
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.tag != "Player")
        {
            gameObject.SetActive(false);
            //instantiate spark particle effect?
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

}
