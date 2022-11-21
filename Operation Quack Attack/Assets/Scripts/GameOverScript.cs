using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject firstOption;

    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        LevelManager.Instance.LoadScene("TutorialChallenge");
    }

    public void BacktoMainMenu()
    {
        LevelManager.Instance.RestartLevel();
    }
}
