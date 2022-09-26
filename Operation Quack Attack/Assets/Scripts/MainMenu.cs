using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject main;
    public GameObject options;
    public GameObject credits;

    // Start is called before the first frame update
    void Start()
    {
        MainMenuButton();
    }

    public void StartButton()
    {
        // Play Game
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void TutorialButton()
    {
        //Play Tutorial
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }

    public void MainMenuButton()
    {
        // Show Main Menu
        options.SetActive(false);
        credits.SetActive(false);
        main.SetActive(true);
    }

    public void OptionsButton()
    {
        // Show Options
        main.SetActive(false);
        credits.SetActive(false);
        options.SetActive(true);
    }

    public void CreditsButton()
    {
        // Show Credits Menu
        main.SetActive(false);
        options.SetActive(false);
        credits.SetActive(true);
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}