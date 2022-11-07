using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public Animator animator;
    public float transitionDelayTime = 1.0f;

    void Awake()
    {
        animator = GameObject.Find("Transition").GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayLoadLevel(("EndLevel")));
        }
    }

    IEnumerator DelayLoadLevel(string sceneName)
    {
        animator.SetTrigger("TriggerTransition");
        yield return new WaitForSeconds(transitionDelayTime);
        SceneManager.LoadScene(sceneName);
    }
}
