using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 1;
    public int Health { get; }

    // Start is called before the first frame update
    void Start()
    {
        
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
        if (health <= 0) gameObject.SetActive(false); 
        return health;
    }
}
