using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UpdateTutorialText : MonoBehaviour
{
    [SerializeField] InputActionAsset input;
    static InputActionMap iam;

    // Start is called before the first frame update
    void Awake()
    {
        iam = input.FindActionMap("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function has to be static bc of Yarnspinner
    [YarnFunction("get_bind")] // This sets the Yarn function to be used in YarnSpinner scripts
    private static string GetBind(string currInput){
        // string s = "";
        //     foreach (InputAction action in iam.actions)
        //     {
        //         if (currInput == "Move")
        //         {
        //             string l = action.bindings[1].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        //             string r = action.bindings[2].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        //             string bind = $"{l}/{r}";
        //             s = bind;
        //         }
        //         else if (currInput == action.name)
        //         {
        //             string bind = action.bindings[0].ToDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        //             s = bind;
        //         }
        //     }
        // return s;
        if (currInput == "Move") {return "(A & D)";}
        else if (currInput == "Jump") {return "(Spacebar)";}
        else if (currInput == "Dash") {return "(Shift)";}
        else if (currInput == "Shoot") {return "(K)";}
        else {return "(NOT FOUND)";};
    }   

}