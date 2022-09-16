using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject menu;

    public void PlayNowButton()
    {
        // Play Game
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuackAttack");
    }

    public void QuitButton()
    {
        // Quit Game
        Application.Quit();
    }
}