using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float timeToLive = 3f;
    public float initialY;

    public float damage = 1;

    void Start()
    {
        StartCoroutine(DestroyBullet());
        initialY = transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, initialY, transform.position.z);

        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 0f;
        rotation.z = 0f;
        
        transform.rotation = Quaternion.Euler(rotation);
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Collision hit!" + collision.gameObject.name);
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
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
    }
}
