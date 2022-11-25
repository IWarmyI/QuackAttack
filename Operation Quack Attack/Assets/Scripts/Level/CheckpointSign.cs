using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSign : Checkpoint
{
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite activatedSprite;

    [SerializeField] private AudioClip sfx;

    private SpriteRenderer spr;
    private ParticleSystem ps;
    private GameObject notif;
    private AudioSource source;

    private void Start()
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        ps = GetComponentInChildren<ParticleSystem>();
        notif = GetComponentInChildren<Animator>().gameObject;

        source = GetComponentInChildren<AudioSource>();
        source.volume = AudioSliders.sfxFloat;

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
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
        source.PlayOneShot(sfx);
    }
}
