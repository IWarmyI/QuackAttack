using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBinds : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputs;

    public void ResetBindings () {
        foreach (InputActionMap map in inputs.actionMaps) {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }
}
