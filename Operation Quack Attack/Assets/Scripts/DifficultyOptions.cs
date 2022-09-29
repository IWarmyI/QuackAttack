using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyOptions : MonoBehaviour
{
    public Text description;
    public Text details;

    // Update diff description based on hover
    public void eggDiff()
    {
        description.text = "Head Empty No Thoughts";
        details.text = "Water is as meaningless as time and is not\nrequired to dash, shoot, or double jump\n(Cringe)";
    }

    public void chickDiff()
    {
        description.text = "Am Baby";
        details.text = "Water is required to shoot";
    }

    public void duckDiff()
    {
        description.text = "Thirsty Duck";
        details.text = "Water is required to shoot and double jump";
    }

    public void gooseDiff()
    {
        description.text = "I NEED IT";
        details.text = "You are a slave to water and need it\nto dash, shoot, and double jump";
    }

    public void noDiff()
    {
        description.text = "";
        details.text = "";
    }
}
