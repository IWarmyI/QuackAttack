using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HatManager : MonoBehaviour
{
    public string[] hatNames = { "No Hat", "Fedora", "Top Hat", "Witch Hat", "Transparent Hat" };
    public Sprite[] hatSprites;

    // Distinguish between the current sprite and the player's selected sprite.
    private int currSprite;
    public static int selectedSprite;

    public Text hatDisplay;
    public Image hatImage;

    // Start is called before the first frame update
    void Start()
    {
        // Restore player's selected sprite.
        currSprite = selectedSprite;

        hatDisplay.text = hatNames[currSprite];
        hatImage.sprite = hatSprites[currSprite];

        // If "None" is selected, hide the image.
        if (currSprite == 0) hatImage.color = new Color(255, 255, 255, 0);
        else hatImage.color = new Color(255, 255, 255, 1);
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

            // If "None" is selected, hide the image.
            hatImage.color = new Color(255, 255, 255, 0);
        }
        else
        {
            currSprite++;
            hatImage.color = new Color(255, 255, 255, 1);
        }

        hatDisplay.text = hatNames[currSprite];
        hatImage.sprite = hatSprites[currSprite];
    }

    public void PreviousHatButton()
    {
        if (currSprite == 0)
        {
            currSprite = hatNames.Length - 1;
            hatImage.color = new Color(255, 255, 255, 1);
        }
        else
        {
            currSprite--;

            // If "None" is selected, hide the image.
            if (currSprite == 0) hatImage.color = new Color(255, 255, 255, 0);
        }

        hatDisplay.text = hatNames[currSprite];
        hatImage.sprite = hatSprites[currSprite];
    }

    /// <summary>
    /// Overwrite the player's selected sprite with their current choice.
    /// </summary>
    public void SaveSelection()
    {
        selectedSprite = currSprite;
    }
}
