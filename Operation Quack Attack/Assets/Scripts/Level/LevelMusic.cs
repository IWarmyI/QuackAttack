using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static AudioSliders;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    private LevelPersistence persist;

    private void Awake()
    {
        persist = GetComponentInParent<LevelPersistence>();
        source.volume = musicFloat;
        persist.OnReload += RestartMusic;
    }

    void RestartMusic()
    {
        if (LevelPersistence.RestartFlag)
        {
            source.Stop();
            source.volume = musicFloat;
            source.Play();
        }

        if (persist.Level != SceneManager.GetActiveScene().buildIndex)
        {
            source.Stop();
        }
    }
}
