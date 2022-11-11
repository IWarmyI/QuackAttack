using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSliders : MonoBehaviour
{
    public Slider music, sfx;
    public static float musicFloat, sfxFloat = 1f;

    void Start ()
    {
        musicFloat = 1f;
        sfxFloat = 1f;
        music.value = musicFloat;
        sfx.value = sfxFloat;
    }

    public void UpdateSound()
    {
        musicFloat = music.value;

        sfxFloat = sfx.value;
    }
}