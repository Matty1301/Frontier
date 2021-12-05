using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : Singleton<PlayerSpawnManager>
{
    private GameObject player;
    private PlayerController playerController;
    public bool isRespawning { get; private set; }

    private void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        yield return new WaitForSeconds(1);
        player.transform.position = new Vector2(0, -2); //Reset the players position
        player.SetActive(true);
        //StartCoroutine(playerController.Invulnerability(2.0f, () => isRespawning = false)); //Once the Invulnerability() coroutine has finished
            //the callback function; which is defined here as a lambda expression, is called
            //which sets isRespawning to false
    }

    //Start coroutine here to ensure it isn't stopped when player is deactivated
    public void StartRespawnCoroutine()
    {
        StartCoroutine(Respawn());
    }
}
