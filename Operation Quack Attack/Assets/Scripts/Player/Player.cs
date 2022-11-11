using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static PlayerCollision;

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
    [Header("Stats")]
    [SerializeField] private int health = 1;
    private PlayerState state = PlayerState.Normal;

    // Water ammo
    [SerializeField] private float maxWater = 100;
    private float currentWater = 0;
    [SerializeField] private float waterRegen = 5;

    // Animation
    private AnimState animState = AnimState.Idle;
    private PlayerAnimation anim;
    private bool hasStarted = false;
    private static bool _isIntro = true;

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
    private PlayerCollision col;

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
    private Timer jumpCooldown;
    private const float airDeccel = 0.96f; // Air Resistance

    [SerializeField] private int airJumps = 1;
    private int jumpCount = 0;

    private bool onWall = false;
    private int wallSide = 0;
    [SerializeField] private Timer wallCoyote = new Timer(0.2f);
    private Timer wallCooldown;
    private const float wallDeccel = 0.8f; // Wall Friction

    // Shooting
    [Header("Shooting")]
    [SerializeField] private Projectile projectile;
    [SerializeField] private Transform projectileManager;
    [SerializeField] private Vector2 projectileOffset = Vector2.zero;
    [SerializeField] private float fireRate = 0.2f;
    private float fireTimer = 0.2f;
    private bool isFiring = false;
    [SerializeField] public List<Projectile> projectileList = new List<Projectile>();

    // GameObject Components
    [Header("UI")]
    [SerializeField] private GameObject pauseObj;
    [SerializeField] private GameObject HUD;
    //[SerializeField] private WaterGauge waterGauge;
    private SpriteRenderer spr;
    private Rigidbody2D rb;

    // Events
    public delegate void PlayerEvent();
    public event PlayerEvent OnPlayerStep; // (Not Implemented)
    public event PlayerEvent OnPlayerLand;
    public event PlayerEvent OnPlayerJump;
    public event PlayerEvent OnPlayerWallJump;
    public event PlayerEvent OnPlayerAirJump;
    public event PlayerEvent OnPlayerDash;
    public event PlayerEvent OnPlayerDashReady;
    public event PlayerEvent OnPlayerShoot;
    public event PlayerEvent OnPlayerQuack;

    // Properties
    public PlayerState State { get { return state; } }
    public AnimState Animation { get { return animState; } }
    public bool HasStarted { get { return hasStarted; } }
    public float Speed { get { return speed; } }
    public int Health { get { return health; } set { health = value; } }
    public float Water { get { return currentWater; }  }
    public float MaxWater { get { return maxWater; } }
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

    public Animator transitionAnimator;
    public float transitionDelayTime = 1.0f;


    public static void Initialize()
    {
        Player._isIntro = true;
    }

    void Awake()
    {
        GameObject.Find("Transition").TryGetComponent(out transitionAnimator);
    }

    void OnEnable()
    {
        input = Vector2.zero;
        isFiring = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        speed = baseSpeed;
        dashingTimer = dashingTime;
        fireTimer = FireTime;
        currentWater = maxWater;

        jumpCooldown = new Timer(jumpCoyote.MaxTime);
        wallCooldown = new Timer(wallCoyote.MaxTime / 2);

        spr = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponentInChildren<Rigidbody2D>();
        col = gameObject.GetOrAddComponent<PlayerCollision>();
        anim = GetComponentInChildren<PlayerAnimation>();

        for (int i = 0; i < 10; i++)
        {
            projectileList.Add(Instantiate(projectile, ProjectilePos, Quaternion.identity, projectileManager));
            projectileList[i].gameObject.SetActive(false);
        }

        if (_isIntro)
        {
            state = PlayerState.Stopped;
            animState = AnimState.Intro;
        }
    }

    private void FixedUpdate()
    {
        // Compute collisions
        ComputeCollisions();

        // Apply rigidbody velocity
        rb.velocity = vel;
    }

    public void Update()
    {
        // Get current position
        pos = transform.position;

        // Determine update based on playerstate
        switch (state)
        {
            // Stopped/Unactionable (Intro)
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

        // Update Shooting Timer
        if (fireTimer < FireTime)
            fireTimer += Time.deltaTime;

        if (currentWater < maxWater)
        {
            currentWater += waterRegen * Time.deltaTime;
            //waterGauge.UpdateBar(currentWater, maxWater);
        }
        if (currentWater > maxWater) currentWater = maxWater;

        jumpCoyote.Update();
        wallCoyote.Update();
        jumpCooldown.Update();
        wallCooldown.Update();
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
        if (onGround || (jumpCoyote.IsRunning && !jumpCooldown.IsRunning && vel.y <= 0))
        {
            // Restart coyote time
            if (onGround && jumpCooldown.IsComplete) jumpCoyote.Ready();

            // Lateral movement
            if (input.x != 0)
            {
                // Set animation state to Run
                animState = AnimState.Run;

                // Reset speed when changing directions
                if (input.x != oldInput.x) speed = baseSpeed;

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
                vel.x *= Mathf.Pow(deccel, 50 * Time.deltaTime);
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
                vel.x *= Mathf.Pow(airDeccel, 50 * Time.deltaTime);
            }

            // Wall sliding
            if (onWall && input.x != 0)
            {
                if (wallCooldown.IsComplete)
                    wallCoyote.Ready();

                // If pressing against wall, wallslide
                if (input.x == wallSide * -1)
                {
                    // Set animation state to Wall
                    animState = AnimState.Wall;

                    // If rising, deccelerate by wallDeccel
                    if (vel.y < 0)
                        vel.y *= Mathf.Pow(wallDeccel, 50 * Time.deltaTime);
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
                OnPlayerDashReady();
            }
        }

        // Stores old input
        oldInput = input;
    }

    private void UpdateDashing()
    {
        // When counter is over, go back to Normal state and reset dash timer
        if (dashingTimer <= 0)
        {
            dashingTimer = dashingTime;
            iFramesTimer = iFrames;
            dashingCooldownTimer = dashingCooldown;
            state = PlayerState.Normal;
        }

        // Set animation state to Air
        animState = AnimState.Dash;

        // Dashing timer counting
        dashingTimer -= Time.deltaTime;

        // Apply dashing speed
        vel.x = speed * (facingRight ? 1 : -1);
    }

    private void UpdateStopped()
    {
        // If intro animation is playing, wait for it to complete
        if (_isIntro)
        {
            if (anim.IsComplete(animState))
            {
                _isIntro = false;
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
            vel.x *= Mathf.Pow(deccel, 50 * Time.deltaTime);
            if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;
        }
        else
        {
            vel.y -= (vel.y > 0 ? jumpGravity : fallGravity) * Time.deltaTime;
            vel.x = Mathf.Min(vel.x, topAirSpeed);
            vel.x *= Mathf.Pow(airDeccel, 50 * Time.deltaTime);
        }

        // Once complete, activate game over screen
        if (anim.IsComplete(animState))
        {
            gameObject.SetActive(false);
            StartCoroutine(DelayLoadLevel(("GameOver")));
        }
    }    

    private void ShootProjectile()
    {
        if (currentWater >= 5)
        {
            currentWater -= 5;
            //waterGauge.UpdateBar(currentWater, maxWater);
            OnPlayerShoot?.Invoke();

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
                    bullet.Offset = new Vector2(0, Random.Range(-1f, 1f) * 0.25f);
                    bullet.transform.SetAsLastSibling();
                    bullet.gameObject.SetActive(true);
                    return;
                }
            }
            Projectile first = projectileManager.GetChild(0).GetComponent<Projectile>();
            first.gameObject.SetActive(false);
            first.transform.position = ProjectilePos;
            first.facingRight = facingRight;
            first.Offset = new Vector2(0, Random.Range(-1f, 1f) * 0.25f);
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
            // Walljumps if on wall
            bool wallJumpInput = input.x != 0 || oldInput.x != 0;
            bool wallJumpReady = onWall || !wallCoyote.IsComplete;
            if (!onGround && wallJumpInput && wallJumpReady)
            {
                Vector2 wallJump = new Vector2(wallSide * 0.75f, 1);
                if (input.x + wallSide > 0) wallJump = new Vector2(wallSide * 0.65f, 0.75f);
                vel = wallJump * jumpStrength;
                onWall = false;
                wallCoyote.Stop();
                wallCooldown.Start();

                OnPlayerWallJump?.Invoke();
            }

            // Jumps if on ground
            else if (onGround || !jumpCoyote.IsComplete)
            {
                vel.y = jumpStrength;
                onGround = false;
                jumpCoyote.Stop();
                jumpCooldown.Start();

                OnPlayerJump?.Invoke();
            }

            // Double jumps if in air
            else if (jumpCount < airJumps && currentWater >= 10)
            {
                vel.y = jumpStrength;
                onGround = false;
                jumpCount++;
                currentWater -= 10;

                //waterGauge.UpdateBar(currentWater, maxWater);
                OnPlayerAirJump?.Invoke();
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
            // Change state to dashing and reset vertical speed
            state = PlayerState.Dashing;
            vel.y = 0;

            // Set dashing speed
            speed = dashingSpeed;
            currentWater -= 10;
            //waterGauge.UpdateBar(currentWater, maxWater);
            OnPlayerDash?.Invoke();
        }

        if (state != PlayerState.Stopped && state != PlayerState.Dead)
            hasStarted = true;
    }

    private void OnQuack(InputValue value)
    {
        OnPlayerQuack?.Invoke();
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

    private void ComputeCollisions()
    {
        if (state == PlayerState.Stopped) return;

        Dictionary<CollisionEvent, bool> events = col.CalculateCollision();

        // If on ground
        if (events[CollisionEvent.Land])
        {
            onGround = true;
            jumpCount = 0;
            if (vel.y < 0) vel.y = 0;
        }
        else
        {
            if (onGround) OnPlayerLand?.Invoke();
            onGround = false;
        }

        // If on wall
        if (events[CollisionEvent.Wall])
        {
            onWall = true;
            wallSide = events[CollisionEvent.WallSide] ? 1 : -1;
        }
        else
        {
            onWall = false;
        }

        // When bumping head, kill vertical velocity
        if (events[CollisionEvent.Roof])
            if (vel.y > 0) vel.y = 0;

        // When on ground but not 100% on, slide off
        if (events[CollisionEvent.Slide])
            onGround = false;

        // When dashing against wall, bonk off
        if (events[CollisionEvent.WallBonk])
        {
            speed = baseSpeed * -1;
            vel.x = speed * (facingRight ? 1 : -1);

            // Cancel dash
            dashingTimer = dashingTime;
            iFramesTimer = iFrames;
            dashingCooldownTimer = dashingCooldown;
            state = PlayerState.Normal;
        }

        // When drifting against wall, reduce lateral velocity
        if (events[CollisionEvent.WallLat])
        {
            if (wallCooldown.IsComplete && state != PlayerState.Dashing)
            {
                speed = baseSpeed;
                vel.x *= Mathf.Pow(deccel, 50 * Time.deltaTime);
                if (Mathf.Abs(vel.x) < baseSpeed * 0.1f) vel.x = 0;
            }
        }

        // When rising against wall, reduce vertical velocity
        if (events[CollisionEvent.WallVert])
        {
            if (jumpCooldown.IsComplete)
            {
                if (vel.y > 0 && Mathf.Abs(vel.x) >= baseSpeed)
                    vel.y *= Mathf.Pow(airDeccel, 50 * Time.deltaTime);
            }
        }
    }

    public void RefillWater(float amount)
    {
        currentWater += amount;
        //waterGauge.UpdateBar(currentWater, maxWater);
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
                if (health <= 0) Die();
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
        if (transitionAnimator != null)
            transitionAnimator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }
}
