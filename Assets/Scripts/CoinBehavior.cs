using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float scoreValue = 1f;
    public AudioClip pickupSFX;
    public static int pickupCount = 0;


    // Start is called before the first frame update
    void Start()
    {
        pickupCount++;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LevelManager.isGameOver) 
        {
            pickupCount = 0;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {

            gameObject.GetComponent<Animator>().SetTrigger("coinCollected");

            LevelManager.score += scoreValue;


            Debug.Log("SCORE: " + LevelManager.score);
            
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position);

            
            Destroy(gameObject, 0.7f);

        }
    }
}
