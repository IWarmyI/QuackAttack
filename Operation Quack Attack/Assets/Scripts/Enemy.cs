using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int health = 1;
    private SpriteRenderer spr;
    private Color color;

    public int Health { get; set; }

    void OnEnable()
    {
        health = maxHealth;

        if (spr == null)
        {
            spr = GetComponent<SpriteRenderer>();
            color = spr.color;
        }
        else
        {
            spr.color = color;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int DealDamage(IDamageable target, int damage = 1)
    {
        return target.TakeDamage(damage);
    }

    public int TakeDamage(int damage = 1)
    {
        health -= damage;
        spr.color = Color.red;
        if (health <= 0) gameObject.SetActive(false); 
        return health;
    }
}
