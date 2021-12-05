using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLifePickup : Pickup
{
    public static UnityEngine.Events.UnityAction pickedUp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == playerCollider)
        {
            FindObjectOfType<LifeTracker>().GainLife();
            pickedUp?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
