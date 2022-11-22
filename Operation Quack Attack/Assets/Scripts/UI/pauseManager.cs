using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class pauseManager : MonoBehaviour
{
    public GameObject FirstOption;
    public GameObject RestartButton;
    public GameObject CheckpointButtons;

    public GameObject pauseObj;
    public GameObject player;

    private LevelManager levelManager;

    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        if (RestartButton != null)
            RestartButton.SetActive(!LevelManager.GamemodeCheckpoints);

        if (CheckpointButtons != null)
            CheckpointButtons.SetActive(LevelManager.GamemodeCheckpoints);

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
        if (!LevelManager.GamemodeCheckpoints)
            HUDTimer.Initialize();
        player.SetActive(true);
        levelManager.Respawn();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1.0f;
        player.SetActive(true);
        levelManager.RestartLevel();
    }


    public void ReturnToGane()
    {
        Time.timeScale = 1.0f;
        player.SetActive(true);
        pauseObj.SetActive(false);

    }

    private void OnPauseToggleOff(InputValue value)
    {
        Time.timeScale = 1.0f;
        player.SetActive(true);
        pauseObj.SetActive(false);
    }

}
