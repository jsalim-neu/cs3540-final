using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    //allows for double-value coins
    public float scoreValue = 1f;
    public AudioClip pickupSFX;
    Collider collider;
    
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<SphereCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        //if player triggers coin, destroy coin and add to player money
        if (other.CompareTag("Player")) 
        {
            collider.enabled = false;

            gameObject.GetComponent<Animator>().SetTrigger("coinCollected");

            LevelManager.money += scoreValue;

            LevelManager.currObjective.ObjectiveUpdate(ObjectiveType.MONEY, (int)scoreValue);
            
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position);

            
            Destroy(gameObject, 0.7f);

        }
    }
}
