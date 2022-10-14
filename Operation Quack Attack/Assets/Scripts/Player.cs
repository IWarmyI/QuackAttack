using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    // Player State
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
    private bool hasStarted = false;

    // Ground Movement
    [Header("Movement")]
    [SerializeField] private Vector2 vel = Vector2.zero;
    private float speed = 0;
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float topSpeed = 10;
    [SerializeField] private float accelRate = 10;
    private const float deccel = 0.65f; // Ground Friction

    // Dashing
    [Header("Dashing")]
    private bool canDash = true;
    private bool isDashing = false;
    [SerializeField] private float dashingSpeed = 10f;
    [SerializeField] private float dashingTime = 0.2f;
    private float dashingTimer = 0.2f;

    // Jumping and Falling
    [Header("Jumping")]
    [SerializeField] private float jumpStrength = 10;
    [SerializeField] private float jumpGravity = 10;
    [SerializeField] private float fallGravity = 15;
    private bool onGround = false;
    [SerializeField] private float driftInfluence = 25;
    [SerializeField] private float topAirSpeed = 10;
    private const float airDeccel = 0.96f; // Air Resistance

    [SerializeField] private int airJumps = 1;
    [SerializeField] private int jumpCount = 0;
    private bool onWall = false;
    [SerializeField] private int wallSide = 0;

    // Shooting
    [Header("Shooting")]
    [SerializeField] public Projectile projectile;
    [SerializeField] private Transform projectileManager;
    [SerializeField] private Vector2 projectileOffset = Vector2.zero;
    [SerializeField] private bool autoShoot = true;
    [SerializeField] private float fireRate = 0.2f;
    private float fireTimer = 0.2f;
    private bool isFiring = false;
    [SerializeField] public List<Projectile> projectileList = new List<Projectile>();

    // I-Frames
    public float iFrames = 0.07f;
    public float iFramesTimer = 0;

    //Sound Effects 
    [Header("Sound")]
    [SerializeField] private AudioClip quack = null;
    AudioSource[] _sources;
    public AudioSource quackSource;
    public AudioSource sfxSource;
    [SerializeField] private AudioClip[] movementSfx = null;

    // GameObject Components
    private SpriteRenderer spr;
    private Rigidbody2D rb;
    [SerializeField]private GameObject gameOverScreen;

    // Properties
    public PlayerState State { get { return state; } }
    public AnimState Animation { get { return animState; } }
    public bool HasStarted { get { return hasStarted; } }
    public int Health { get { return health; } set { health = value; } }
    public bool OnGround { get { return onGround; } }
    public Vector2 Velocity { get { return vel; } }
    private Vector2 ProjectilePos {
        get
        {
            return pos + new Vector2(
                projectileOffset.x * (facingRight ? 1 : -1),
                projectileOffset.y
                );
        }
    }
    private float FireTime { get { return 1.0f / fireRate; } }

    // Start is called before the first frame update
    void Start()
    {
        _sources = GetComponents<AudioSource>();
       quackSource = _sources[0];
        sfxSource = _sources[1];

        if (quackSource == null)
        {
            Debug.LogError("Quack Source is empty");
        }
        else if (sfxSource == null)
        {
            Debug.LogError("SFX source is empty");
        }
        else
        {
            quackSource.clip = quack;
        }

        pos = transform.position;
        speed = baseSpeed;
        dashingTimer = dashingTime;
        fireTimer = FireTime;

        spr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();

        for (int i = 0; i < 10; i++)
        {
            projectileList.Add(Instantiate(projectile, ProjectilePos, Quaternion.identity, projectileManager));
            projectileList[projectileList.Count - 1].gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
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

        // Update Shooting Timer
        if (fireTimer < FireTime)
        {
            fireTimer += Time.deltaTime;
        }

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
            // Lateral movement
            if (input.x != 0)
            {
                // Set animation state to Run
                animState = AnimState.Run;

                // Reset speed when changing directions
                if (input.x != oldInput.x) { speed = baseSpeed; }

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
        { // Set animation state to Air
            animState = AnimState.Air;

            // Vertical movement
            // If rising, use jump gravity; if falling, use fall gravity
            vel.y -= (vel.y > 0 ? jumpGravity : fallGravity) * Time.deltaTime;

            // Lateral movement
            if (input.x != 0)
            {
                // Inluence velocity by driftInfluence, clamped at topSpeed
                Vector2 latVel = new Vector2(vel.x + input.x * driftInfluence * Time.deltaTime, 0);
                latVel = Vector2.ClampMagnitude(latVel, topAirSpeed);
                speed = latVel.magnitude;

                // Update velocity
                vel.x = latVel.x;
            }
            else
            {
                // Deccelerate velocity
                vel.x = Mathf.Min(vel.x, topAirSpeed);
                vel.x *= airDeccel;
            }
        }

        if (autoShoot && isFiring)
        {
            if (fireTimer >= FireTime)
            {
                ShootProjectile();
                fireTimer -= FireTime;
            }
        }

        // Stores old input
        oldInput = input;

        // I-Frames
        if (iFramesTimer > 0)
        {
            iFramesTimer -= Time.deltaTime;
        }
    }

    private void UpdateDashing()
    {
        // Set animation state to Air
        animState = AnimState.Dash;

        // Dashing timer counting
        dashingTimer -= Time.deltaTime;

        // Apply dashing speed
        vel.x = speed * (facingRight ? 1 : -1);

        // When counter is over, go back to Normal state and reset dash timer
        if (dashingTimer <= 0)
        {
            isDashing = false;
            dashingTimer = dashingTime;
            iFramesTimer = iFrames;
            state = PlayerState.Normal;
        }
    }

    private void ShootProjectile()
    {
        // Create projectile instance
        foreach (Projectile bullet in projectileList)
        {
            if (bullet.gameObject.activeSelf)
            {
                continue;
            }
            else
            {
                bullet.transform.position = ProjectilePos;
                bullet.facingRight = facingRight;
                bullet.transform.SetAsLastSibling();
                bullet.gameObject.SetActive(true);
                return;
            }
        }
        Projectile first = projectileManager.GetChild(0).GetComponent<Projectile>();
        first.transform.position = ProjectilePos;
        first.facingRight = facingRight;
        first.transform.SetAsLastSibling();
        first.gameObject.SetActive(true);
    }

    // ========================================================================
    // Player Input Messages
    // ========================================================================
    private void OnMove(InputValue value)
    {
        // Gets new input
        input = value.Get<Vector2>();

        // Only uses x axis of input (x can only be -1, 0, or 1)
        input.y = 0;
        if (input != Vector2.zero) input.Normalize();

        if (input != Vector2.zero) hasStarted = true;
    }

    private void OnJump(InputValue value)
    {
        // Can only jump if in normal state
        if (state == PlayerState.Normal)
        {
            // Wall jumps if on wall
            if (input.x != 0 && onWall && !onGround)
            {
                sfxSource.PlayOneShot(movementSfx[0]);
                Vector2 wallJump = new Vector2(wallSide, 1);
                if (input.x + wallSide > 0) wallJump = new Vector2(wallSide * 0.55f, 0.75f);
                vel = wallJump * jumpStrength;
                onWall = false;
                return;
            }

            // Jumps if on ground or double jump
            if (jumpCount < airJumps || onGround)
            {
                vel.y = jumpStrength;
                onGround = false;
                jumpCount++;
                sfxSource.PlayOneShot(movementSfx[1]);
            }
        }

        hasStarted = true;
    }

    private void OnDash(InputValue value)
    {
        // Can only dash if in normal state
        if (state == PlayerState.Normal)
        {
            sfxSource.PlayOneShot(movementSfx[2]);
            // Change state to dashing and reset vertical speed
            state = PlayerState.Dashing;
            vel.y = 0;

            // Set dashing speed
            speed = dashingSpeed;
        }

        hasStarted = true;
    }

    private void OnQuack(InputValue value)
    {
        //Play the quack sound effect when button is pressed
        quackSource.PlayOneShot(quack);
    }

    private void OnShoot(InputValue value)
    {
        if (autoShoot)
        {
            isFiring = value.isPressed;
        }
        else
        {
            if (state == PlayerState.Normal)
            {
                if (value.isPressed) ShootProjectile();
            }
        }

        if (value.isPressed) hasStarted = true;
    }

    public void OnRestart(InputValue value)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ========================================================================
    // Collision Messages
    // ========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            // If bottom of player collider is over top of platform colllider
            if (collision.collider.bounds.max.y < collision.otherCollider.bounds.min.y)
            {
                onGround = true;
                jumpCount = 0;

                // Resets vertical velocity
                if (vel.y < 0)
                {
                    vel.y = 0;
                }
            }

            // If top of player collider is under bottom of platform colllider
            else if (collision.collider.bounds.min.y > collision.otherCollider.bounds.max.y)
            {
                // When bumping head, kill vertical velocity
                if (vel.y > 0) vel.y = 0;
            }

            // If player bumps into wall/side of a platform
            else if (collision.collider.bounds.min.y <= collision.otherCollider.bounds.max.y)
            {
                // If air dashing, bonk off of the wall
                if (!onGround && (state == PlayerState.Dashing))// || Math.Abs(vel.x) >= topSpeed))
                {
                    speed = baseSpeed * -1;
                    vel.x = speed * (facingRight ? 1 : -1);

                    // Cancel dash
                    isDashing = false;
                    dashingTimer = dashingTime;
                    state = PlayerState.Normal;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
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
            if (bMaxY <= aMinY)
            {
                onGround = true;
                jumpCount = 0;

                // Resets vertical velocity
                if (vel.y < 0)
                {
                    vel.y = 0;
                }
            }

            // If player bumps into wall/side of a platform
            else
            {
                if (!onGround)
                {
                    // If under platform
                    if (aMaxY <= bMinY)
                    {
                    }

                    // If on wall, allow walljump
                    if (bMinY <= aMaxY)
                    {
                        onWall = true;

                        // Determine which side the wall is on
                        float deltaX = vel.x * Time.deltaTime;
                        if (aMaxX + deltaX >= bMinX && aMinX < bMinX) // Rightside Wall
                            wallSide = -1;
                        else if (aMinX + deltaX <= bMaxX && aMaxX > bMaxX) // Leftside Wall
                            wallSide = 1;
                    }

                    // If air dashing, bonk off of the wall
                    if (state == PlayerState.Dashing)// || Math.Abs(vel.x) >= topSpeed))
                    {
                        if (bMinY <= aMaxY)
                        {
                            // Only trigger when dashing into wall
                            float deltaX = vel.x * Time.deltaTime;
                            if ((aMaxX + deltaX >= deltaX && aMinX < bMinX) ||
                                (aMinX + deltaX <= bMaxX && aMaxX > bMaxX))
                            {
                                speed = baseSpeed;
                                vel.x = -1 * speed * (facingRight ? 1 : -1);

                                // Cancel dash
                                isDashing = false;
                                dashingTimer = dashingTime;
                                state = PlayerState.Normal;
                            }
                        }
                    }
                    // Otherwise, if in the air, reduces lateral velocity as if on the ground
                    else
                    {
                        speed = baseSpeed;
                        vel.x *= deccel;
                        if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0; ;
                    }

                    // If rising, reduce vertical velocity
                    if (vel.y > 0 && Math.Abs(vel.x) >= baseSpeed) vel.y *= airDeccel;
                }

                else
                {
                    if (aMaxY > bMaxY && aMinY <= bMaxY)
                    {
                        onGround = false;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Stage"))
        {
            onGround = false;
            onWall = false;
            wallSide = 0;
        }
    }

    // ========================================================================
    // Damageable Methods
    // ========================================================================

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

    public int TakeDamage(int damage = 1)
    {
        health -= damage;
        if (health <= 0)
        {
            gameObject.SetActive(false);
            gameOverScreen.SetActive(true);
            hasStarted = false;
        }
        return health;
    }
}
