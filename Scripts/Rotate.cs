using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float rotationSpeed;

    private void Start()
    {
        rotationSpeed = 1.2f;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed));
    }
}
