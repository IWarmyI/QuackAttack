using System;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public int Health { get; set; }
    public int TakeDamage(int damage = 1);
    public int DealDamage(IDamageable target, int damage = 1);
}
