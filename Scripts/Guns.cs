using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;

    private ObjectPooler objectPooler;
    private int duration = 10;
    private float timeBetweenShots = 0.3f;
    private int numGuns = 2;
    private GameObject gun1;
    private GameObject gun2;
    private SpriteRenderer[] spriteRenderers;

    private void Awake()
    {
        objectPooler = FindObjectOfType<ObjectPooler>();
        gun1 = transform.GetChild(0).gameObject;
        gun2 = transform.GetChild(1).gameObject;
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(ActivatePickup());
    }

    private IEnumerator ActivatePickup()
    {
        for (int i = 0; i < duration / (numGuns * timeBetweenShots); i++)
        {
            yield return new WaitForSeconds(timeBetweenShots);
            objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.bullet, 0, gun1.transform.position, Vector3.zero);
            yield return new WaitForSeconds(timeBetweenShots);
            objectPooler.SpawnPooledObject(ObjectPooler.PooledObjectType.bullet, 0, gun2.transform.position, Vector3.zero);
        }

        yield return new WaitForSeconds(timeBetweenShots);
        gameObject.SetActive(false);
    }

    public void ToggleGunsVisible()
    {
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.enabled = !spriteRenderer.enabled;
    }
}
