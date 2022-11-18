using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCheckpoints : MonoBehaviour
{
    private UIToggle toggler;

    public void Start()
    {
        toggler = GetComponent<UIToggle>();
        toggler.SetValue(LevelManager.GamemodeCheckpoints);
    }

    public void Toggle()
    {
        LevelManager.GamemodeCheckpoints = !LevelManager.GamemodeCheckpoints;
    }

    public void Set(bool value)
    {
        LevelManager.GamemodeCheckpoints = value;
    }
}
