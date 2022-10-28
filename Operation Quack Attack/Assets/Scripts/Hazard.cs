using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour, IDamageable
{
    public int Health { get; set; }

    public int DealDamage(IDamageable target, int damage = 1)
    {
        return target.TakeDamage(damage);
    }

    public int TakeDamage(int damage = 1)
    {
        return -1;
    }
}
