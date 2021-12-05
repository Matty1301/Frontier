using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField]
    private Button muteButton;
    [SerializeField]
    private GameObject soundOnIcon, soundOffIcon;

    private void OnEnable()
    {
        muteButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.mute));
        SoundManager.muteToggled += ToggleMuteButtonIcon;
    }

    private void ToggleMuteButtonIcon(bool isMuted)
    {
        soundOffIcon.SetActive(isMuted);
        soundOnIcon.SetActive(!isMuted);
    }

    private void OnDisable()
    {
        muteButton.onClick.RemoveAllListeners();
        SoundManager.muteToggled -= ToggleMuteButtonIcon;
    }
}
