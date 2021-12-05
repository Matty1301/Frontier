using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInButtons : MonoBehaviour
{
    [SerializeField]
    private Button signInButton, signOutButton;

    private void OnEnable()
    {
        signInButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.signIn));
        signOutButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.signOut));
        PlayGamesPlatformManager.signedInStatusChanged += ToggleSignInButton;

        //signedInIcon.SetActive(Social.localUser.authenticated);
    }

    private void ToggleSignInButton(bool isSignedIn)
    {
        signInButton.gameObject.SetActive(!isSignedIn);
        signOutButton.gameObject.SetActive(isSignedIn);
    }

    private void OnDisable()
    {
        signInButton.onClick.RemoveAllListeners();
        PlayGamesPlatformManager.signedInStatusChanged -= ToggleSignInButton;
    }
}
