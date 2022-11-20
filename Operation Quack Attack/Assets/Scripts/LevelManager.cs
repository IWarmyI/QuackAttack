using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static int CurrentLevel = -1;
    public const int FirstLevel = 1;
    public const int NumberOfLevels = 1;
    public static bool IsLastLevel = false;
    public static bool IsInGame = false;
    public static string[] MenuLevels = { "MainMenu", "EndLevel", "GameOver" };

    public static bool GamemodeCheckpoints = true;

    public static LevelManager Instance;

    public Animator transitionAnimator;
    public float transitionDelayTime = 1.0f;
    public static bool IsLoading;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

        IsLastLevel = CurrentLevel + 1 == FirstLevel + NumberOfLevels;

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
        LoadNewLevel(CurrentLevel);
    }

    public void NextLevel()
    {
        LoadNewLevel(CurrentLevel + 1);
    }

    public void Respawn()
    {
        StartCoroutine(DelayLoadLevel(CurrentLevel));
    }

    public void LoadNewLevel(int level, bool transition = true)
    {
        Player.Initialize();
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