using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class pauseManager : MonoBehaviour
{
    public GameObject FirstOption;
    public GameObject pauseObj;
    public GameObject player;

    private LevelManager levelManager;

    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstOption);
    }

    public void BackToMain()
    {
        Time.timeScale = 1.0f;
        levelManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        levelManager.Respawn();
        player.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        levelManager.RestartLevel();
        player.SetActive(true);
    }


    public void ReturnToGane()
    {
        Time.timeScale = 1.0f;
        player.SetActive(true);
        pauseObj.SetActive(false);

    }
}
