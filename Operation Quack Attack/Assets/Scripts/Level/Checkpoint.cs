using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] protected Vector2 respawnOffset = Vector2.zero;
    protected bool isActivated = false;

    public Vector2 RespawnPosition { get => (Vector2) transform.position + respawnOffset; }
    public bool IsActivated { get => isActivated; set => isActivated = value; }

    public delegate void CheckpointEvent(Checkpoint cp);
    public event CheckpointEvent OnActivated;

    public virtual void Activate()
    {
        if (isActivated) return;

        SetComplete();
        Player.SetRespawn(RespawnPosition);
        OnActivated?.Invoke(this);
    }

    public virtual void SetComplete()
    {
        isActivated = true;
    }

    public virtual void Restart()
    {
        isActivated = false;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isActivated) return;
        if (collider.isTrigger) return;

        if (collider.gameObject.CompareTag("Player"))
        {
            Player player = collider.gameObject.GetComponentInParent<Player>();
            if (player.State == Player.PlayerState.Dead) return;

            Activate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(
            RespawnPosition + new Vector2(-0.5f, -0.5f),
            RespawnPosition + new Vector2(0.5f, -0.5f));
        Gizmos.DrawLine(
            RespawnPosition + Vector2.up * 0.5f,
            RespawnPosition + Vector2.down * 0.5f);
    }
}
