using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    public Vector2 pos;
    [SerializeField] private Vector2 velRight = new Vector2(0, 20);
    [SerializeField] private Vector2 velLeft = new Vector2(0, -20);
    public bool facingRight = false;
    public GameObject player;

    // GameObject Components
    SpriteRenderer spr;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;

        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;

        if (facingRight)
        {
            pos += velRight * Time.deltaTime;
            rb.velocity = velRight;
        }
        else
        {
            pos += velLeft * Time.deltaTime;
            rb.velocity = velLeft;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag(player.tag))
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
        return 0;
    }
}
