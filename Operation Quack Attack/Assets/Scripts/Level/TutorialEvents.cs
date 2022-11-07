using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialEvents : MonoBehaviour
{
    [SerializeField] float timer;
    private bool needHelp;
    public TextMeshProUGUI tutorial;

    // Start is called before the first frame update
    void Start()
    {
        tutorial.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // count down timer before tutorial shows
        if (needHelp)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                tutorial.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D()
    {
        // check for collision
        needHelp = true;
    }

    void OnTriggerExit2D()
    {
        needHelp = false;
        timer = 20.0f;
    }
}
