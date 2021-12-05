using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton, leaderboardButton, optionsButton, quitButton;
    [SerializeField] private GameObject highScoreText;
    private int highScore;

    private void OnEnable()
    {
        playButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.play));
        leaderboardButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.leaderboard));
        optionsButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.options));
        quitButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.quit));
        GameManager.highScoreUpdated += UpdateHighScoreText;
    }

    private void Start()
    {
        UpdateHighScoreText();
    }

    private void UpdateHighScoreText()
    {
        highScore = GameManager.Instance.highScore;

        if (highScoreText.activeSelf == false && highScore > 0)
            highScoreText.SetActive(true);
        if (highScoreText.activeSelf == true && highScore == 0)
            highScoreText.SetActive(false);

        highScoreText.GetComponent<Text>().text = "High score:\n" + highScore.ToString();
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveAllListeners();
        leaderboardButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
        GameManager.highScoreUpdated -= UpdateHighScoreText;
    }
}
