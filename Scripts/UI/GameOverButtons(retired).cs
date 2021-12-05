using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverButtons : MonoBehaviour
{
    [SerializeField]
    private Button restartButton, mainMenuButton;  //Multiple declaration to avoid repeating SerializeField

    private void OnEnable()
    {
        restartButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.restart));
        mainMenuButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.mainMenu));
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
    }
}
