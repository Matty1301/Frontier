using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour
{
    public int score { get; private set; }   //Max value for int32: 2147483647
    private Text scoreText;
    private PlayerController playerController;

    private int scoreIncrement = 10;
    private float _IncrementScoreRepeatTime = 0.5f;

    private void Start()
    {
        scoreText = GetComponent<Text>();
        playerController = FindObjectOfType<PlayerController>();
        InvokeRepeating("IncrementScore", _IncrementScoreRepeatTime, _IncrementScoreRepeatTime);
    }

    private void IncrementScore()
    {
        if (!playerController.isRespawning)
        {
            score += scoreIncrement;
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void StopIncrementScore()
    {
        CancelInvoke();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
