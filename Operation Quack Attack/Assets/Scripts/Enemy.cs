using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy")]
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected int health = 1;

    protected Vector2 pos = Vector2.zero;
    protected Vector2 spawn = Vector2.zero;
    protected bool facingRight = false;

    protected SpriteRenderer spr;
    [SerializeField] protected Color color;

    public int Health { get; set; }

    protected virtual void Start()
    {
        pos = transform.position;
        spawn = transform.position;
        spr = GetComponent<SpriteRenderer>();
        color = spr.color;
    }

    protected virtual void OnEnable()
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

        if (pos != spawn)
        {
            pos = spawn;
            transform.position = pos;
        }
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
