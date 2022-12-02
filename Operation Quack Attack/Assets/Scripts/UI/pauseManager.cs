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
    public GameObject transition;

    void Start()
    {
        if (RestartButton != null)
            RestartButton.SetActive(!LevelManager.GamemodeCheckpoints);

        if (CheckpointButtons != null)
            CheckpointButtons.SetActive(LevelManager.GamemodeCheckpoints);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstOption);
    }



    public void BackToMain()
    {
        //transition.SetActive(true);
        Time.timeScale = 1.0f;
        LevelManager.Instance.LoadScene("MainMenu");
    }

    public void Restart()
    {
        //transition.SetActive(true);
        Time.timeScale = 1.0f;
        if (!LevelManager.GamemodeCheckpoints)
            HUDTimer.Initialize();
        player.SetActive(true);
        LevelManager.Instance.Respawn();
    }

    public void RestartLevel()
    {
        //transition.SetActive(true);
        Time.timeScale = 1.0f;
        player.SetActive(true);
        LevelManager.Instance.RestartLevel();
    }


    public void ReturnToGane()
    {
        //transition.SetActive(true);
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
