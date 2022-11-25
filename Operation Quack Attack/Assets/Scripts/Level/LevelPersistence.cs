using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPersistence : MonoBehaviour
{
    // Dictionaries to store instances and restart flags for each inheritor
    public static LevelPersistence Instance;
    public static bool RestartFlag;

    // Level of this instance of persistent
    [SerializeField] private int levelNumber;

    public delegate void PersistentEvent();
    public event PersistentEvent OnReload;
    public event PersistentEvent OnStart;

    public int Level { get => levelNumber; }

    /// <summary>
    /// On awake, ensure only one instance of Persistent can exist.
    /// </summary>
    protected virtual void Awake()
    {
        // Destroy new persistent if persistent of same level already exist
        if (Instance != null && Level == Instance.Level)
        {
            Destroy(gameObject);
            return;
        }

        // Attach onload event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Every time a scene is loaded, ensures that this persistent persists
    /// until a new level is loaded.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Keep persistent if persistent match current level
        if (Instance == null && Level == LevelManager.CurrentLevel)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        // Destroy old persistent if it doesn't, or if you're in the main menu
        else if (Level != LevelManager.CurrentLevel || SceneManager.GetActiveScene().buildIndex == 0)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
            return;
        }

        OnReload?.Invoke();
        RestartFlag = false;
    }

    /// <summary>
    /// On start, sets own restart flag as false.
    /// </summary>
    protected virtual void Start()
    {
        OnStart?.Invoke();
        RestartFlag = false;
    }

    /// <summary>
    /// Initialize persistent of specified type by setting its restart flag.
    /// </summary>
    public static void Initialize()
    {
        RestartFlag = true;
    }
}
