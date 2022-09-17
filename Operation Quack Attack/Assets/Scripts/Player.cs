using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 pos;
    private Vector2 input = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;
    [SerializeField] private Vector2 vel = Vector2.zero;
    private float speed = 0;
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float accelRate = 10;
    const float deccel = 0.75f;

    [SerializeField] private float jumpStrength = 10;
    [SerializeField] private float fallGravity = 15;
    [SerializeField] private float jumpGravity = 10;
    private bool onGround = false;
    [SerializeField] private float driftSpeed = 5;
    const float airDeccel = 0.95f;


    SpriteRenderer spr;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        speed = baseSpeed;
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get current position
        //pos = transform.position;

        // Ground Movement
        if (onGround)
        {
            spr.color = Color.white;

            // Lateral movement
            if (input.x != 0)
            {
                // Reset speed when changing directions
                if (input.x + oldInput.x == 0)
                {
                    speed = baseSpeed;
                }
                // Accelerate speed, clamped from baseSpeed to topSpeed
                speed = Mathf.Clamp(speed + accelRate * Time.deltaTime, baseSpeed, topSpeed);
                // Update velocity
                vel.x = input.x * speed;
            }
            else
            {
                // Reset speed and deccelerate velocity
                speed = baseSpeed;
                vel.x *= deccel;
                if (vel.x < baseSpeed * 0.1f) vel.x = 0;
            }
        }
        // Air Movement
        else
        {
            spr.color = Color.red;

            // Vertical movement
            if (vel.y > 0)
            {
                vel.y -= jumpGravity * Time.deltaTime;
            }
            else
            {
                vel.y -= fallGravity * Time.deltaTime;
            }

            // Lateral movement
            if (input.x != 0)
            {
                // Update velocity
                Vector2 latVel = new Vector2(vel.x + input.x * driftSpeed * Time.deltaTime, 0);
                latVel = Vector2.ClampMagnitude(latVel, topSpeed);
                vel.x = latVel.x;
            }
            else
            {
                // Reset speed and deccelerate velocity
                vel.x *= airDeccel;

            }
        }

        // Apply velocity
        pos += vel * Time.deltaTime;

        // Update position
        //transform.position = pos;
        rb.velocity = vel;
    }

    private void OnMove(InputValue value)
    {
        oldInput = input;
        input = value.Get<Vector2>();
        input.y = 0;
        if (input != Vector2.zero) input.Normalize();
    }

    private void OnJump(InputValue value)
    {
        if (onGround)
        {
            vel.y = jumpStrength;
            onGround = false;
        }
    }

    private void OnDash(InputValue value)
    {
        // TBD
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;
            if (vel.y < 0)
            {
                vel.y = 0;
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;
        }
        else
        {
            if (!onGround)
            {
                speed = baseSpeed;
                vel.x *= deccel;
                if (vel.x < baseSpeed * 0.1f) vel.x = 0;
                spr.color = Color.blue;
            }

            if (vel.y > 0)
            {
                vel.y *= deccel;
                spr.color = Color.blue;
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }
}
