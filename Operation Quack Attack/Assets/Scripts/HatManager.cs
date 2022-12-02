using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatManager : MonoBehaviour
{
    public string[] hatNames = { "No Hat", "Fedora", "Top Hat", "Witch Hat", "Transparent Hat" };
    public static int currSprite;
    public Text hatDisplay;

    // Start is called before the first frame update
    void Start()
    {
        currSprite = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextHatButton()
    {
        if (currSprite == hatNames.Length - 1)
        {
            currSprite = 0;
        }
        else
        {
            currSprite++;
        }

        hatDisplay.text = hatNames[currSprite];
    }

    public void PreviousHatButton()
    {
        if (currSprite == 0)
        {
            currSprite = hatNames.Length - 1;
        }
        else
        {
            currSprite--;
        }

        hatDisplay.text = hatNames[currSprite];
    }
}
