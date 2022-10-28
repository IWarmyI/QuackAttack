using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class RebindControls : MonoBehaviour
{
    [SerializeField] private InputActionReference left = null;
    [SerializeField] private InputActionReference right = null;
    [SerializeField] private InputActionReference jump = null;
    [SerializeField] private InputActionReference dash = null;
    [SerializeField] private InputActionReference shoot = null;
    [SerializeField] private InputActionReference quack = null;
    [SerializeField] private InputActionReference restart = null;
}
