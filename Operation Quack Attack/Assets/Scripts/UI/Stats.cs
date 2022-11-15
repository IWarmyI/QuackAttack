using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using static HUDTimer;
using static AudioSliders;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public GameObject mainmenuButton;
    public GameObject nextLevelButton;

    public GameObject canvas;

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        int min = Mathf.FloorToInt(timer / 60.0f);
        int sec = Mathf.FloorToInt(timer - min * 60);
        int mil = Mathf.FloorToInt((timer - (min * 60 + sec)) * 100);
        string formattedTime = $"{min:00}:{sec:00}.{mil:00}";

        score.text = "Score: 0";
        time.text = $"Time: {formattedTime}";

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainmenuButton);

        if (LevelManager.IsLastLevel)
            nextLevelButton.SetActive(false);
        else
            nextLevelButton.SetActive(true);

        canvas.GetComponent<AudioSource>().volume = musicFloat;
    }

    public void MainMenuButton()
    {
        levelManager.MainMenu();
    }

    public void NextLevelButton()
    {
        levelManager.NextLevel();
    }

    public void RestartLevelButton()
    {
        levelManager.RestartLevel();
    }
}
