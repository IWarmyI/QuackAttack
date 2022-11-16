using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour, IDamageable
{
    public int Health { get; set; }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DealDamage(collision.gameObject);
        }
    }

    public int DealDamage(IDamageable target, int damage = 1)
    {
        return target.TakeDamage(damage);
    }

    public int DealDamage(GameObject target, int damage = 1)
    {
        if (target.TryGetComponent(out IDamageable damageable))
        {
            return DealDamage(damageable, damage);
        }
        return -1;
    }

    public int TakeDamage(int damage = 1)
    {
        return -1;
    }
}
