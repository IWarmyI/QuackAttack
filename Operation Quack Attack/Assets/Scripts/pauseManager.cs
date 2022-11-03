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

    public Animator animator;
    public float transitionDelayTime = 1.0f;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(FirstOption);
    }

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    public void BackToMain()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(DelayLoadLevel("MainMenu"));
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(DelayLoadLevelNum((SceneManager.GetActiveScene().buildIndex)));
        player.SetActive(true);
    }

    public void ReturnToGane()
    {
        Time.timeScale = 1.0f;
        player.SetActive(true);
        pauseObj.SetActive(false);

    }

    IEnumerator DelayLoadLevel(string sceneName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator DelayLoadLevelNum(int levelNum)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(levelNum);
    }
}
