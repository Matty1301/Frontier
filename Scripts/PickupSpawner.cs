using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;
    private float gameBoundX;
    private float gameBoundY;
    private float playerBoundX;

    private void Start()
    {
        objectPooler = FindObjectOfType<ObjectPooler>();
        gameBoundX = GameManager.Instance.gameBoundX;
        gameBoundY = GameManager.Instance.gameBoundY;
        playerBoundX = gameBoundX - 0.25f;

        StartCoroutine(SpawnPickupDelay());
    }

    private IEnumerator SpawnPickupDelay()
    {
        int delay = Random.Range(20, 40);
        yield return new WaitForSeconds(delay);
        SpawnPickup();
        StartCoroutine(SpawnPickupDelay());
    }

    private void SpawnPickup()
    {
        int prefabIndex = Random.Range(0, objectPooler.pickupPrefabs.Length);
        Vector2 spawnPoint = new Vector2(Random.Range(-playerBoundX, playerBoundX),
            gameBoundY + 1);    //Pad 1 to ensure objects are spawned fully off screen

        objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.Pickup,
            prefabIndex, spawnPoint, Vector3.zero);
    }
}
