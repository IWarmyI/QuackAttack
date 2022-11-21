using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerAnimation : MonoBehaviour
{
    // Player Animation Parameters
    private AnimState _animState = AnimState.Idle;
    private bool _onGround = false;
    private bool _onWall = false;
    private bool _isFalling = false;
    private bool _facing = false;
    private bool _isTopSpeed = false;
    [SerializeField] private float speedMultiplier = 1.25f;

    // Player Components
    private Player player;
    private Animator anim;
    private SpriteRenderer spr;
    private Dictionary<AnimState, bool> complete = new();

    // Dash Ready Flash
    [SerializeField] private Material defaultMat;
    [SerializeField] private Material flashMat;
    [SerializeField] private Color flashColor = new Color(0.5f, 0.5f, 0.5f);
    private const float flashTime = 0.2f;
    private float flashTimer = 0.2f;

    // Particle Systems
    private ParticleSystem psDash;
    private ParticleSystem psJump;
    private ParticleSystem psShoot;
    private ParticleSystem psAfter;
    private ParticleSystem psTrail;
    private ParticleSystemRenderer psrAfter;

    private ParticleSystem psDustJump;
    private ParticleSystem psWaveLand;
    private ParticleSystem psDustLand;
    private ParticleSystem psDustRun;
    private ParticleSystem psDustWall;

    private Vector3 rotate180 = new Vector3(0, 180, 0);


    void Start()
    {
        player = GetComponentInParent<Player>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        GameObject waterFx = transform.GetChild(0).gameObject;
        psDash = waterFx.GetComponentsInChildren<ParticleSystem>()[0];
        psJump = waterFx.GetComponentsInChildren<ParticleSystem>()[1];
        psShoot = waterFx.GetComponentsInChildren<ParticleSystem>()[2];
        psAfter = waterFx.GetComponentsInChildren<ParticleSystem>()[3];
        psTrail = waterFx.GetComponentsInChildren<ParticleSystem>()[4];
        psrAfter = psAfter.GetComponent<ParticleSystemRenderer>();

        GameObject dustFx = transform.GetChild(1).gameObject;
        psDustJump = dustFx.GetComponentsInChildren<ParticleSystem>()[0];
        psWaveLand = dustFx.GetComponentsInChildren<ParticleSystem>()[1];
        psDustLand = dustFx.GetComponentsInChildren<ParticleSystem>()[2];
        psDustRun = dustFx.GetComponentsInChildren<ParticleSystem>()[3];
        psDustWall = dustFx.GetComponentsInChildren<ParticleSystem>()[4];

        player.OnPlayerLand += LandFX;
        player.OnPlayerJump += JumpFX;
        player.OnPlayerWallJump += JumpFX;
        player.OnPlayerAirJump += AirJumpFX;
        player.OnPlayerDash += DashFX;
        player.OnPlayerDashReady += FlashFX;
        player.OnPlayerShoot += ShootFX;
        player.OnPlayerTopSpeed += RunFX; 
    }

    private void Update()
    {
        if (flashTimer < flashTime)
        {
            flashTimer += Time.deltaTime;

            if (flashTimer >= flashTime)
            {
                spr.material = defaultMat;
                spr.material.color = Color.white;
            }
        }

    }

    public void Animate()
    {
        anim.SetInteger("PlayerState", (int)_animState);
        anim.SetBool("OnGround", _onGround);
        anim.SetBool("OnWall", _onWall);
        anim.SetBool("IsFalling", _isFalling);
        anim.SetFloat("Speed", _isTopSpeed ? speedMultiplier : 1);

        if (!(_onWall && _animState == AnimState.Air))
        {
            spr.flipX = !_facing;
        }

        if (_onWall && !_onGround)
        {
            if (!psDustWall.isPlaying)
                StopStart(psDustWall);
        }
        else
        {
            if (psDustWall.isPlaying)
                psDustWall.Stop();
        }

        psDash.transform.rotation  = Quaternion.Euler(_facing ? Vector3.zero : rotate180);
        psShoot.transform.rotation = Quaternion.Euler(_facing ? Vector3.zero : rotate180);
        psDustWall.transform.rotation = Quaternion.Euler(_facing ? Vector3.zero : rotate180);
        psrAfter.flip = _facing ? Vector2.zero : Vector2.right;
        if (!_onGround && psDustRun.isPlaying)
            psDustRun.Stop();
    }

    public void Animate(AnimState animState, bool onGround, bool onWall, bool isFalling, bool isTopSpeed, bool facing)
    {
        _animState = animState;
        _onGround = onGround;
        _onWall = onWall;
        _isFalling = isFalling;
        _isTopSpeed = isTopSpeed;
        _facing = facing;
        Animate();
    }

    public bool IsComplete(AnimState animState)
    {
        if (complete.TryGetValue(animState, out bool value))
        {
            if (value) complete[animState] = false;
            return value;
        }
        return false;
    }

    public void OnAnimEnter(AnimState animState)
    {
        //complete[animState] = false;
    }
    public void OnAnimExit(AnimState animState)
    {
        complete[animState] = true;
    }

    private void FlashFX()
    {
        spr.material = flashMat;
        spr.material.color = flashColor;
        flashTimer = 0;
    }
    private void JumpFX()
    {
        StopStart(psDustJump);
    }
    private void AirJumpFX()
    {
        StopStart(psJump);
        StopStart(psTrail);
    }
    private void DashFX()
    {
        StopStart(psDash);
        StopStart(psAfter);
    }
    private void ShootFX()
    {
        StopStart(psShoot);
    }
    private void LandFX()
    {
        StopStart(psWaveLand);
        StopStart(psDustLand);
    }
    private void RunFX()
    {
        StopStart(psDustRun);
    }

    private void StopStart(ParticleSystem ps)
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        ps.Play();
    }
}
