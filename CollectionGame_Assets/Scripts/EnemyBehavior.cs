using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public AudioClip enemySFX;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!LevelManager.isGameOver)
        {

            // rotate

            // follow

        }

        RotateEnemy();
        FollowPlayer();
    }

    void RotateEnemy() {
        transform.Rotate(Vector3.forward, 360 * Time.deltaTime);
    }

    void FollowPlayer() {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(
                enemySFX,
                Camera.main.transform.position
            );
            Destroy(gameObject);
        }
    }
}
