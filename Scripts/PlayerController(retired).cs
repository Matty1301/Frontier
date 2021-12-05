using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerOld : MonoBehaviour
{
    public static UnityEngine.Events.UnityAction playerCrashed;

    private Rigidbody2D rigidBody;
    private new PolygonCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem trail;
    private ParticleSystemRenderer trailRenderer;
    private PlayerSpawnManager playerSpawnManager;
    private LifeTracker lifeTracker;

    private float gameBoundX;
    private float playerBoundX;

    private Touch theTouch;
    private float deltaPosX;
    private Vector3 mousePosition;
    private int fingerIdLast = -1;

    [SerializeField]
    private GameObject explosion;

    public bool isRespawning { get; private set; }
    private bool invulnerable = false;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponentInChildren<ParticleSystem>();
        trailRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        playerSpawnManager = FindObjectOfType<PlayerSpawnManager>();
        lifeTracker = FindObjectOfType<LifeTracker>();

        gameBoundX = GameManager.Instance.gameBoundX;
        playerBoundX = gameBoundX - 0.25f;
    }

    private void StopAtXPos(float XPos)
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;
        transform.position = new Vector2(XPos, transform.position.y);
    }

    private void FixedUpdate()  //FixedUpdate has a constant deltaTime, and so is preferred for physics (rigidbody) calculations
    {
        //Keep player within screen bounds
        if (transform.position.x > playerBoundX)
        {
            StopAtXPos(playerBoundX);
        }
        if (transform.position.x < -playerBoundX)
        {
            StopAtXPos(-playerBoundX);
        }

#if UNITY_EDITOR
        //Mouse movement controls
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            deltaPosX = mousePosition.x - transform.position.x;

            rigidBody.AddForce(Vector2.right * deltaPosX * Time.deltaTime, ForceMode2D.Impulse);
        }

#else
        //Touch movement controls
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            //If the touch is outside the game bounds do nothing
            if (Mathf.Abs(Camera.main.ScreenToWorldPoint(theTouch.position).x) > gameBoundX)
                return;

            deltaPosX = theTouch.deltaPosition.x;

            if (theTouch.fingerId != fingerIdLast)
            {
                fingerIdLast = theTouch.fingerId;
            }

            //If the touch is a drag and it didn't move more than 200 pixels from the last position
            else if ((theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended) && Mathf.Abs(deltaPosX) <= 200)
            {
                rigidBody.AddForce(Vector2.right * deltaPosX * 0.5f * Time.deltaTime, ForceMode2D.Impulse); //0.5f is a speed scalar
            }
        }
#endif
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision = GetComponentInChildren<CircleCollider2D>())
            return;

        if (!invulnerable)
        {
            if (lifeTracker.livesRemaining > 0)
            {
                lifeTracker.LoseLife();
                StartCoroutine(Respawn());
            }
            else
            {
                StartCoroutine(GameManager.Instance.GameOver());
            }

            playerCrashed?.Invoke();
            Instantiate(explosion, transform.position, Quaternion.identity);

            collider.enabled = false;
            spriteRenderer.enabled = false;
            trail.Stop();
        }
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        yield return new WaitForSeconds(1);
        transform.position = new Vector2(0, -2); //Reset the players position

        collider.enabled = true;
        spriteRenderer.enabled = true;
        trail.Play();

        StartCoroutine(Invulnerability(2.0f, () => isRespawning = false)); //Once the Invulnerability() coroutine has finished
                                                                           //the callback function; defined here as a lambda expression, is called which sets isRespawning to false
    }

    private IEnumerator Invulnerability(float totalTime, System.Action callback = null)
    {
        float blinkDuration;    //blinkDuration is declared inside the coroutine
                                //so that a new instance will be created for each call to this coroutine
                                //to ensure that multipes of this coroutine running simultanesouly do not interfere with each other

        invulnerable = true;

        //Blink while invulnerable
        for (float timeRemaining = totalTime; timeRemaining > 0; timeRemaining -= blinkDuration)
        {
            blinkDuration = Mathf.Min(0.25f, timeRemaining);
            yield return new WaitForSeconds(blinkDuration);
            trailRenderer.enabled = !spriteRenderer.enabled;
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        invulnerable = false;

        //If a callback function has been provided call it here to notify the caller that the coroutine has finished
        if (callback != null)
            callback();
    }
}
