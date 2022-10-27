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
    private bool _isFalling = false;
    private bool _facing = false;
    private bool _isTopSpeed = false;
    [SerializeField] private float speedMultiplier = 1.25f;

    private Animator anim;
    private SpriteRenderer spr;
    private TrailRenderer trail;
    private Dictionary<AnimState, bool> complete = new();

    void Start()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    public void Animate()
    {
        anim.SetInteger("PlayerState", (int)_animState);
        anim.SetBool("OnGround", _onGround);
        anim.SetBool("IsFalling", _isFalling);
        anim.SetFloat("Speed", _isTopSpeed ? speedMultiplier : 1);
        spr.flipX = !_facing;

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

    public void Animate(AnimState animState, bool onGround, bool isFalling, bool isTopSpeed, bool facing)
    {
        _animState = animState;
        _onGround = onGround;
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
        Debug.Log($"End!");
    }
}
