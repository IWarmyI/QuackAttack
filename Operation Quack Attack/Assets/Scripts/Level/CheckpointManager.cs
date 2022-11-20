using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : LevelPersistent
{
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    protected override void Awake()
    {
        base.Awake();
        OnReload += RestartCheckpoints;
    }

    void RestartCheckpoints()
    {
        // Restart checkpoints if desired
        foreach (Checkpoint cp in checkpoints)
        {
            if (!LevelManager.GamemodeCheckpoints)
                cp.gameObject.SetActive(false);

            if (RestartFlag)
                cp.Restart();
            else if (cp.IsActivated)
                cp.SetComplete();
        }
        RestartFlag = false;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        checkpoints.Clear();
        foreach (Transform child in transform)
        {
            Checkpoint cp = child.GetComponent<Checkpoint>();

            if (!LevelManager.GamemodeCheckpoints)
            {
                cp.gameObject.SetActive(false);
            }
            else
            {
                checkpoints.Add(cp);
                cp.OnActivated += UpdateCheckpoints;
            }
        }

        base.Start();
    }

    // Activates all checkpoints before the furthest checkpoint touched
    private void UpdateCheckpoints(Checkpoint e)
    {
        int last = checkpoints.IndexOf(e);
        for (int i = 0; i < last; i++)
        {
            checkpoints[i].SetComplete();
        }
    }

    public static void Initialize()
    {
        Initialize(typeof(CheckpointManager));
    }
}
