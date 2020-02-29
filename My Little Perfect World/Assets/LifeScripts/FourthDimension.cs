using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthDimension : MonoBehaviour
{
    public enum TimeOfDay { Day, Night};
    public TimeOfDay timeOfDay;
    private float currentTime = 0;
    private float currentDay = 0;
    public float fullCycleTime = 200;
    public float timeSpeedMultiplier = 1;

    private void Start()
    {
        timeOfDay = TimeOfDay.Day;
    }

    private void Update()
    {
        currentTime += (Time.deltaTime / fullCycleTime) * timeSpeedMultiplier;
        print(timeOfDay);
        if(currentTime >= 0.5f)
        {
            timeOfDay = TimeOfDay.Night;
        }
        if(currentTime >= 1)
        {
            PassDay();
        }
    }

    private void PassDay()
    {
        currentDay++;
        currentTime = 0;
        timeOfDay = TimeOfDay.Day;
    }
}
