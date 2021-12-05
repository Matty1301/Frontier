using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    private float outerBoundY;
    public float speedRelativeToParent = 3;

    private void Start()
    {
        //Pad bounds with largest object (dust wisp) radius to ensure all objects are fully off screen before being destroyed
        outerBoundY = GameManager.Instance.gameBoundY + 10;
    }

    void Update()
    {
        transform.Translate(Vector2.down * speedRelativeToParent * Time.deltaTime, Space.World);

        if (transform.position.y < -outerBoundY)
            gameObject.SetActive(false);
    }
}
