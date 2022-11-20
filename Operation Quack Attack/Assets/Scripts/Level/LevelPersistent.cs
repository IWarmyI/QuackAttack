using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelPersistent : MonoBehaviour
{
    public static Dictionary<Type, LevelPersistent> instances = new();
    public static Dictionary<Type, bool> restartFlags = new();

    [SerializeField] private int levelNumber;

    public delegate void PersistentEvent();
    public event PersistentEvent OnReload;

    public int Level { get => levelNumber; }
    public LevelPersistent Instance
    {
        get
        {
            bool value = instances.TryGetValue(GetType(), out LevelPersistent lp);
            if (value) return lp;
            return null;
        }
        set => instances[GetType()] = value;
    }
    public bool RestartFlag
    {
        get
        {
            bool value = restartFlags.TryGetValue(GetType(), out bool rf);
            if (value) return rf;
            return true;
        }
        set => restartFlags[GetType()] = value;
    }

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
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (RestartFlag) RestartFlag = false;
    }


    // Restart this persistant
    protected static void Initialize(Type t)
    {
        restartFlags[t] = true;
    }
}
