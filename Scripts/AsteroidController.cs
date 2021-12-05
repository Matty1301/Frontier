using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidController : MonoBehaviour, PooledObjectController
{
    public static UnityEngine.Events.UnityAction asteroidDestroyed;

    [SerializeField]
    private GameObject explosion;

    private Rigidbody2D rigidBody;
    private Collider2D playerForceFieldCollider;
    private Vector2 directionAwayFromForceField;
    private bool inForceField;
    private float forceFieldStrength = 0.2f;

    private float gameBoundX;
    private float gameBoundY;

    private float velocityMax = 0.5f;
    private int hitCountMax = 3;
    private int hitCount;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerForceFieldCollider = GameObject.Find("Player").transform.Find("ForceField").GetComponent<Collider2D>();

        gameBoundX = GameManager.Instance.gameBoundX;
        gameBoundY = GameManager.Instance.gameBoundY;

        OnSpawnObject();
    }

    public void OnSpawnObject()
    {
        rigidBody.velocity = new Vector2(Random.Range(-velocityMax, velocityMax), Random.Range(-velocityMax, velocityMax));
        rigidBody.angularVelocity = Random.Range(-360, 360);

        hitCount = 0;
    }

    private void Update()
    {
        //Once object has gone fully off screen deactivate it so that it can be reused
        if (Mathf.Abs(transform.position.x) > gameBoundX + 1
            || Mathf.Abs(transform.position.y) > gameBoundY + 1)    //Pad 1 to ensure objects are fully off screen before being deactivated
            gameObject.SetActive(false);
    }

    private void FixedUpdate()  //Fixed update used for physics simulations
    {
        //If this object is in the force field launch it away from the force field
        if (inForceField == true)
        {
            directionAwayFromForceField = (transform.position - playerForceFieldCollider.transform.position).normalized;
            rigidBody.AddForce(directionAwayFromForceField * forceFieldStrength, ForceMode2D.Impulse);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == playerForceFieldCollider)
        {
            inForceField = true;
        }

        else if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            hitCount++;
            collision.gameObject.SetActive(false);
            if (hitCount >= hitCountMax)
            {
                asteroidDestroyed?.Invoke();
                Instantiate(explosion, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == playerForceFieldCollider)
        {
            inForceField = false;
        }
    }
}
