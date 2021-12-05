using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    private float gameBoundX;
    private float gameBoundY;
    private float timeOfLastDifficultyIncrease;
    private int timeToIncreaseDifficulty = 30;
    private float spawnDelay = 0.65f;    //Starting spawn delay in seconds
    private float minSpawnDelay = 0.25f;

    private void Start()
    {
        objectPooler = FindObjectOfType<ObjectPooler>();
        gameBoundX = GameManager.Instance.gameBoundX;
        gameBoundY = GameManager.Instance.gameBoundY;
        timeOfLastDifficultyIncrease = Time.time;

        InvokeRepeating("SpawnAsteroid", 0, spawnDelay);
    }

    private void SpawnAsteroid()
    {
        int prefabIndex = Random.Range(0, objectPooler.asteroidPrefabs.Length);
        Vector2 spawnPoint = new Vector2(Random.Range(-gameBoundX, gameBoundX),
            gameBoundY + 1);    //Pad 1 to ensure objects are spawned fully off screen
        Vector3 spawnRotation = new Vector3(0, 0, Random.Range(0, 360));

        objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.Asteroid,
            prefabIndex, spawnPoint, spawnRotation);
    }

    private void Update()
    {
        if (Time.time >= timeOfLastDifficultyIncrease + timeToIncreaseDifficulty)
        {
            IncreaseSpawnRate();
            timeOfLastDifficultyIncrease = Time.time;
        }
    }

    private void IncreaseSpawnRate()
    {
        CancelInvoke();
        if (spawnDelay > minSpawnDelay)
            spawnDelay -= 0.1f;
        InvokeRepeating("SpawnAsteroid", 0, spawnDelay);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
