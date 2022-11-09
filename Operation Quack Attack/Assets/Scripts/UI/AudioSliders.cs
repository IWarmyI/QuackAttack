using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent( typeof (Slider))]
public class AudioSliders : MonoBehaviour
{
    Slider slider
    {
        get { return GetComponent<Slider>(); }
    }

    public AudioMixer mixer;
    public string volumeName;

    // Update is called once per frame
    public void UpdateValueOnChange(float value)
    {
        mixer.SetFloat(volumeName, Mathf.Log(value) * 20.0f);
    }
}