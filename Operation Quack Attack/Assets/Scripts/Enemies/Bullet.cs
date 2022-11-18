using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageable
{
    public Vector2 pos;
    public Vector2 dir;
    [SerializeField] private float speed = 20;
    private GameObject owner;
    private GameObject player;
    private int id;

    // GameObject Components
    SpriteRenderer spr;
    Rigidbody2D rb;

    public int Health { get; set; }
    public int Id { get { return id; } }
    public GameObject Owner
    { 
        get { return owner; }
        set { owner = value; IgnoreCollision(); }
    }
    private Vector2 Vel { get { return dir * speed; } }

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
        pos += Vel * Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.velocity = Vel;
        rb.rotation = Vector2.SignedAngle(Vector2.left, dir);
    }

    public void Reinstantiate(Vector2 pos, Vector2 dir, float speed)
    {
        transform.position = pos;
        this.pos = pos;
        this.dir = dir;
        this.speed = speed;
        
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
    public void Reinstantiate(Vector2 pos, Vector2 dir)
    {
        Reinstantiate(pos, dir, this.speed);
    }
    public void Reinstantiate()
    {
        Reinstantiate(this.pos, this.dir, this.speed);
    }

    private void IgnoreCollision()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        Collider2D[] ownerCols = owner.GetComponentsInChildren<Collider2D>();
        Collider2D[] playerCols = GameObject.FindWithTag("Player").GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in colliders)
        {
            foreach (Collider2D col2 in ownerCols)
            {
                Physics2D.IgnoreCollision(col, col2);
            }

            foreach (Collider2D col3 in playerCols)
            {
                if (!col3.isTrigger)
                {
                    Physics2D.IgnoreCollision(col, col3);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (owner == null) gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Stage"))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.activeSelf)
        {
            if (DealDamage(collision.gameObject) != -1)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public int DealDamage(IDamageable target, int damage = 1)
    {
        return target.TakeDamage(damage);
    }
    public int DealDamage(GameObject target, int damage = 1)
    {
        try
        {
            IDamageable damageable = target.GetComponentInParent<IDamageable>();
            return DealDamage(damageable, damage);
        }
        catch
        {
            return -1;
        }
    }

    public int TakeDamage(int damage = 1)
    {
        gameObject.SetActive(false);
        return 0;
    }
}
