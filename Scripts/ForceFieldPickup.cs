using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldPickup : Pickup
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == playerCollider)
        {
            playerController.forceField.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
