using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSign : Checkpoint
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activatedSprite;

    private SpriteRenderer spr;
    private ParticleSystem ps;
    private GameObject notif;

    private void Start()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        notif = GetComponentInChildren<Animator>().gameObject;

        spr.enabled = true;
        spr.sprite = defaultSprite;
        notif.SetActive(false);

        OnActivated += AnimateFlip;
    }

    public override void SetComplete()
    {
        base.SetComplete();
        spr.enabled = true;
        spr.sprite = activatedSprite;
    }

    public override void Restart()
    {
        base.Restart();
        spr.enabled = true;
        spr.sprite = defaultSprite;
        notif.SetActive(false);
    }

    private void AnimateFlip(Checkpoint cp)
    {
        spr.enabled = false;
        notif.SetActive(true);
        ps.Emit(1);
    }
}
