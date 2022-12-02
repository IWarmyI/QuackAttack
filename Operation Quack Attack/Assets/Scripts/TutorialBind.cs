using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class TutorialBind : MonoBehaviour
{
    [SerializeField] InputActionAsset input;
    InputActionMap iam;
    TMP_Text tutorial;

    private void Awake()
    {
        iam = input.FindActionMap("Player");
        tutorial = GetComponent<TMP_Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateBinds();
    }

    void UpdateBinds()
    {
        string[] subStrings = tutorial.text.Split("$");
        for (int i = 0; i < subStrings.Length; i++)
        {
            string s = subStrings[i];

            foreach (InputAction action in iam.actions)
            {
                if (s == "Move")
                {
                    string l = action.bindings[1].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
                    string r = action.bindings[2].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
                    string bind = $"{l}/{r}";
                    s = bind;
                }
                else if (s == action.name)
                {
                    string bind = action.bindings[0].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
                    s = bind;
                }
            }

            subStrings[i] = s;
        }
        string newText = string.Concat(subStrings);
        tutorial.text = newText;
    }
}
