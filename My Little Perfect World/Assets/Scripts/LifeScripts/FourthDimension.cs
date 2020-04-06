using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourthDimension : MonoBehaviour
{
    public enum TimeOfDay { Day, Night};
    public static TimeOfDay timeOfDay;
    public static float currentTime = 0;
    public static int currentDay = 0;
    public float fullCycleTime = 240;
    public float timeSpeedMultiplier = 100;

    private void Start()
    {
        timeOfDay = TimeOfDay.Day;
    }

    private void Update()
    {
        currentTime += (Time.deltaTime / fullCycleTime) * timeSpeedMultiplier;
        //print(currentTime);
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
