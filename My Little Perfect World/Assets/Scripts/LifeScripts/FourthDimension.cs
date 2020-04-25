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

    public static float fCT;
    public float fullCycleTime;
    public static float tSM;
    public float timeSpeedMultiplier;

    public event Action onPassDay;

    private void Awake()
    {
        fCT = fullCycleTime;
        tSM = timeSpeedMultiplier;
    }

    private void Start()
    {
        timeOfDay = TimeOfDay.Day;
    }

    private void Update()
    {
        fCT = fullCycleTime;
        tSM = timeSpeedMultiplier;
        currentTime += (Time.deltaTime / fCT) * tSM;
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
        if(onPassDay != null)
        {
            onPassDay();
        }
    }

    
}
