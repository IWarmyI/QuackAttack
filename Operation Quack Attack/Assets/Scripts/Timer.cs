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
    private event TimerCallback OnTimerComplete;
    [SerializeField] private bool isPaused = false;

    public bool IsReady { get { return currentTime == time; } }
    public bool IsComplete { get { return currentTime <= 0; } }
    public bool IsRunning { get { return !isPaused && !IsComplete; } }
    public bool IsPaused { get { return isPaused; } }
    public float MaxTime { get { return time; } }
    public float CurrentTime { get { return currentTime; } }

    public Timer(float time, TimerCallback timerCallback)
    {
        this.time = time;
        this.currentTime = 0;
        this.OnTimerComplete = timerCallback;
    }
    public Timer(float time) : this(time, null)
    { }

    public void Update()
    {
        if (isPaused) return;

        if (!IsComplete)
        {
            currentTime -= Time.deltaTime;

            if (IsComplete)
            {
                OnTimerComplete?.Invoke();
                currentTime = 0;
            }
        }
    }

    public void Ready()
    {
        Start(true);
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
