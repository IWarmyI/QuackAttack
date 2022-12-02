using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Static
    public static LevelManager Instance;

    public static bool GamemodeCheckpoints = true;

    public static int CurrentLevel = -1;
    public static bool IsLastLevel = false;
    public static bool IsInGame = false;
    public static int FirstLevel { get => Instance.firstLevel; }
    public static int NumberOfLevels { get => Instance.numberOfLevels; }
    public static string[] MenuLevels { get => Instance.menuScenes; }
    public static float[] DevTimes { get => Instance.devTimes; }
    public static float DevTime {
        get
        {
            if (CurrentLevel - FirstLevel >= DevTimes.Length) return 0;
            return DevTimes[CurrentLevel - FirstLevel];
        }
    }

    // Instance
    public Animator transitionAnimator;
    public float transitionDelayTime = 1.0f;
    public static bool IsLoading;
    [NonSerialized] public GameObject transition;

    [Header("Global (Only Edit in Prefab)")]
    [Tooltip("Build index of first level's scene.")]
    public int firstLevel = 1;
    [Tooltip("Number of playable levels.")]
    public int numberOfLevels = 1;
    public string[] menuScenes = { "MainMenu", "EndLevel", "GameOver" };
    public float[] devTimes = { 811f, 1000f };

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transition = GameObject.Find("Transition");
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = SceneManager.GetActiveScene().name;

        if (!MenuLevels.Contains(name))
        {
            CurrentLevel = SceneManager.GetActiveScene().buildIndex;
            IsInGame = false;
        }
        else
        {
            IsInGame = true;
        }

        IsLastLevel = CurrentLevel + 1 >= FirstLevel + NumberOfLevels;

        //Debug.Log($"Current Level: {CurrentLevel}");
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(DelayLoadLevel(sceneName));
    }

    public void MainMenu()
    {
        LoadScene("MainMenu");
    }

    public void NewGame()
    {
        LoadNewLevel(FirstLevel);
    }

    public void RestartLevel()
    {
        LoadNewLevel(CurrentLevel, GamemodeCheckpoints);
    }
    public void RestartLevel(bool showLevelPan)
    {
        LoadNewLevel(CurrentLevel, showLevelPan);
    }

    public void NextLevel()
    {
        LoadNewLevel(CurrentLevel + 1);
    }

    public void Respawn()
    {
        StartCoroutine(DelayLoadLevel(CurrentLevel));
    }

    public void LoadNewLevel(int level, bool showLevelPan = true, bool transition = true)
    {
        Player.Initialize(showLevelPan);
        CurrentLevel = level;
        if (transition)
            StartCoroutine(DelayLoadLevel(level));
        else
            SceneManager.LoadScene(level);
    }

    private IEnumerator DelayLoadLevel(int sceneID)
    {
        if (!IsLoading)
        {
            if (transitionAnimator != null)
            transitionAnimator.SetTrigger("TriggerTransition");
            IsLoading = true;
            yield return new WaitForSeconds(transitionDelayTime);
            IsLoading = false;
            SceneManager.LoadScene(sceneID);
        }
    }

    private IEnumerator DelayLoadLevel(string sceneName)
    {
        if (!IsLoading)
        {
            if (transitionAnimator != null)
                transitionAnimator.SetTrigger("TriggerTransition");
            IsLoading = true;
            yield return new WaitForSeconds(transitionDelayTime);
            IsLoading = false;
            SceneManager.LoadScene(sceneName);
        }
    }
}
