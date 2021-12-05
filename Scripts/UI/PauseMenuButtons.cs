using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuButtons : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton, optionsButton, mainMenuButton;  //Multiple declaration to avoid repeating SerializeField

    private void OnEnable()
    {
        resumeButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.resume));
        optionsButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.options));
        mainMenuButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.mainMenu));
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
    }
}
