using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupWater : MonoBehaviour
{
    [SerializeField] public float refillAmount = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.activeSelf)
        {
            Player player = collision.gameObject.GetComponentInParent<Player>();

            if (player != null)
            {
                player.RefillWater(refillAmount);
                gameObject.SetActive(false);
            }
        }
    }
}
