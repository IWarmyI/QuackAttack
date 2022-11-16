using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private LevelManager levelManager;

    void Start()
    {
        levelManager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            levelManager.LoadScene("EndLevel");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            levelManager.LoadScene("EndLevel");
        }
    }
}
