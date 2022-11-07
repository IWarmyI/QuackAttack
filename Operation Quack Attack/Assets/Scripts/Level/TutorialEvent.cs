using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialEvent : MonoBehaviour
{
    private bool needHelp;
    private float resetTimer;
    [SerializeField] private float timer = 20.0f;
    [SerializeField] private TMP_Text tutorial;

    // Start is called before the first frame update
    void Start()
    {
        needHelp = false;
        tutorial.enabled = false;
        resetTimer = timer;
    }

    // Update is called once per frame
    void Update()
    {
        // count down timer before tutorial shows
        if (needHelp)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            if (timer <= 0)
            {
                tutorial.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            needHelp = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            needHelp = false;
            timer = resetTimer;
            //tutorial.enabled = false;
        }
    }
}
