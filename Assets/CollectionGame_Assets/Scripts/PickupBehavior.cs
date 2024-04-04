using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        if (LevelManager.isGameOver)
        {
            LevelManager.pickUpCount--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!LevelManager.isGameOver)
        {
            LevelManager.pickUpCount--;
            if (LevelManager.pickUpCount <= 0)
            {
                FindObjectOfType<LevelManager>().LevelBeat();
            }
        }
    }
}
