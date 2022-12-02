using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using static HUDTimer;
using static AudioSliders;
using System;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI dev;
    public TextMeshProUGUI time;

    public GameObject mainmenuButton;
    public GameObject nextLevelButton;

    public GameObject canvas;
    private AchievementManager achievement;

    private void Awake()
    {
        achievement = AchievementManager.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        string yourTime = FormatTime(timer);
        string devTime = FormatTime(LevelManager.DevTime);

        dev.text = $"Dev Time: {devTime}";
        time.text = $"Your Time: {yourTime}";

        nextLevelButton.SetActive(!LevelManager.IsLastLevel);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(nextLevelButton.activeSelf ? nextLevelButton : mainmenuButton);

        canvas.GetComponent<AudioSource>().volume = musicFloat;

        // HUDTimer.timer is in seconds
        if (LevelManager.GamemodeCheckpoints == false)
        {
            achievement.Unlock("NoCheckpoints");
        }
        if (timer < LevelManager.DevTime)
        {
            achievement.Unlock("BeatDev");
        }
    }

    public void MainMenuButton()
    {
        LevelManager.Instance.MainMenu();
    }

    public void NextLevelButton()
    {
        LevelManager.Instance.NextLevel();
    }

    public void RestartLevelButton()
    {
        LevelManager.Instance.RestartLevel();
    }

    private string FormatTime(float value)
    {
        int min = (int)(value / 6000);
        int sec = (int)(value / 100 - min * 60);
        int dec = (int)(value - (min * 6000 + sec * 100));
        return $"{min:00}:{sec:00}.{dec:00}";
    }
}
