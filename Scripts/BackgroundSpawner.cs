using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject star, starFlare, dustWisp;

    private ObjectPooler objectPooler;
    private float gameBoundX;
    private float gameBoundY;
    private float outerBoundX;
    private float backgroundHeight = 20.48f;    //Height of the background exculding overlap

    private int planetRadius = 1;
    private int starRadius = 4;
    private int dustWispRadius = 10;

    private void Start()
    {
        objectPooler = FindObjectOfType<ObjectPooler>();
        gameBoundX = GameManager.Instance.gameBoundX;
        gameBoundY = GameManager.Instance.gameBoundY;
        outerBoundX = gameBoundX + (starRadius / 2);
    }

    //Spawn a random planet and a random planet mask at the same position
    public void SpawnPlanet()
    {
        int planetPrefabIndex = Random.Range(0, objectPooler.planetPrefabs.Length);
        Vector2 planetSpawnPoint = new Vector2(Random.Range(-gameBoundX, gameBoundX),
            Random.Range(gameBoundY, gameBoundY + backgroundHeight) + planetRadius);   //Pad with object radius
                                                                                       //to ensure objects are spawned fully off screen
        Vector3 planetSpawnRotation = new Vector3(0, 0, Random.Range(0, 360));

        objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.Planet, planetPrefabIndex, planetSpawnPoint, planetSpawnRotation);

        int planetMaskPrefabIndex = Random.Range(0, objectPooler.planetMaskPrefabs.Length);
        Vector3 planetMaskSpawnRotation;
        if (Random.Range(0, 2) == 0)    //Create a random number between 0 and 1
            planetMaskSpawnRotation = new Vector3(0, 0, Random.Range(150, 210));
        else
            planetMaskSpawnRotation = new Vector3(0, 0, Random.Range(-30, 30));

        objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.PlanetMask, planetMaskPrefabIndex, planetSpawnPoint, planetMaskSpawnRotation);
    }

    public void SpawnStar()
    {
        star.transform.position = new Vector2(Random.Range(-outerBoundX, outerBoundX),
            Random.Range(gameBoundY, gameBoundY + backgroundHeight) + starRadius);
        star.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        star.SetActive(true);

        /*
        int starPrefabIndex = Random.Range(0, objectPooler.starPrefabs.Length);
        Vector3 starSpawnPoint = new Vector3(Random.Range(-outerBoundX, outerBoundX),
            Random.Range(gameBoundY, gameBoundY + backgroundHeight) + starRadius, -10);   //Positioned on the z-plane of the camera
                                                                                          //because of the 3D audio source
        Vector3 starSpawnRotation = new Vector3(0, 0, Random.Range(0, 360));

        objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.Star, starPrefabIndex, starSpawnPoint, starSpawnRotation);
        */
    }

    public void SpawnStarFlare()
    {
        starFlare.transform.position = new Vector2(Random.Range(-gameBoundX, gameBoundX),
            Random.Range(gameBoundY, gameBoundY + backgroundHeight) + starRadius);
        starFlare.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        starFlare.SetActive(true);
    }

    public void SpawnDustWisp()
    {
        dustWisp.transform.position = new Vector2(Random.Range(-gameBoundX, gameBoundX),
            Random.Range(gameBoundY, gameBoundY + backgroundHeight) + dustWispRadius);
        dustWisp.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        dustWisp.SetActive(true);
    }
}
