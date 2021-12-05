using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public static UnityEngine.Events.UnityAction buttonClicked;

    private GameObject backgroundPanel;
    private GameObject pauseMenu;
    private GameObject optionsMenu;
    [SerializeField] private GameObject signInButttons;
    private GameObject signOutConfirmationBox;
    private GameManager gameManager;
    private SoundManager soundManager;
    private PlayGamesPlatformManager playGamesPlatformManager;

    public enum MyButton
    {
        play,
        leaderboard,
        options,
        quit,
        pause,
        resume,
        mainMenu,
        back,
        restart,
        mute,
        signIn,
        signOut,
        signOutConfirm,
        signOutDeny,
    }

    private void OnEnable()
    {
        GameManager.gameStateChanged += OnGameStateChanged;
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void Start()
    {
        backgroundPanel = transform.Find("Canvas/BackgroundPanel").gameObject;
        pauseMenu = transform.Find("Canvas/SafeAreaPanel/PauseMenu").gameObject;
        optionsMenu = transform.Find("Canvas/SafeAreaPanel/OptionsMenu").gameObject;
        signOutConfirmationBox = transform.Find("Canvas/SafeAreaPanel/SignOutConfirmationBox").gameObject;
        gameManager = GameManager.Instance;
        soundManager = SoundManager.Instance;
        playGamesPlatformManager = PlayGamesPlatformManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (signOutConfirmationBox.activeInHierarchy)
                OnButtonClicked(MyButton.signOutDeny);
            else if (optionsMenu.activeInHierarchy)
                OnButtonClicked(MyButton.back);
            else if (pauseMenu.activeInHierarchy)
                OnButtonClicked(MyButton.resume);
            else if (gameManager.currentGameState == GameManager.GameState.Running)
                OnButtonClicked(MyButton.pause);
        }
    }

    private void OnGameStateChanged(GameManager.GameState newGameState)
    {
        pauseMenu.SetActive(newGameState == GameManager.GameState.Paused);
        backgroundPanel.SetActive(newGameState == GameManager.GameState.Paused);
    }

    private void OnSceneChanged(Scene newScene, LoadSceneMode unused)
    {
        signInButttons.SetActive(newScene != SceneManager.GetSceneByName("Main"));
    }

    public void OnButtonClicked(MyButton button)
    {
        buttonClicked?.Invoke();    //If the event has at least one subscriber invoke the event

        switch (button)
        {
            case MyButton.play:
                StartCoroutine(gameManager.StartGame());
                break;
            case MyButton.leaderboard:
                playGamesPlatformManager.TryShowLeaderboard();
                break;
            case MyButton.options:
                optionsMenu.SetActive(true);
                backgroundPanel.SetActive(true);
                break;
            case MyButton.quit:
                Application.Quit();
                break;
            case MyButton.pause:
                gameManager.TogglePause();
                break;
            case MyButton.resume:
                gameManager.TogglePause();
                break;
            case MyButton.mainMenu:
                gameManager.ReturnToMainMenu();
                break;
            case MyButton.back:
                optionsMenu.SetActive(false);
                if (!pauseMenu.activeInHierarchy)
                    backgroundPanel.SetActive(false);
                break;
            case MyButton.restart:
                StartCoroutine(gameManager.StartGame());
                break;
            case MyButton.mute:
                soundManager.ToggleMute();
                break;
            case MyButton.signIn:
                playGamesPlatformManager.ToggleSignedInStatus();
                break;
            case MyButton.signOut:
                signOutConfirmationBox.SetActive(true);
                backgroundPanel.SetActive(true);
                break;
            case MyButton.signOutConfirm:
                playGamesPlatformManager.ToggleSignedInStatus();
                signOutConfirmationBox.SetActive(false);
                if (!optionsMenu.activeInHierarchy && !pauseMenu.activeInHierarchy)
                    backgroundPanel.SetActive(false);
                break;
            case MyButton.signOutDeny:
                signOutConfirmationBox.SetActive(false);
                if (!optionsMenu.activeInHierarchy && !pauseMenu.activeInHierarchy)
                    backgroundPanel.SetActive(false);
                break;
        }
    }

    private void OnDisable()
    {
        GameManager.gameStateChanged -= OnGameStateChanged;
        SceneManager.sceneLoaded -= OnSceneChanged;
    }
}
