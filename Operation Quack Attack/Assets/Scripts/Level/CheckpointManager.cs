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
        if (Instance != null && Level == Instance.Level)
        {
            Destroy(gameObject);
            return;
        }

        if (Level == LevelManager.CurrentLevel)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        checkpoints.Clear();
        foreach (Transform child in transform)
        {
            Checkpoint cp = child.GetComponent<Checkpoint>();
            checkpoints.Add(cp);
            cp.OnActivated += UpdateCheckpoints;

            if (RestartFlag)
            {
                cp.Restart();
                RestartFlag = false;
            }
        }
    }

    public static void Initialize()
    {
        RestartFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateCheckpoints(Checkpoint e)
    {
        int last = checkpoints.IndexOf(e);
        for (int i = 0; i < last; i++)
        {
            checkpoints[i].SetComplete();
        }
    }
}
