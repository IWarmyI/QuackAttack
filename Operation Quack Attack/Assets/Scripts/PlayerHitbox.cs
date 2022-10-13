
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // When hit, take damage
            if (player.State != PlayerState.Dashing)
            {
                if (player.iFramesTimer == 0)
                {
                    player.TakeDamage(1);
                }
            }
            // If dashing, become invincible and deal damage with collisions
            else
            {
                player.DealDamage(collision.gameObject, 2);
            }
        }
    }
}