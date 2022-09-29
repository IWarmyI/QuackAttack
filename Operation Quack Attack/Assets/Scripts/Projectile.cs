using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector2 pos;
    [SerializeField] private Vector2 velRight = new Vector2(0, 20);
    [SerializeField] private Vector2 velLeft = new Vector2(0, -20);
    public bool facingRight = false;

    // GameObject Components
    SpriteRenderer spr;
    Rigidbody2D rb;

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
}
