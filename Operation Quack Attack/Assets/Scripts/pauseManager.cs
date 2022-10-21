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

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstOption);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToGane()
    {
        Time.timeScale = 1.0f;
        pauseObj.SetActive(false);

    }
}
