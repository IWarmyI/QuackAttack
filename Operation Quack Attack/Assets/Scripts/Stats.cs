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

    public Animator animator;
    public float transitionDelayTime = 1.0f;

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

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    public void MainMenuButton()
    {
        StartCoroutine(DelayLoadLevel("MainMenu"));
    }

    public void NextLevelButton()
    {
        levelNum++;
        levelNum.ToString();
        Player.Initialize();
        StartCoroutine(DelayLoadNextLevel(levelNum));
        Debug.Log("Loading Next Level");
    }

    IEnumerator DelayLoadNextLevel(int levelNum)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(levelNum);
    }

    IEnumerator DelayLoadLevel(string levelName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(levelName);
    }
}
