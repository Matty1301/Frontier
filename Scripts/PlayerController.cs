using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static UnityEngine.Events.UnityAction playerCrashed;

    private Rigidbody2D rigidbody;
    private new PolygonCollider2D collider;
    private SpriteRenderer spriteRenderer;
    private ParticleSystem trail;
    private ParticleSystemRenderer trailRenderer;
    public GameObject forceField;
    public GameObject guns;
    private PlayerSpawnManager playerSpawnManager;
    private LifeTracker lifeTracker;

    private float gameBoundX;
    private float playerBoundX;

    private Vector2 mousePosition;
    private float distanceToTarget;
    private Vector2 newVelocity;

    private Touch theTouch;
    private int fingerIdLast = -1;
    private Vector2 touchPosLast;
    private float touchDeltaPosX;
    private Vector2 targetPos;

    [SerializeField]
    private GameObject explosion;

    private Vector2 directionAwayFromPlayer;

    public bool isRespawning { get; private set; }

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trail = GetComponentInChildren<ParticleSystem>();
        trailRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        forceField = transform.Find("ForceField").gameObject;
        guns = transform.Find("Guns").gameObject;
        playerSpawnManager = FindObjectOfType<PlayerSpawnManager>();
        lifeTracker = FindObjectOfType<LifeTracker>();

        gameBoundX = GameManager.Instance.gameBoundX;
        playerBoundX = gameBoundX - 0.25f;
    }

    private void StopAtXPos(float XPos)
    {
        newVelocity = Vector2.zero;
        transform.position = new Vector2(XPos, transform.position.y);
        targetPos.x = XPos;
    }

    private void Update()
    {
#if UNITY_EDITOR
        //Mouse movement controls
        if (Input.GetMouseButton(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            distanceToTarget = mousePosition.x - transform.position.x;
            newVelocity = Vector2.right * distanceToTarget * 2;
        }
#else
        //Touch movement controls
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            //If the touch is outside the game bounds do nothing
            if (Mathf.Abs(Camera.main.ScreenToWorldPoint(theTouch.position).x) > gameBoundX)
            {
                //Do nothing
            }

            else if (theTouch.fingerId != fingerIdLast)
            {
                fingerIdLast = theTouch.fingerId;
                newVelocity = Vector2.zero;
                targetPos.x = transform.position.x;
            }

            else if (theTouch.phase == TouchPhase.Began)
            {
                newVelocity = Vector2.zero;
                targetPos.x = transform.position.x;
            }

            else //if (theTouch.phase == TouchPhase.Moved || theTouch.phase == TouchPhase.Ended)
            {
                touchDeltaPosX = (Camera.main.ScreenToWorldPoint(theTouch.position) - Camera.main.ScreenToWorldPoint(touchPosLast)).x;
                targetPos.x += touchDeltaPosX * 2;
                distanceToTarget = targetPos.x - transform.position.x;
                newVelocity = Vector2.right * distanceToTarget * 2;
            }

            touchPosLast = theTouch.position;
        }
#endif

        //Keep player within screen bounds
        if (transform.position.x > playerBoundX)
            StopAtXPos(playerBoundX);
        else if (transform.position.x < -playerBoundX)
            StopAtXPos(-playerBoundX);
    }

    private void FixedUpdate()  //Apply rigidbody physics in fixed update
    {
        rigidbody.velocity = newVelocity;
        rigidbody.angularVelocity = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (forceField.activeInHierarchy == true)
            return;

        //Create explosion
        playerCrashed?.Invoke();    //Invoke the event to play a sound
        Instantiate(explosion, transform.position, Quaternion.identity);

        //Launch the asteroid away from the explosion
        directionAwayFromPlayer = (collision.transform.position - transform.position).normalized;
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(directionAwayFromPlayer * 1f, ForceMode2D.Impulse);

        //Hide player and disable collisions
        collider.enabled = false;
        spriteRenderer.enabled = false;
        trail.Stop();
        guns.SetActive(false);

        lifeTracker.LoseLife();

        if (lifeTracker.livesRemaining > 0)
            StartCoroutine(Respawn());
        else
            StartCoroutine(GameManager.Instance.GameOver());
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        yield return new WaitForSeconds(1);
        transform.position = new Vector2(0, -2); //Reset the players position
        targetPos = new Vector2(0, targetPos.y);

        //Show player and enable collisions
        collider.enabled = true;
        spriteRenderer.enabled = true;
        trail.Play();

        StartCoroutine(Invulnerability(2.0f, () => isRespawning = false)); //Start the Invulnerability coroutine passing in a callback function
            //to set isRespawning to false once the coroutine has finished executing
    }

    private IEnumerator Invulnerability(float totalTime, System.Action callback = null)
    {
        float blinkDuration;    //blinkDuration is declared inside the coroutine
                                //so that a new instance will be created for each call to this coroutine
                                //to ensure that multipes of this coroutine running simultanesouly do not interfere with each other

        collider.enabled = false;

        //Blink while invulnerable
        for (float timeRemaining = totalTime; timeRemaining > 0; timeRemaining -= blinkDuration)
        {
            blinkDuration = Mathf.Min(0.25f, timeRemaining);
            yield return new WaitForSeconds(blinkDuration);
            trailRenderer.enabled = !spriteRenderer.enabled;
            if (guns.activeInHierarchy)
                guns.GetComponent<Guns>().ToggleGunsVisible();
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }

        collider.enabled = true;

        //If a callback function has been provided call it here to notify the caller that the coroutine has finished
        if (callback != null)
            callback();
    }
}
