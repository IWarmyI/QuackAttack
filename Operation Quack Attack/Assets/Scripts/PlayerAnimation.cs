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

    private Color high = new Color(0, 2.0f / 3.0f, 1);

    private Animator anim;
    private SpriteRenderer spr;
    private TrailRenderer trail;

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
}
