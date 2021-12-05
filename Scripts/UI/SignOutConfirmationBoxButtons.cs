using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignOutConfirmationBoxButtons : MonoBehaviour
{
    [SerializeField]
    private Button yesButton, noButton;

    private void OnEnable()
    {
        yesButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.signOutConfirm));
        noButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.signOutDeny));
    }

    private void OnDisable()
    {
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
    }
}
