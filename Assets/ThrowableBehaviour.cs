using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableBehaviour : MonoBehaviour
{
    public enum ThrowableType
    {
        GRENADE,
        MINE,
        TEAR_GAS
    }

    public float radius = 5.0F;
    public float damage = 10.0F;
    public float fuseTime = 3.0F;
    public GameObject triggeredPrefab;
    public ThrowableType thType;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // apply a downward force to the object
        rb.AddForce(Vector3.down * 10);
    }

    public void TriggerThrowable()
    {
        switch (thType)
        {
            case ThrowableType.GRENADE:
                Invoke("Explode", fuseTime);
                break;
            case ThrowableType.MINE:
                Invoke("Explode", 0);
                break;
            case ThrowableType.TEAR_GAS:
                break;
        }
    }

    void Explode()
    {
        GameObject explosion = Instantiate(triggeredPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
        explosion.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        
        // freeze the object
        Rigidbody explosionRb = explosion.GetComponent<Rigidbody>();
        explosionRb.constraints = RigidbodyConstraints.FreezeAll;

        Destroy(explosion, .1f);
    }
}
