using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMusic : LevelPersistent
{
    [SerializeField] private AudioSource source;

    protected override void Awake()
    {
        base.Awake();
        OnReload += RestartMusic;
    }

    void RestartMusic()
    {
        if (RestartFlag)
        {
            source.Stop();
            source.Play();
        }
        RestartFlag = false;

        if (Level != SceneManager.GetActiveScene().buildIndex)
        {
            source.Stop();
        }
    }

    public static void Initialize()
    {
        Initialize(typeof(LevelMusic));
    }
}
