using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TintManager : MonoBehaviour
{
    public static Color selectedColor;
    public Image duck;

    /// <summary>
    /// Makes a Color32 from a hex string.
    /// Prefer Color32 over Color for accurate representation of hues in the UI.
    /// </summary>
    /// <param name="hex">
    /// Hex string containing an RGB value.
    /// </param>
    /// <returns>
    /// Color32 equivalent of the hex string.
    /// </returns>
    public static Color32 MakeColor(string hex)
    {
        float r = Convert.ToInt32(hex.Substring(0, 2), 16) / 255f;
        float g = Convert.ToInt32(hex.Substring(2, 2), 16) / 255f;
        float b = Convert.ToInt32(hex.Substring(4, 2), 16) / 255f;

        return new Color(r, g, b, 1);
    }

    /// <summary>
    /// Creates the default color, yellow.
    /// </summary>
    /// <returns>
    /// A Color32 representation of yellow.
    /// </returns>
    public static Color32 MakeColor()
    {
        return MakeColor("FFBD00");
    }

    /// <summary>
    /// Updates the selected color.
    /// </summary>
    /// <param name="hex">
    /// Hex string containing an RGB value.
    /// </param>
    public void ChangeColor(string hex)
    {
        selectedColor = duck.color = MakeColor(hex);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Default to yellow if a color hasn't been picked yet.
        if (selectedColor == Color.clear) duck.color = MakeColor();

        // Else display the player's selected color.
        else duck.color = selectedColor;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
