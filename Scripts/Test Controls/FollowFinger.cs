using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFinger : MonoBehaviour
{
    private Touch theTouch;
    private Vector3 theTouchPos;

    void Update()
    {
        if (Input.touchCount > 0)
        {
            theTouch = Input.GetTouch(0);

            theTouchPos = Camera.main.ScreenToWorldPoint(theTouch.position);
            transform.position = new Vector3(theTouchPos.x, theTouchPos.y, 0);
        }
    }
}
