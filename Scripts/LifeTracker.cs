using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTracker : MonoBehaviour
{
    public int livesRemaining { get; private set; }
    private GameObject[] lifeIcons;

    private void Start()
    {
        //Define an array so that GetChild doesn't need to be called everytime lives remaining changes
        lifeIcons = new GameObject[transform.childCount];
        for (int i = 0; i < lifeIcons.Length; ++i)
        {
            lifeIcons[i] = transform.GetChild(i).gameObject;
        }
    }

    public void SetStartingLives(int startingLives = 0) //Allow statrting lives to be passed in as may want to add other difficulty levels later
    {
        livesRemaining = startingLives;

        //Set life icons active for starting lives
        for (int j = 0; j < startingLives; ++j)
        {
            lifeIcons[j].SetActive(true);
        }
    }

    public void GainLife()
    {
        if (livesRemaining < lifeIcons.Length)
        {
            livesRemaining++;
            lifeIcons[livesRemaining - 1].SetActive(true);  //Subtract 1 because lifeIcons[] starts at index 0
        }
        else
        {
            //Debug.Log("You already have the maximum number of lives");
        }
    }

    public void LoseLife()
    {
        lifeIcons[livesRemaining - 1].SetActive(false); //Subtract 1 because lifeIcons[] starts at index 0
        livesRemaining--;
    }
}
