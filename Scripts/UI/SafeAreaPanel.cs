using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform rectTransform;

    private float upperGameBoundX_Pixels;
    private float lowerGameBoundX_Pixels;
    private float upperGameBoundY_Pixels;
    private float lowerGameBoundY_Pixels;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        upperGameBoundX_Pixels = GameManager.Instance.upperGameBoundX_Pixels;
        lowerGameBoundX_Pixels = GameManager.Instance.lowerGameBoundX_Pixels;
        upperGameBoundY_Pixels = GameManager.Instance.upperGameBoundY_Pixels;
        lowerGameBoundY_Pixels = GameManager.Instance.lowerGameBoundY_Pixels;

        //Set the extents of the safe area panel to the screen safe area or the game bounds, which ever is more restrictive
        rectTransform.anchorMin = new Vector2 (Mathf.Max(Screen.safeArea.xMin, lowerGameBoundX_Pixels) / Screen.width,
            Mathf.Max(Screen.safeArea.yMin, lowerGameBoundY_Pixels) / Screen.height);
        rectTransform.anchorMax = new Vector2 (Mathf.Min(Screen.safeArea.xMax, upperGameBoundX_Pixels) / Screen.width,
            Mathf.Min(Screen.safeArea.yMax, upperGameBoundY_Pixels) / Screen.height);
    }
}
