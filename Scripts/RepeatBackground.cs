using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    private BackgroundSpawner backgroundSpawner;
    private Vector3 startPos;
    private float backgroundHeight = 20.48f;    //Height of the background exculding overlap
    private int counter = 6;

    private void Start()
    {
        backgroundSpawner = FindObjectOfType<BackgroundSpawner>();
        startPos = transform.position;
    }

    private void Update()
    {
        if (transform.position.y < startPos.y - backgroundHeight)
        {
            transform.position = startPos;  //Loop background
            backgroundSpawner.SpawnPlanet();
            counter++;
            if (counter % 6 == 0)
                backgroundSpawner.SpawnStar();
            if (counter % 5 == 0)
                backgroundSpawner.SpawnStarFlare();
            if (counter % 4 == 0)
                backgroundSpawner.SpawnDustWisp();
        }
    }
}
