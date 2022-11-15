using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public GameObject firstOption;

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstOption);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
        levelManager.LoadScene("TutorialChallenge");
    }

    public void BacktoMainMenu()
    {
        levelManager.RestartLevel();
    }
}
