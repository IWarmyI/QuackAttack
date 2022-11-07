using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [Header("Shooting")]
    [SerializeField] private int bulletID;
    [SerializeField] private BulletManager bulletManager;
    [SerializeField] private Vector2 bulletOffset = Vector2.zero;
    [SerializeField] private float fireRate = 0.2f;
    private float fireTimer = 0.2f;

    [SerializeField] private float visionRadius;
    [SerializeField] private GameObject target;

    private Vector2 BulletPos
    {
        get
        {
            return pos + new Vector2(
                bulletOffset.x * (facingRight ? 1 : -1),
                bulletOffset.y
                );
        }
    }
    private Vector2 Aim
    {
        get
        {
            if (target == null) return facingRight ? Vector2.right : Vector2.left;
            Vector2 diff = (Vector2)target.transform.position - pos;
            return diff.normalized;
        }
    }
    private float FireTime { get { return 1.0f / fireRate; } }

    protected override void Start()
    {
        base.Start();
        if (target == null) target = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (target == null) return;
        float distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance <= visionRadius)
        {
            if (fireTimer >= FireTime)
            {
                ShootProjectile();
                fireTimer -= FireTime;
            }
        }

        if (fireTimer < FireTime)
        {
            fireTimer += Time.deltaTime;
        }
    }

    private void ShootProjectile()
    {
        bulletManager.SpawnBullet(gameObject, bulletID, BulletPos, Aim);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
