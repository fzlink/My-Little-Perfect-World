using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Creature
{
    public float chlorophyllAmount;
    public float starch;
    private Air air;
    private FourthDimension time;
    private Sun sun;
    public float growFactor = 0.01f;

    private void Start()
    {
        air = FindObjectOfType<Air>();
        time = FindObjectOfType<FourthDimension>();
        sun = FindObjectOfType<Sun>();
    }

    private void Update()
    {
        if(starch < 0)
        {
            starch = 0;
        }

        if(chlorophyllAmount > 0 && air.carbondioxide > 0 && time.timeOfDay == FourthDimension.TimeOfDay.Day)
        {
            Photosynthesis();
        }
        else if(starch > 0 && time.timeOfDay == FourthDimension.TimeOfDay.Night)
        {
            Grow();
        }
    }

    private void Photosynthesis()
    {
        starch += chlorophyllAmount * air.carbondioxide * sun.sunlightStrength * Time.deltaTime;
        air.DecreaseCarbondioxide();
        air.IncreaseOxygen();
    }

    private void Grow()
    {
        Vector3 clampedScale = transform.localScale;
        clampedScale += Vector3.one * starch * growFactor * Time.deltaTime;
        clampedScale.x = Mathf.Clamp(clampedScale.x, 0, 1);
        clampedScale.y = Mathf.Clamp(clampedScale.y, 0, 1);
        clampedScale.z = Mathf.Clamp(clampedScale.z, 0, 1);
        transform.localScale = clampedScale;

        starch -= Time.deltaTime * growFactor;
        air.DecreaseOxygen();
        air.IncreaseCarbondioxide();
    }

}
