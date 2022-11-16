using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public static bool RestartFlag = true;

    [SerializeField] private int levelNumber;
    private List<Checkpoint> checkpoints = new List<Checkpoint>();

    public int Level { get => levelNumber; }

    private void Awake()
    {
        // Destroy new checkpoints if checkpoints of same level already exist
        if (Instance != null && Level == Instance.Level)
        {
            Destroy(gameObject);
            return;
        }

        // Attach onload event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Keep checkpoints if checkpoints match current level
        if (Instance == null && Level == LevelManager.CurrentLevel)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Destroy old checkpoints if it doesn't, or if you're in the main menu
        else if (Level != LevelManager.CurrentLevel || SceneManager.GetActiveScene().buildIndex == 0)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }

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
    void Start()
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
        if (RestartFlag) RestartFlag = false;
    }

    // Restart all checkpoints
    public static void Initialize()
    {
        RestartFlag = true;
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
