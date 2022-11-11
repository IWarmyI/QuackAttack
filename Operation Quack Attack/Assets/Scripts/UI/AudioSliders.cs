using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSliders : MonoBehaviour
{
    public Slider music, sfx;
    public static float musicFloat;
    public static float sfxFloat;

    void OnEnable()
    {
        music.value = musicFloat;
        sfx.value = sfxFloat;
    }

    public void UpdateMusic()
    {
        musicFloat = music.value;
    }

    public void UpdateSFX()
    {
        sfxFloat = sfx.value;
    }
}