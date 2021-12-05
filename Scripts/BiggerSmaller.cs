using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiggerSmaller : MonoBehaviour
{
    public float scalar;
    public float time;
    public float minScale;
    public float maxScale;

    private void Start()
    {
        StartCoroutine(Smaller());
    }

    private IEnumerator Smaller()
    {
        while (transform.localScale.x > minScale)
        {
            transform.localScale -= Vector3.one * scalar;
            yield return new WaitForSeconds(time);
        }
        StartCoroutine(Bigger());
    }

    private IEnumerator Bigger()
    {
        while (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * scalar;
            yield return new WaitForSeconds(time);
        }
        StartCoroutine(Smaller());
    }
}
