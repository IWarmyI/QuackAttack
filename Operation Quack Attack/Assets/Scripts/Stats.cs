using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
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
        score.text = "Score: 0";
        time.text = $"Time: {timer}";
    }

    public void BackMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevelButton()
    {
        levelNum++;
        levelNum.ToString();
        SceneManager.LoadScene(levelNum);
        Debug.Log("Loading Next Level");
    }
}
