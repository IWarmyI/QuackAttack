using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 pos;
    [SerializeField] private float speed = 20;
    public bool facingRight = false;
    public GameObject player;

    // GameObject Components
    SpriteRenderer spr;
    Rigidbody2D rb;

    private void OnEnable()
    {
        if (player == null) player = GameObject.FindWithTag("Player");

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] playerCols = player.GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            foreach (Collider2D col2 in playerCols)
            {
                Physics2D.IgnoreCollision(col, col2);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;

        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;

        Vector2 vel = Vector2.zero;
        if (facingRight)
        {
            vel = Vector2.right * speed;
        }
        else
        {
            vel = Vector2.left * speed;
        }
        pos += vel * Time.deltaTime;
        rb.velocity = vel;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(player.tag))
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && gameObject.activeSelf)
        {
            DealDamage(collision.gameObject);
            this.gameObject.SetActive(false);
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
}
