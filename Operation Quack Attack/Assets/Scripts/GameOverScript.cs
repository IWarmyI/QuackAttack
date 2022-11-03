using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject firstOption;

    public Animator animator;
    public float transitionDelayTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        StartCoroutine(DelayLoadLevel("TutorialChallenge"));
    }

    public void BacktoMainMenu()
    {
        StartCoroutine(DelayLoadLevel("MainMenu"));
    }

    IEnumerator DelayLoadLevel(string sceneName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }
}
