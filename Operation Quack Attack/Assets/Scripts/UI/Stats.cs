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
    public TextMeshProUGUI time;

    public GameObject mainmenuButton;
    public GameObject nextLevelButton;

    public GameObject canvas;
    [SerializeField] AchievementManager achievement;

    // Start is called before the first frame update
    void Start()
    {
        int min = Mathf.FloorToInt(timer / 60.0f);
        int sec = Mathf.FloorToInt(timer - min * 60);
        int mil = Mathf.FloorToInt((timer - (min * 60 + sec)) * 100);
        string formattedTime = $"{min:00}:{sec:00}.{mil:00}";

        time.text = $"Your Time: {formattedTime}";

        nextLevelButton.SetActive(!LevelManager.IsLastLevel);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(nextLevelButton.activeSelf ? nextLevelButton : mainmenuButton);

        canvas.GetComponent<AudioSource>().volume = musicFloat;

        if ( Convert.ToInt32(formattedTime) < 000811)
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
}
