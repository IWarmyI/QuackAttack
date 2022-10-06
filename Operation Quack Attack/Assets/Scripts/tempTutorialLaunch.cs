using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class tempTutorialLaunch : MonoBehaviour
{
    public GameObject firstOption;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    public void GoToTutorial()
    {
        //Play Tutorial
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}