using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageable
{
    public enum PlayerState
    {
        Stopped,
        Normal,
        Dashing,
        Dead
    }

    public enum AnimState
    {
        Idle,
        Run,
        Air,
        Dash,
        Intro,
        Wall,
        Dead
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        VeryHard
    }

    // Player State
    private PlayerState state = PlayerState.Normal;
    [SerializeField] private int health = 1;

    // Animation
    private AnimState animState = AnimState.Idle;
    private PlayerAnimation anim;
    private bool hasStarted = false;
    private static bool isIntro = true;

    // Position and Input
    private Vector2 pos;
    private Vector2 input = Vector2.zero;
    private Vector2 oldInput = Vector2.zero;
    private bool facingRight = true;

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
    [SerializeField] private float dashingSpeed = 10f;
    [SerializeField] private float dashingTime = 0.2f;
    private float dashingTimer = 0.2f;
    [SerializeField]private float dashingCooldown = 0.5f;
    private float dashingCooldownTimer;

    // I-Frames
    private float iFrames = 0.07f;
    private float iFramesTimer = 0;
    private int hitSide = 0;

    // Jumping and Falling
    [Header("Jumping")]
    [SerializeField] private float jumpStrength = 10;
    [SerializeField] private float jumpGravity = 10;
    [SerializeField] private float fallGravity = 15;
    [SerializeField] private float driftInfluence = 25;
    [SerializeField] private float topAirSpeed = 10;

    private bool onGround = true;
    [SerializeField] private Timer jumpCoyote = new Timer(0.2f);
    private const float airDeccel = 0.96f; // Air Resistance

    [SerializeField] private int airJumps = 1;
    private int jumpCount = 0;

    private bool onWall = false;
    private bool isWallJumping = false;
    private int wallSide = 0;
    [SerializeField] private Timer wallCoyote = new Timer(0.2f);
    private const float wallDeccel = 0.85f; // Wall Friction

    // Shooting
    [Header("Shooting")]
    [SerializeField] public Projectile projectile;
    [SerializeField] private Transform projectileManager;
    [SerializeField] private Vector2 projectileOffset = Vector2.zero;
    [SerializeField] private float fireRate = 0.2f;
    private float fireTimer = 0.2f;
    private bool isFiring = false;
    [SerializeField] public List<Projectile> projectileList = new List<Projectile>();

    // Water ammo
    private float maxWater = 100;
    [SerializeField] public float currentWater = 0;
    [SerializeField] public float reFillAmount = 10;

    //Sound Effects 
    [Header("Sound")]
    [SerializeField] private AudioClip[] quack = null;
    AudioSource[] _sources;
    public AudioSource quackSource;
    public AudioSource sfxSource;
    [SerializeField] private AudioClip[] movementSfx = null;

    // GameObject Components
    [Header("UI")]
    private SpriteRenderer spr;
    private Rigidbody2D rb;
    [SerializeField] private GameObject pauseObj;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject waterGauge;

    // Properties
    public PlayerState State { get { return state; } }
    public AnimState Animation { get { return animState; } set { animState = value; } }
    public bool HasStarted { get { return hasStarted; } }
    public float Speed { get { return speed; } }
    public int Health { get { return health; } set { health = value; } }
    public bool OnGround { get { return onGround; } }
    public int HitSide { get { return hitSide; } set { hitSide = value; } }
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

    public Animator animator;
    public float transitionDelayTime = 1.0f;


    public static void Initialize()
    {
        Player.isIntro = true;
    }

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
            quackSource.clip = quack[0];
        }

        pos = transform.position;
        speed = baseSpeed;
        dashingTimer = dashingTime;
        fireTimer = FireTime;
        currentWater = maxWater;

        spr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
        anim = GetComponentInChildren<PlayerAnimation>();

        for (int i = 0; i < 10; i++)
        {
            projectileList.Add(Instantiate(projectile, ProjectilePos, Quaternion.identity, projectileManager));
            projectileList[projectileList.Count - 1].gameObject.SetActive(false);
        }

        if (isIntro)
        {
            state = PlayerState.Stopped;
            animState = AnimState.Intro;
        }
    }

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // Get current position
        pos = transform.position;

        // Determine update based on playerstate
        switch (state)
        {
            // Paused/Unactionable (Intro)
            case PlayerState.Stopped:
                UpdateStopped();
                break;
            // Normal/Actionable (Idle, Running, Jumping)
            case PlayerState.Normal:
                UpdateNormal();
                break;
            // Dashing
            case PlayerState.Dashing:
                UpdateDashing();
                break;
            // Dying
            case PlayerState.Dead:
                UpdateDead();
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

        jumpCoyote.Update();
        wallCoyote.Update();
    }

    private void LateUpdate()
    {
        // Animate
        anim.Animate(
            animState,
            onGround,
            onWall,
            vel.y <= 0,
            speed >= topSpeed,
            facingRight
        );
    }

    private void UpdateNormal()
    {
        // Get facing
        if (input.x != 0) facingRight = input.x > 0;

        // Resume coyote time
        jumpCoyote.Resume();
        wallCoyote.Resume();

        // Ground Movement
        if (onGround)
        {
            // Restart coyote time
            jumpCoyote.Start(true);

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
        {
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

            // Wall sliding
            if (onWall && input.x != 0)
            {
                wallCoyote.Ready();

                // If pressing against wall, wallslide
                if (input.x == wallSide * -1)
                {
                    animState = AnimState.Wall;

                    if (vel.y < 0)
                    {
                        vel.y *= wallDeccel;
                        //float slow = Mathf.Max(Mathf.Abs(vel.y) - wallDeccel * Time.deltaTime, 0);
                        //vel.y = slow * Mathf.Sign(vel.y);
                    }
                }
            }
        }

        // Firing
        if (isFiring)
        {
            if (fireTimer >= FireTime)
            {
                ShootProjectile();
                fireTimer -= FireTime;
            }
        }

        // I-Frames
        if (iFramesTimer > 0)
            iFramesTimer -= Time.deltaTime;

        // Dashing
        if (dashingCooldownTimer > 0)
        {
            dashingCooldownTimer -= Time.deltaTime;

            if (dashingCooldownTimer <= 0)
            {
                anim.PlayFlash();
            }
        }

        // Stores old input
        oldInput = input;
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
            dashingTimer = dashingTime;
            iFramesTimer = iFrames;
            dashingCooldownTimer = dashingCooldown;
            state = PlayerState.Normal;
        }
    }

    private void UpdateStopped()
    {
        // If intro animation is playing, wait for it to complete
        if (isIntro)
        {
            if (anim.IsComplete(animState))
            {
                isIntro = false;
                state = PlayerState.Normal;
                animState = AnimState.Idle;
            }
        }
        // Else, return to normal
        else
        {
            state = PlayerState.Normal;
            animState = AnimState.Idle;
        }

        // Don't move when stopped
        vel = Vector2.zero;
    }

    private void UpdateDead()
    {
        // Set animation state to Dead
        animState = AnimState.Dead;
        hasStarted = false;

        // Move as if there is no input
        speed = baseSpeed;
        if (onGround)
        {
            vel.x *= deccel;
            if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;
        }
        else
        {
            vel.y -= (vel.y > 0 ? jumpGravity : fallGravity) * Time.deltaTime;
            vel.x = Mathf.Min(vel.x, topAirSpeed);
            vel.x *= airDeccel;
        }

        // Once complete, activate game over screen
        if (anim.IsComplete(animState))
        {
            //gameObject.SetActive(false);
            StartCoroutine(DelayLoadLevel(("GameOver")));
        }
    }    

    private void ShootProjectile()
    {
        if (currentWater >= 5)
        {
            currentWater -= 5;
            waterGauge.GetComponent<WaterGauge>().UpdateBar(currentWater, maxWater);
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

        if (state != PlayerState.Stopped && state != PlayerState.Dead)
            if (input != Vector2.zero) hasStarted = true;
    }

    private void OnJump(InputValue value)
    {
        // Can only jump if in normal state
        if (state == PlayerState.Normal)
        {
            // Wall jumps if on wall
            if (input.x != 0 && !onGround && (onWall || !wallCoyote.IsComplete))
            {
                Vector2 wallJump = new Vector2(wallSide * 1.1f, 1);
                if (input.x + wallSide > 0) wallJump = new Vector2(wallSide * 0.65f, 0.75f);
                vel = wallJump * jumpStrength;

                onWall = false;
                isWallJumping = true;
                wallCoyote.Stop();

                sfxSource.PlayOneShot(movementSfx[0]);
            }

            // Jumps if on ground
            else if (onGround || !jumpCoyote.IsComplete)
            {
                vel.y = jumpStrength;
                onGround = false;
                jumpCoyote.Stop();

                sfxSource.PlayOneShot(movementSfx[1]);
            }

            // Double jump
            else if (jumpCount < airJumps && currentWater >= 10)
            {
                vel.y = jumpStrength;
                onGround = false;
                jumpCount++;
                currentWater -= 10;
                waterGauge.GetComponent<WaterGauge>().UpdateBar(currentWater, maxWater);

                sfxSource.PlayOneShot(movementSfx[1]);
            }
        }

        if (state != PlayerState.Stopped && state != PlayerState.Dead)
            hasStarted = true;
    }

    private void OnDash(InputValue value)
    {
        // Can only dash if in normal state
        if (state == PlayerState.Normal && dashingCooldownTimer <= 0 && currentWater >= 10)
        {
            sfxSource.PlayOneShot(movementSfx[2]);
            // Change state to dashing and reset vertical speed
            state = PlayerState.Dashing;
            vel.y = 0;

            // Set dashing speed
            speed = dashingSpeed;
            currentWater -= 10;
            waterGauge.GetComponent<WaterGauge>().UpdateBar(currentWater, maxWater);
        }

        if (state != PlayerState.Stopped && state != PlayerState.Dead)
            hasStarted = true;
    }

    private void OnQuack(InputValue value)
    {
        //Play the quack sound effect when button is pressed
        int deepQuackPercentage = UnityEngine.Random.Range(0, 100);

        if (deepQuackPercentage <= 10)
        {
            quackSource.PlayOneShot(quack[1]);
        }
        else if (deepQuackPercentage > 10)
        {
            quackSource.PlayOneShot(quack[0]);
        }
    }

    private void OnShoot(InputValue value)
    {
        isFiring = value.isPressed;
        if (state != PlayerState.Stopped && state != PlayerState.Dead)
            if (value.isPressed) hasStarted = true;
    }

    public void OnRestart(InputValue value)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }

    private void OnPauseToggle(InputValue value)
    {
        if (Time.timeScale == 0.0f)
        {
            Time.timeScale = 1.0f;
        }
        else
        {
            Time.timeScale = 0.0f;
        }

        pauseObj.SetActive(!pauseObj.activeSelf);
        HUD.SetActive(!HUD.activeSelf);
        gameObject.SetActive(false);

    }

    // ========================================================================
    // Collision Messages
    // ========================================================================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
            // OtherCollider (Player)
            float aMinX = collision.otherCollider.bounds.min.x;
            float aMaxX = collision.otherCollider.bounds.max.x;
            float aMinY = collision.otherCollider.bounds.min.y;
            float aMaxY = collision.otherCollider.bounds.max.y;
            // Collider (Wall)
            float bMinX = collision.collider.bounds.min.x;
            float bMaxX = collision.collider.bounds.max.x;
            float bMinY = collision.collider.bounds.min.y;
            float bMaxY = collision.collider.bounds.max.y;

            // If bottom of player collider is over top of platform colllider
            if (bMaxY < aMinY)
            {
                onGround = true;
                jumpCount = 0;

                // Resets vertical velocity
                if (vel.y < 0)
                    vel.y = 0;
            }

            // If top of player collider is under bottom of platform colllider
            else if (bMinY > aMaxY)
            {
                // When bumping head, kill vertical velocity
                if (vel.y > 0) vel.y = 0;
            }

            // If player bumps into wall/side of a platform
            else if (bMinY <= aMaxY)
            {
                // If air dashing, bonk off of the wall
                if (!onGround && (state == PlayerState.Dashing))
                {
                    speed = baseSpeed * -1;
                    vel.x = speed * (facingRight ? 1 : -1);

                    // Cancel dash
                    dashingTimer = dashingTime;
                    state = PlayerState.Normal;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
            // OtherCollider (Player)
            float aLeft = collision.otherCollider.bounds.min.x;
            float aRight = collision.otherCollider.bounds.max.x;
            float aBottom = collision.otherCollider.bounds.min.y;
            float aTop = collision.otherCollider.bounds.max.y;
            // Collider (Wall)
            float bLeft = collision.collider.bounds.min.x;
            float bRight = collision.collider.bounds.max.x;
            float bBottom = collision.collider.bounds.min.y;
            float bTop = collision.collider.bounds.max.y;

            // If bottom of player collider is over top of platform colllider
            if (bTop <= aBottom)
            {
                onGround = true;
                isWallJumping = false;
                jumpCount = 0;

                //// Resets vertical velocity
                //if (vel.y < 0)
                //    vel.y = 0;
            }

            // If player bumps into wall/side of a platform
            else
            {
                // If in the air...
                if (!onGround)
                {
                    // If on wall, allow walljump
                    if (bBottom <= aTop && !isWallJumping)
                    {
                        onWall = true;

                        // Determine which side the wall is on
                        float deltaX = vel.x * Time.deltaTime;
                        if (aRight + deltaX >= bLeft && aLeft < bLeft) // Rightside Wall
                            wallSide = -1;
                        else if (aLeft + deltaX <= bRight && aRight > bRight) // Leftside Wall
                            wallSide = 1;
                    }

                    // If air dashing, bonk off of the wall
                    if (state == PlayerState.Dashing)// || Math.Abs(vel.x) >= topSpeed))
                    {
                        if (bBottom <= aTop)
                        {
                            // Only trigger when dashing into wall
                            float deltaX = vel.x * Time.deltaTime;
                            if ((aRight + deltaX >= deltaX && aLeft < bLeft) ||
                                (aLeft + deltaX <= bRight && aRight > bRight))
                            {
                                speed = baseSpeed;
                                vel.x = -1 * speed * (facingRight ? 1 : -1);

                                // Cancel dash
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

                // If on the ground, but not 100% on top, slide down
                else
                {
                    if (aTop > bTop && aBottom <= bTop)
                    {
                        onGround = false;
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.otherCollider.isTrigger) return;

        if (collision.gameObject.CompareTag("Stage"))
        {
            // OtherCollider (Player)
            float aMinX = collision.otherCollider.bounds.min.x;
            float aMaxX = collision.otherCollider.bounds.max.x;
            float aMinY = collision.otherCollider.bounds.min.y;
            float aMaxY = collision.otherCollider.bounds.max.y;
            // Collider (Wall)
            float bMinX = collision.collider.bounds.min.x;
            float bMaxX = collision.collider.bounds.max.x;
            float bMinY = collision.collider.bounds.min.y;
            float bMaxY = collision.collider.bounds.max.y;

            if (bMaxY < aMinY)
            {
                onGround = false;
            }
            else if (!onGround)
            {
                if (bMinY <= aMaxY)
                {
                    onWall = false;
                    isWallJumping = false;
                }
            }

            //onGround = false;
            //wallSide = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PickUp"))
        {
            currentWater += reFillAmount;
            waterGauge.GetComponent<WaterGauge>().UpdateBar(currentWater, maxWater);
            Destroy(collision.gameObject);
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
        if (state == PlayerState.Dead) return 0;

        if (state != PlayerState.Dashing)
        {
            if (iFramesTimer <= 0)
            {
                health -= damage;

                if (health <= 0)
                    Die();
            }
        }

        return health;
    }

    private void Die()
    {
        state = PlayerState.Dead;
        Vector2 knockback = new Vector2(hitSide, 0.5f);
        vel = knockback * jumpStrength;
    }

    IEnumerator DelayLoadLevel(string sceneName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }
}
