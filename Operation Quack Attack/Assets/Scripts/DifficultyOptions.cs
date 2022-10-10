using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultyOptions : MonoBehaviour
{
    public TextMeshProUGUI description;
    public TextMeshProUGUI details;
    public Image textbox;

    void Start()
    {
        textbox.enabled = false;
        description.text = "";
        details.text = "";
    }

    // Update diff description based on hover
    public void eggDiff()
    {
        textbox.enabled = true;
        description.text = "Head Empty No Thoughts";
        details.text = "Water is as meaningless as time and is not\nrequired to dash, shoot, or double jump\n(Cringe)";
    }

    public void chickDiff()
    {
        textbox.enabled = true;
        description.text = "Am Baby";
        details.text = "Water is required to shoot";
    }

    public void duckDiff()
    {
        textbox.enabled = true;
        description.text = "Thirsty Duck";
        details.text = "Water is required to shoot and double jump";
    }

    public void gooseDiff()
    {
        textbox.enabled = true;
        description.text = "I NEED IT";
        details.text = "You are a slave to water and need it\nto dash, shoot, and double jump";
    }

    public void noDiff()
    {
        textbox.enabled = false;
        description.text = "";
        details.text = "";
    }
}
