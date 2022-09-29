using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempTutorialLaunch : MonoBehaviour
{
    public void GoToTutorial()
    {
        //Play Tutorial
        UnityEngine.SceneManagement.SceneManager.LoadScene("Tutorial");
    }
}