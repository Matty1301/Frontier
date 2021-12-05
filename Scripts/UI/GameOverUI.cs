using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button restartButton, leaderboardButton, mainMenuButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject newHighScoreText;
    private GameManager gameManager;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.restart));
        leaderboardButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.leaderboard));
        mainMenuButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.mainMenu));
    }

    private void Start()
    {;
        gameManager = GameManager.Instance;

        scoreText.text = "Score:\n" + gameManager.finalScore.ToString();
        if (gameManager.finalScore > gameManager.prevHighScore)
        {
            newHighScoreText.SetActive(true);
            gameManager.prevHighScore = gameManager.highScore;
        }
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        leaderboardButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
    }
}
