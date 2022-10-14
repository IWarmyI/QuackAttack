using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerEvent : MonoBehaviour
{
    [SerializeField] private HUDTimer timer;
    [SerializeField] private bool stop = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (timer == null) return;
        timer.Stop = stop;
    }
}
