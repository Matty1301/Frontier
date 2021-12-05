using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, PooledObjectController
{
    public static UnityEngine.Events.UnityAction bulletFired;

    private float gameBoundX;
    private float gameBoundY;

    private void Start()
    {
        gameBoundX = GameManager.Instance.gameBoundX;
        gameBoundY = GameManager.Instance.gameBoundY;

        OnSpawnObject();
    }

    public void OnSpawnObject()
    {
        bulletFired?.Invoke();
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameManager.GameState.Running)
            transform.Translate(new Vector2(0, 0.05f));

        if (transform.position.y > gameBoundY)
            gameObject.SetActive(false);
    }
}
