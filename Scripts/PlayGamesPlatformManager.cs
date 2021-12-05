using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class PlayGamesPlatformManager : Singleton<PlayGamesPlatformManager>
{
    public static UnityEngine.Events.UnityAction<bool> signedInStatusChanged;
    public static UnityEngine.Events.UnityAction<string> showMessage;

    private void Start()
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) => { HandleSignInResult(result); });
    }

    private void TrySignInPromptAlways(System.Action<SignInStatus> callback = null)
    {
        PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) => { HandleSignInResult(result, callback); });
    }

    //Callback can be passed in to allow the caller to perform specific actions depending on the sign in result returned
    private void HandleSignInResult(SignInStatus result, System.Action<SignInStatus> callback = null)
    {
        if (result == SignInStatus.Success)
        {
            signedInStatusChanged?.Invoke(true);

            //Get the user's high score from the leaderboard then sync that value with the high score from local save
            //TryGetScoreFromLeaderboard(GameManager.Instance.SyncLocalAndCloudSaves);
            TryGetScoreFromLeaderboard(GameManager.Instance.UpdateHighScore);
        }

        callback?.Invoke(result);
    }

    public void ToggleSignedInStatus()
    {
        if (Social.localUser.authenticated == true)
        {
            PlayGamesPlatform.Instance.SignOut();
            signedInStatusChanged?.Invoke(false);
        }
        else
            TrySignInPromptAlways((result) =>
            {
                if (result == SignInStatus.NetworkError)
                    showMessage?.Invoke("You must be connected to the internet to sign in");
                else if (result != SignInStatus.Success && result != SignInStatus.Canceled)
                    showMessage?.Invoke("Sign in failed");
            });
    }

    //If the user is signed in show the leaderboard else try and sign in.
    //If sign in is successful show the leaderboard else display a message
    public void TryShowLeaderboard()
    {
        if (Social.localUser.authenticated == true)
            ShowLeaderboard();
        else
            showMessage?.Invoke("You must be signed in to view the leaderboard");
        /*
         * TrySignInPromptAlways((result) =>
        {
            if (result == SignInStatus.Success)
                ShowLeaderboard();
            else if (result == SignInStatus.Canceled)
                showMessage?.Invoke("You must be signed in to view the leaderboard");
            else if (result == SignInStatus.NetworkError)
                showMessage?.Invoke("You must be connected to the internet to view the leaderboard");
            else
                showMessage?.Invoke("Sign in failed");
        });
        */
    }

    //Show the leaderboard and once finished showing check if the user is no longer signed in
    private void ShowLeaderboard()
    {
        PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkIhe7toJIdEAIQBA", (UIStatus) =>
        {
            if (Social.localUser.authenticated == false)
                signedInStatusChanged?.Invoke(false);
        });
    }

    public void TryUploadScoreToLeaderboard(int score)
    {
        if (Social.localUser.authenticated)
            Social.ReportScore(score, "CgkIhe7toJIdEAIQBA", (success) =>
            {
                if (success == false)
                    showMessage?.Invoke("Failed to upload score to the leaderboard");
            });
    }

    //Loop through all leaderboard entries until the current user's entry is found and return the score for that entry
    //If no entry is found for the current user or the user is not signed in return 0
    private void TryGetScoreFromLeaderboard(System.Action<int> callback = null)
    {
        int score = 0;

        if (Social.localUser.authenticated)
            Social.LoadScores("CgkIhe7toJIdEAIQBA", (leaderboardEntries) =>
            {
                foreach (IScore entry in leaderboardEntries)
                {
                    //Debug.Log("USER: " + entry.userID + " VALUE: " + entry.value);
                    if (entry.userID == Social.localUser.id)
                    {
                        score = (int)entry.value;
                        break;
                    }
                }
                //Debug.Log("SCORE FROM LEADERBOARD: " + score);
                callback?.Invoke(score);
            });
    }
}
