using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    private LevelPersistence persist;

    private void Awake()
    {
        persist = GetComponentInParent<LevelPersistence>();
        persist.OnReload += RestartMusic;
    }

    void RestartMusic()
    {
        if (LevelPersistence.RestartFlag)
        {
            source.Stop();
            source.Play();
        }

        if (persist.Level != SceneManager.GetActiveScene().buildIndex)
        {
            source.Stop();
        }
    }
}
