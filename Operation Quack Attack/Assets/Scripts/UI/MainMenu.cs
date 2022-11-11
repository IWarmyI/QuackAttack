using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static AudioSliders;

public class MainMenu : MonoBehaviour
{
    public GameObject main;
    public GameObject difficulty;
    public GameObject options;
    public GameObject credits;

    public GameObject mainMenuFirstOption;
    public GameObject DifficultyFirstOption;
    public GameObject OptionsFirstOption;
    public GameObject CreditsFirstObject;

    public GameObject canvas;

    public Animator animator;
    public float transitionDelayTime = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
        Time.timeScale = 1.0f;
        canvas.GetComponent<AudioSource>().volume = musicFloat;
    }

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    void Update()
    {
        canvas.GetComponent<AudioSource>().volume = musicFloat;
    }

    public void DifficultyButton()
    {
        // Play Game
        Player.Initialize();
        SceneManager.LoadScene("Game");
    }

    public void TutorialButton()
    {
        //Play Tutorial
        Player.Initialize();
        //SceneManager.LoadScene("TutorialChallenge");
        StartCoroutine(DelayLoadLevel("TutorialChallenge"));
    }

    public void StartButton()
    {
        // Show Main Menu
        options.SetActive(false);
        credits.SetActive(false);
        main.SetActive(false);
        difficulty.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(DifficultyFirstOption);
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        options.SetActive(false);
        credits.SetActive(false);
        difficulty.SetActive(false);
        main.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainMenuFirstOption);

    }

    public void OptionsButton()
    {
        // Show Options
        main.SetActive(false);
        credits.SetActive(false);
        difficulty.SetActive(false);
        options.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(OptionsFirstOption);
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        main.SetActive(false);
        options.SetActive(false);
        difficulty.SetActive(false);
        credits.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CreditsFirstObject);
    }

    public void SecondTutorialButton()
    {
        StartCoroutine(DelayLoadLevel("TutorialChallenge"));
    }

    IEnumerator DelayLoadLevel(string sceneName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }
}