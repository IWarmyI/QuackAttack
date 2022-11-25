using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private LevelPersistence persist;

    private  void Awake()
    {
        persist = GetComponentInParent<LevelPersistence>();
        persist.OnStart += OnStart;
        persist.OnReload += RestartCheckpoints;
    }

    void RestartCheckpoints()
    {
        // Restart checkpoints if desired
        foreach (Checkpoint cp in checkpoints)
        {
            if (!LevelManager.GamemodeCheckpoints)
                cp.gameObject.SetActive(false);

            if (LevelPersistence.RestartFlag)
                cp.Restart();
            else if (cp.IsActivated)
                cp.SetComplete();
        }
    }

    // Start is called before the first frame update
    private void OnStart()
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
}
