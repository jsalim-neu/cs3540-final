using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public LevelManager levelManager;
    public float scoreValue = 1f;
    public AudioClip pickupSFX;
    public static int pickupCount = 0;

    Collider collider;


    // Start is called before the first frame update
    void Start()
    {
        pickupCount++;
        collider = GetComponent<SphereCollider>();
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
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
            collider.enabled = false;

            gameObject.GetComponent<Animator>().SetTrigger("coinCollected");

            levelManager.money += scoreValue;

            levelManager.objective.ObjectiveUpdate(ObjectiveType.MONEY, (int)scoreValue);

            Debug.Log("SCORE: " + levelManager.money);
            
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position);

            
            Destroy(gameObject, 0.7f);

        }
    }
}
