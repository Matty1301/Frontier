using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static UnityEngine.Events.UnityAction<GameState> gameStateChanged;
    public static UnityEngine.Events.UnityAction highScoreUpdated;

    private int gameWidth_Pixels;
    private int gameHeight_Pixels;
    public int upperGameBoundX_Pixels { get; private set; }
    public int lowerGameBoundX_Pixels { get; private set; }
    public int upperGameBoundY_Pixels { get; private set; }
    public int lowerGameBoundY_Pixels { get; private set; }
    public float gameBoundX { get; private set; }
    public float gameBoundY { get; private set; }

    public enum GameState
    {
        Pregame,
        Running,
        Paused,
        Postgame
    }
    public GameState currentGameState { get; private set; }
    public int finalScore { get; private set; }
    public int highScore { get; private set; }
    public int prevHighScore;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        SetGameBounds();
    }

    private void SetGameBounds()
    {
        //Use aspect ratio as separate ints to avoid rounding errors
        int aspectRatioWidth = 1080;
        int aspectRatioHeight = 2400;

        //If the screen width is too small then reduce the game height to maintain the aspect ratio
        if ((Screen.height * aspectRatioWidth) / aspectRatioHeight > Screen.width)
        {
            //Zoom the camera out so that the game width fits the screen width
            Camera.main.orthographicSize = 5 * ((float)Screen.height / gameHeight_Pixels);

            gameWidth_Pixels = Screen.width;
            gameHeight_Pixels = (Screen.width * aspectRatioHeight) / aspectRatioWidth;
        }
        else
        {
            gameWidth_Pixels = (Screen.height * aspectRatioWidth) / aspectRatioHeight;
            gameHeight_Pixels = Screen.height;
        }

        //Halving the screen width and height gives the position at the centre of the screen
        upperGameBoundX_Pixels = Screen.width / 2 + gameWidth_Pixels / 2;
        lowerGameBoundX_Pixels = Screen.width / 2 - gameWidth_Pixels / 2;
        upperGameBoundY_Pixels = Screen.height / 2 + gameHeight_Pixels / 2;
        lowerGameBoundY_Pixels = Screen.height / 2 - gameHeight_Pixels / 2;

        gameBoundX = Camera.main.ScreenToWorldPoint(new Vector2(upperGameBoundX_Pixels, 0)).x;
        gameBoundY = Camera.main.ScreenToWorldPoint(new Vector2(0, upperGameBoundY_Pixels)).y;
    }

    private void Start()
    {
        SaveFileManager.Instance.ClearHighScore();
        //UpdateHighScore(SaveFileManager.Instance.LoadHighScore());
    }

    public void UpdateHighScore(int newHighScore)
    {
        if (newHighScore > highScore)
        {
            highScore = newHighScore;
            prevHighScore = newHighScore;
            highScoreUpdated?.Invoke();
        }
        else if (newHighScore < highScore)
            PlayGamesPlatformManager.Instance.TryUploadScoreToLeaderboard(highScore);
    }

    //Called with true when app is suspended and false when app is resumed
    private void OnApplicationPause(bool pause)
    {
        if (currentGameState == GameState.Running && pause == true)
            TogglePause();
    }

    public void TogglePause()
    {
        if (currentGameState == GameState.Running)
            ChangeGameState(GameState.Paused);
        else if (currentGameState == GameState.Paused)
            ChangeGameState(GameState.Running);
    }

    private void ChangeGameState(GameState newGameState)
    {
        gameStateChanged?.Invoke(newGameState);

        if (newGameState == GameState.Paused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1.0f;

        currentGameState = newGameState;
    }

    private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public IEnumerator StartGame()
    {
        ChangeGameState(GameState.Running);
        LoadScene("Main");
        yield return null;  //Wait for the next frame to ensure the scene has loaded

        FindObjectOfType<LifeTracker>().SetStartingLives(3);
    }

    public IEnumerator GameOver()
    {
        ScoreTracker scoreTracker = FindObjectOfType<ScoreTracker>();   //scoreTracker is declared here
            //to ensure it is not used outside of this function as it will be null everywhere else
            //due to being destroyed when the scene changes

        ChangeGameState(GameState.Postgame);
        scoreTracker.StopIncrementScore();
        finalScore = scoreTracker.score;
        if (finalScore > highScore)
        {
            highScore = finalScore;
            //SaveFileManager.Instance.SaveHighScore(highScore);
            PlayGamesPlatformManager.Instance.TryUploadScoreToLeaderboard(highScore);
        }
        yield return new WaitForSeconds(1);
        LoadScene("GameOver");
    }

    public void ReturnToMainMenu()
    {
        ChangeGameState(GameState.Pregame);
        LoadScene("MainMenu");
    }

    /*
    public void SyncLocalAndCloudSaves(int cloudSaveHighScore)
    {
        int localSaveHighScore = highScore;

        if (localSaveHighScore > cloudSaveHighScore)
            PlayGamesPlatformManager.Instance.TryUploadScoreToLeaderboard(localSaveHighScore);
        else if (cloudSaveHighScore > localSaveHighScore)
        {
            SaveFileManager.Instance.SaveHighScore(cloudSaveHighScore);
            UpdateHighScore(cloudSaveHighScore);
        }
    }
    */
}
