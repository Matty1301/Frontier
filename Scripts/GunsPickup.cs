using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsPickup : Pickup
{
    public static UnityEngine.Events.UnityAction pickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == playerCollider)
        {
            playerController.guns.SetActive(true);
            pickedUp?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
