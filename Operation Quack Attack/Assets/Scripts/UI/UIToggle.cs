using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIToggle : MonoBehaviour
{
    public bool Value = false;
    private Slider slider;

    public UnityEvent OnValueChanged;
    public UnityEvent OnValueFalse;
    public UnityEvent OnValueTrue;

    public void Awake()
    {
        if (OnValueChanged == null)
            OnValueChanged = new UnityEvent();
        if (OnValueFalse == null)
            OnValueFalse = new UnityEvent();
        if (OnValueTrue == null)
            OnValueTrue = new UnityEvent();

        slider = GetComponent<Slider>();
    }

    public void UpdateValue()
    {
        Value = slider.value > 0;
        OnValueChanged.Invoke();

        if (Value)
            OnValueTrue.Invoke();
        else
            OnValueFalse.Invoke();
    }

    public void SetValue(bool value)
    {
        Value = value;
        slider.value = Value ? 1 : 0;

        if (Value)
            OnValueTrue.Invoke();
        else
            OnValueFalse.Invoke();
    }

    public void Toggle()
    {
        SetValue(!Value);
    }
}
