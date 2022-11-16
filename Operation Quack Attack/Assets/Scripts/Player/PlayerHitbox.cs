
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private Player player;
    private Collider2D hurtbox;
    private Collider2D hitbox;

    private void Start()
    {
        Collider2D[] colliders = GetComponents<Collider2D>();
        hurtbox = colliders[0];
        hitbox = colliders[1];
    }

    private void Update()
    {
        if (player == null) return;

        if (player.State == PlayerState.Dashing)
        {
            hitbox.enabled = true;
        } 
        else
        {
            hitbox.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player == null) return;
        if (player.State == PlayerState.Dead) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            // When hit, take damage
            if (player.State != PlayerState.Dashing)
            {
                float diff = player.gameObject.transform.position.x - collision.gameObject.transform.position.x;
                player.HitSide = (int)Mathf.Sign(diff);
                player.TakeDamage(1);
            }
            // If dashing, become invincible and deal damage with collisions
            else
            {
                player.DealDamage(collision.gameObject, 2);
            }
        }
    }
}