using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Dashing
    }

    // Player state
    private PlayerState state = PlayerState.Normal;

    // Position and Input
    private Vector2 pos;
    private Vector2 input = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;

    // Ground Movement
    [SerializeField] private Vector2 vel = Vector2.zero;
    private float speed = 0;
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float accelRate = 10;
    const float deccel = 0.75f; // Ground Friction

    // Jumping and Falling
    [SerializeField] private float jumpStrength = 10;
    [SerializeField] private float jumpGravity = 10;
    [SerializeField] private float fallGravity = 15;
    private bool onGround = false;
    [SerializeField] private float driftInfluence = 25;
    const float airDeccel = 0.96f; // Air Resistance

    // GameObject Components
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
        pos = transform.position;

        // Determine update based on playerstate
        switch (state)
        {
            // Normal/Actionable (Idle, Running, Jumping)
            case PlayerState.Normal:
                UpdateNormal();
                break;
            // Dashing
            case PlayerState.Dashing:
                break;
            default:
                break;
        }

        // Apply velocity
        pos += vel * Time.deltaTime;
        rb.velocity = vel;
    }

    //
    private void UpdateNormal()
    {
        // Ground Movement
        if (onGround)
        {
            spr.color = Color.white; // On-ground debug color

            // Lateral movement
            if (input.x != 0)
            {
                // Reset speed when changing directions
                if (input.x + oldInput.x == 0) { speed = baseSpeed; Debug.Log("Pivot."); }

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
                if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;
            }
        }

        // Air Movement
        else
        {
            spr.color = Color.blue; // In-air debug color

            // Vertical movement
            // If rising, use jump gravity; if falling, use fall gravity
            vel.y -= (vel.y > 0 ? jumpGravity : fallGravity) * Time.deltaTime;

            // Lateral movement
            if (input.x != 0)
            {
                // Inluence velocity by driftInfluence, clamped at topSpeed
                Vector2 latVel = new Vector2(vel.x + input.x * driftInfluence * Time.deltaTime, 0);
                latVel = Vector2.ClampMagnitude(latVel, topSpeed);

                // Update velocity
                vel.x = latVel.x;
            }
            else
            {
                // Deccelerate velocity
                vel.x *= airDeccel;
            }
        }
    }

    // ========================================================================
    // Player Input Messages
    // ========================================================================
    private void OnMove(InputValue value)
    {
        // Can only move if in normal state
        if (state == PlayerState.Normal)
        {
            // Stores old input
            oldInput = input;
            // Gets new input
            input = value.Get<Vector2>();

            // Only uses x axis of input (x can only be -1, 0, or 1)
            input.y = 0;
            if (input != Vector2.zero) input.Normalize();
        }
    }

    private void OnJump(InputValue value)
    {
        // Can only jump if in normal state
        if (state == PlayerState.Normal)
        {
            // Jumps if on ground
            if (onGround)
            {
                vel.y = jumpStrength;
                onGround = false;
            }
        }
    }

    private void OnDash(InputValue value)
    {
        // !!!!!!!!!!!!!!!!!!
        //        TBD
        // !!!!!!!!!!!!!!!!!!
    }

    // ========================================================================
    // Collion Messages
    // ========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If bottom of player collider is over top of platform colllider
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;

            // Resets vertical velocity
            if (vel.y < 0)
            {
                vel.y = 0;
            }
        }

        // If top of player collider is under bottom of platform colllider
        else if (collision.collider.bounds.min.y > collision.otherCollider.bounds.max.y)
        {
            spr.color = Color.red; // Bumping debug color

            // When bumping head, kill vertical velocity
            if (vel.y > 0) vel.y = 0;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // If bottom of player collider is over top of platform colllider
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;
        }

        // If player bumps into wall/side of a platform
        else
        {
            if (!onGround)
            {
                spr.color = Color.red; // Bumping debug color

                // If in the air, reduces lateral velocity as if on the ground
                speed = baseSpeed;
                vel.x *= deccel;
                if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;

                // If rising, reduce vertical velocity
                if (vel.y > 0) vel.y *= deccel;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }
}
