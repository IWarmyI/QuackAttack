using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class Timer
{
    public delegate void TimerCallback();

    [SerializeField] private float time;
    [SerializeField] private float currentTime;
    private TimerCallback timerCallback;
    [SerializeField] private bool isPaused = false;

    public bool IsReady
    {
        get
        {
            return currentTime <= 0;
        }
    }
    public bool IsRunning
    {
        get { return !isPaused && !IsReady; }
    }

    public Timer(float time, TimerCallback timerCallback)
    {
        this.time = time;
        this.currentTime = 0;
        this.timerCallback = timerCallback;
    }
    public Timer(float time) : this(time, null)
    { }

    public void Update()
    {
        if (isPaused) return;

        if (!IsReady)
        {
            currentTime -= Time.deltaTime;

            if (IsReady)
                timerCallback?.Invoke();
        }
    }

    public void Start(bool pause = false)
    {
        currentTime = time;
        isPaused = pause;
    }

    public void Pause(bool val = true)
    {
        isPaused = val;
    }

    public void Resume()
    {
        Pause(false);
    }

    public void Stop()
    {
        currentTime = 0;
        isPaused = true;
    }
}
