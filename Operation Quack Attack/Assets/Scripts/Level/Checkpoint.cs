using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Vector2 respawnOffset = Vector2.zero;
    private bool isActivated = false;

    public Vector2 RespawnPosition { get => (Vector2) transform.position + respawnOffset; }
    public bool IsActivated { get => isActivated; set => isActivated = value; }

    public delegate void CheckpointEvent(Checkpoint cp);
    public event CheckpointEvent OnActivated;

    public void Activate()
    {
        if (isActivated) return;

        SetComplete();
        Player.Respawn(RespawnPosition);
        OnActivated?.Invoke(this);
    }

    public void SetComplete()
    {
        isActivated = true;
        gameObject.SetActive(false);
    }

    public void Restart()
    {
        isActivated = false;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isActivated) return;

        if (collider.gameObject.CompareTag("Player"))
        {
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
