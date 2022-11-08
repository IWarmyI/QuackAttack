using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerAnimation : MonoBehaviour
{
    // Animation
    private AnimState _animState = AnimState.Idle;
    private bool _onGround = false;
    private bool _onWall = false;
    private bool _isFalling = false;
    private bool _facing = false;
    private bool _isTopSpeed = false;
    [SerializeField] private float speedMultiplier = 1.25f;

    private Animator anim;
    private SpriteRenderer spr;
    private Dictionary<AnimState, bool> complete = new();

    [SerializeField] private Material defaultMat;
    [SerializeField] private Material flashMat;
    [SerializeField] private Color flashColor = new Color(0.5f, 0.5f, 0.5f);
    private const float flashTime = 0.2f;
    private float flashTimer = 0.2f;

    private GameObject fxDash;
    private TrailRenderer trail;
    private ParticleSystem psDash;

    private GameObject fxJump;
    private ParticleSystem psJump;


    void Start()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
        psDash = transform.GetComponentsInChildren<ParticleSystem>()[0];
        psJump = transform.GetComponentsInChildren<ParticleSystem>()[1];
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

        if (trail != null)
        {
            if (_animState == AnimState.Dash)
            {
                trail.emitting = true;
            }
            else
            {
                trail.emitting = false;
            }
        }
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

    public void PlayFlash()
    {
        spr.material = flashMat;
        spr.material.color = flashColor;
        flashTimer = 0;
    }

    public void PlayJump()
    {
        psJump.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        psJump.Play();
    }

    public void PlayDash()
    {
        psDash.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        psDash.Play();
    }
}
