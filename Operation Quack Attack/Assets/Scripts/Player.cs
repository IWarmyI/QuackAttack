using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    public enum PlayerState
    {
        Normal,
        Dashing
    }

    public enum AnimState
    {
        Idle,
        Run,
        Air,
        Dash
    }

    // Player state
    private PlayerState state = PlayerState.Normal;
    [SerializeField] private int health = 1;

    // Animation
    private AnimState animState = AnimState.Idle;
    private PlayerAnimation anim;

    // Position and Input
    private Vector2 pos;
    private Vector2 input = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;
    private bool facingRight = true;

    // Ground Movement
    [SerializeField] private Vector2 vel = Vector2.zero;
    private float speed = 0;
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float accelRate = 10;
    const float deccel = 0.75f; // Ground Friction

    // Dashing
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashingSpeed = 10f;
    [SerializeField] private float dashingTime = 0.2f;
    private float dashingTimer = 0.2f;

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

    //Air jump counter
    [SerializeField] int numOfJumps = 0;

    // Shooting
    public Projectile projectile;
    public List<Projectile> projectileList = new List<Projectile>();

    public int Health { get; }

    //Sound Effects 
    [SerializeField]
    private AudioClip quack = null;
    private AudioSource _source = null;

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();

        if (_source == null)
        {
            Debug.LogError("Audio Source is empty");
        }
        else
        {
            _source.clip = quack;
        }

        pos = transform.position;
        speed = baseSpeed;
        dashingTimer = dashingTime;

        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponentInChildren<PlayerAnimation>();
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
                UpdateDashing();
                break;
            default:
                break;
        }

        // Apply velocity
        pos += vel * Time.deltaTime;
        rb.velocity = vel;

        // Animate
        anim.Animate(animState, onGround, vel.y <= 0, speed >= topSpeed, facingRight);
    }

    private void UpdateNormal()
    {
        // Get facing
        if (input.x != 0) facingRight = input.x > 0;

        // Ground Movement
        if (onGround)
        {
            spr.color = Color.white; // On-ground debug color

            // Lateral movement
            if (input.x != 0)
            {
                // Set animation state to Run
                animState = AnimState.Run;

                // Reset speed when changing directions
                if (input.x + oldInput.x == 0) { speed = baseSpeed; }

                // Accelerate speed, clamped from baseSpeed to topSpeed
                speed = Mathf.Clamp(speed + accelRate * Time.deltaTime, baseSpeed, topSpeed);

                // Update velocity
                vel.x = input.x * speed;
            }
            else
            {
                // Set animation state to Idle
                animState = AnimState.Idle;

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

            // Set animation state to Air
            animState = AnimState.Air;

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

    private void UpdateDashing()
    {
        spr.color = Color.yellow; // Dashing debug color

        // Set animation state to Air
        animState = AnimState.Dash;

        // Dashing timer counting
        dashingTimer -= Time.smoothDeltaTime;

        // Apply dashing speed
        vel.x = speed * (facingRight ? 1 : -1);

        // When counter is over, go back to Normal state and reset dash timer
        if (dashingTimer <= 0)
        {
            isDashing = false;
            dashingTimer = dashingTime;
            state = PlayerState.Normal;
            if (oldInput != input)
            {
                vel.x = 0;
            }
        }
    }

    // ========================================================================
    // Player Input Messages
    // ========================================================================
    private void OnMove(InputValue value)
    {
        // Can only move if in normal state
        //if (state == PlayerState.Normal)
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
            // Jumps if on ground or double jump
            if (numOfJumps < 1 || onGround)
            {
                vel.y = jumpStrength;
                onGround = false;
                numOfJumps++;
            }
        }
    }

    private void OnDash(InputValue value)
    {
        // Can only dash if in normal state
        if (state == PlayerState.Normal)
        {
            // Change state to dashing and reset vertical speed
            state = PlayerState.Dashing;
            vel.y = 0;

            // Set dashing speed
            speed = dashingSpeed;
        }
    }

    private void OnQuack(InputValue value)
    {
        //Play the quack sound effect when button is pressed
        _source.Play();
    }

    private void OnShoot(InputValue value)
    {
        projectileList.Add(Instantiate(projectile, pos, Quaternion.identity));
        projectileList[projectileList.Count - 1].pos = pos;
        if (facingRight)
        {
            projectileList[projectileList.Count - 1].facingRight = true;
        }
        else
        {
            projectileList[projectileList.Count - 1].facingRight = false;
        }
    }

    // ========================================================================
    // Collision Messages
    // ========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // If bottom of player collider is over top of platform colllider
        if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
        {
            onGround = true;
            numOfJumps = 0;

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
        // If player bumps into wall/side of a platform
        else if (collision.collider.bounds.min.y <= collision.otherCollider.bounds.max.y)
        {
            // If air dashing, bonk off of the wall
            if (!onGround && (state == PlayerState.Dashing))// || Math.Abs(vel.x) >= topSpeed))
            {
                spr.color = Color.red; // Bumping debug color

                speed = baseSpeed * -1;
                vel.x = speed * (facingRight ? 1 : -1);

                isDashing = false;
                dashingTimer = dashingTime;
                state = PlayerState.Normal;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // OtherCollider (this)
        float aMinX = collision.otherCollider.bounds.min.x;
        float aMaxX = collision.otherCollider.bounds.max.x;
        float aMinY = collision.otherCollider.bounds.min.y;
        float aMaxY = collision.otherCollider.bounds.max.y;
        // Collider
        float bMinX = collision.collider.bounds.min.x;
        float bMaxX = collision.collider.bounds.max.x;
        float bMinY = collision.collider.bounds.min.y;
        float bMaxY = collision.collider.bounds.max.y;

        // If bottom of player collider is over top of platform colllider
        if (bMaxY < aMinY)
        {
            onGround = true;
            numOfJumps = 0;
        }

        // If player bumps into wall/side of a platform
        else
        {
            if (!onGround)
            {
                spr.color = Color.red; // Bumping debug color

                // If air dashing, bonk off of the wall
                if (state == PlayerState.Dashing)// || Math.Abs(vel.x) >= topSpeed))
                {
                    if (bMinY <= aMaxY)
                    {
                        // Only trigger when dashing into wall
                        float deltaX = vel.x * Time.deltaTime;
                        if ((bMinX < aMaxX + deltaX && bMaxX > aMinX) ||
                            (bMaxX > aMinX + deltaX && bMinX < aMaxX))
                        {
                            speed = baseSpeed;
                            vel.x = -1 * speed * (facingRight ? 1 : -1);

                            // Cancel dash
                            isDashing = false;
                            dashingTime = 0.2f;
                            state = PlayerState.Normal;
                        }
                    }
                }
                // Otherwise, if in the air, reduces lateral velocity as if on the ground
                else
                {
                    speed = baseSpeed;
                    vel.x *= deccel;
                    if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;;
                }

                // If rising, reduce vertical velocity
                if (vel.y > 0 && Math.Abs(vel.x) >= baseSpeed) vel.y *= deccel;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
    }

    // ========================================================================
    // Damageable Methods
    // ========================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != PlayerState.Dashing)
        {
            TakeDamage();
        }
        else
        {
            DealDamage(collision.GetComponent<IDamageable>());
        }
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
