using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public GameObject[] asteroidPrefabs, planetPrefabs, planetMaskPrefabs, starPrefabs, pickupPrefabs, bulletPrefabs;

    public enum PooledObjectType
    {
        Asteroid,
        Planet,
        PlanetMask,
        Star,
        Pickup,
        bullet,
    }

    private struct PooledObject
    {
        public GameObject m_GameObject;
        public int m_PrefabIndex;
        public PooledObjectController m_Controller;
    }

    private List<PooledObject> asteroidPool = new List<PooledObject>();
    private List<PooledObject> planetPool = new List<PooledObject>();
    private List<PooledObject> planetMaskPool = new List<PooledObject>();
    private List<PooledObject> starPool = new List<PooledObject>();
    private List<PooledObject> pickupPool = new List<PooledObject>();
    private List<PooledObject> bulletPool = new List<PooledObject>();

    public GameObject SpawnPooledObject(PooledObjectType objectType, int prefabIndex, Vector3 spawnPoint, Vector3 spawnRotation)
    {
        List<PooledObject> objectPool = null;
        GameObject[] objectPrefabs = null;
        Transform objectParent = null;
        switch (objectType)
        {
            case PooledObjectType.Asteroid:
                objectPool = asteroidPool;
                objectPrefabs = asteroidPrefabs;
                objectParent = transform.Find("AsteroidPool");
                break;
            case PooledObjectType.Planet:
                objectPool = planetPool;
                objectPrefabs = planetPrefabs;
                objectParent = transform.Find("PlanetPool");
                break;
            case PooledObjectType.PlanetMask:
                objectPool = planetMaskPool;
                objectPrefabs = planetMaskPrefabs;
                objectParent = transform.Find("PlanetMaskPool");
                break;
            case PooledObjectType.Star:
                objectPool = starPool;
                objectPrefabs = starPrefabs;
                objectParent = transform.Find("StarPool");
                break;
            case PooledObjectType.Pickup:
                objectPool = pickupPool;
                objectPrefabs = pickupPrefabs;
                objectParent = transform.Find("PickupPool");
                break;
            case PooledObjectType.bullet:
                objectPool = bulletPool;
                objectPrefabs = bulletPrefabs;
                objectParent = transform.Find("BulletPool");
                break;
        }

        PooledObject pooledObject;

        //Search through the object pool for an object of the correct type that is free
        //If one is found set it to be the spawned object then return out of the function
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (objectPool[i].m_PrefabIndex == prefabIndex && objectPool[i].m_GameObject.activeSelf == false)
            {
                pooledObject = objectPool[i];
                pooledObject.m_GameObject.transform.position = spawnPoint;
                pooledObject.m_GameObject.transform.rotation = Quaternion.Euler(spawnRotation);
                pooledObject.m_GameObject.SetActive(true);
                if (pooledObject.m_Controller != null)
                    pooledObject.m_Controller.OnSpawnObject();
                return pooledObject.m_GameObject;
            }
        }

        //If there is no suitable object in the pool instantiate a new object and add it to the pool
        GameObject gameObject = (Instantiate(objectPrefabs[prefabIndex], spawnPoint, Quaternion.Euler(spawnRotation), objectParent));

        pooledObject = new PooledObject
        {
            m_GameObject = gameObject,
            m_PrefabIndex = prefabIndex,
            m_Controller = gameObject.GetComponent<PooledObjectController>()
        };

        objectPool.Add(pooledObject);
        return pooledObject.m_GameObject;
    }
}
