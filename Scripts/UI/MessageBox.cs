using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private Button box;
    private Text text;
    private float scaleRate = 0.2f;
    private Vector3 scaleRateVector;

    private void OnEnable()
    {
        PlayGamesPlatformManager.showMessage += StartShowMessageCoroutine;
        SceneManager.sceneLoaded += SceneChanged;
        box.onClick.AddListener(() => { StartCoroutine(HideMessage()); });
    }

    private void Start()
    {
        text = GetComponentInChildren<Text>();
        transform.localScale = Vector3.zero;
        scaleRateVector = new Vector3(scaleRate, scaleRate, scaleRate);
    }

    private void StartShowMessageCoroutine(string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessage(message));
    }

    private IEnumerator ShowMessage(string message)
    {
        text.text = message;

        transform.localScale = Vector3.zero;
        while (transform.localScale.x < 1)
        {
            transform.localScale += scaleRateVector;
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage()
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= scaleRateVector;
            yield return null;
        }
        transform.localScale = Vector3.zero;
    }

    private void SceneChanged(Scene unused1, LoadSceneMode unused2)
    {
        StartCoroutine(HideMessage());
    }

    private void OnDisable()
    {
        PlayGamesPlatformManager.showMessage -= StartShowMessageCoroutine;
        SceneManager.sceneLoaded -= SceneChanged;
        box.onClick.RemoveAllListeners();
    }
}
