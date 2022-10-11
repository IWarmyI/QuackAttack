using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{

    public int levelNum;

    void NextLevel()
    {
        levelNum++;
        levelNum.ToString();
        SceneManager.LoadScene(levelNum);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            NextLevel();
        }
    }
}
