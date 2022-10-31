using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using static HUDTimer;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI time;

    public GameObject mainmenuButton;
    public GameObject nextLevelButton;

    public int levelNum;

    // Start is called before the first frame update
    void Start()
    {
        int min = Mathf.FloorToInt(timer / 60.0f);
        int sec = Mathf.FloorToInt(timer - min * 60);
        int mil = Mathf.FloorToInt((timer - (min * 60 + sec)) * 100);
        string formattedTime = $"{min:00}:{sec:00}.{mil:00}";

        score.text = "Score: 0";
        time.text = $"Time: {formattedTime}";

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainmenuButton);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevelButton()
    {
        levelNum++;
        levelNum.ToString();
        Player.Initialize();
        SceneManager.LoadScene(levelNum);
        Debug.Log("Loading Next Level");
    }
}
