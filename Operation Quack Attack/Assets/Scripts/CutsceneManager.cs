using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using UnityEngine.SceneManagement;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject textToChangeToLoad;
    [SerializeField] GameObject runDuck;
    GameObject lvlManager;

    void Start()
    {
        lvlManager = GameObject.Find("LevelManager");
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SkipCutscene()
    {
        StartCoroutine(cutscene());
    }

    IEnumerator cutscene()
    {
        loadPanel.SetActive(true);
        runDuck.SetActive(true);
        textToChangeToLoad.GetComponent<TextMeshProUGUI>().text = "LOADING...";//Rotate 90 deg


        //Wait for 4 seconds
        yield return new WaitForSecondsRealtime(1);

        lvlManager.GetComponent<LevelManager>().LoadNewLevel(2);

    }
}