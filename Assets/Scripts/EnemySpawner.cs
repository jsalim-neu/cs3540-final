using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnTime = 3;

    public float xMin = 0;
    public float xMax = 0;
    public float yMin = 0;
    public float yMax = 0;
    public float zMin = 0;
    public float zMax = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemies", spawnTime, spawnTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnEnemies()
    {
        Vector3 enemyPosition;

        enemyPosition.x = Random.Range(xMin, xMax);
        enemyPosition.y = Random.Range(yMin, yMax);
        enemyPosition.z = Random.Range(zMin, zMax);

        GameObject spawnedEnemy = Instantiate(enemyPrefab, enemyPosition, transform.rotation)
            as GameObject;

        spawnedEnemy.transform.parent = gameObject.transform;
    }
}
