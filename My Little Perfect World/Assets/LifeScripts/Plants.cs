using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plants : MonoBehaviour
{
    public float chlorophyllAmount;
    public float starch;
    private Environment environment;
    private FourthDimension time;
    private Sun sun;
    public float growFactor = 0.01f;

    private void Start()
    {
        environment = FindObjectOfType<Environment>();
        time = FindObjectOfType<FourthDimension>();
        sun = FindObjectOfType<Sun>();
    }

    private void Update()
    {
        if(starch < 0)
        {
            starch = 0;
        }

        if(chlorophyllAmount > 0 && environment.carbondioxide > 0 && time.timeOfDay == FourthDimension.TimeOfDay.Day)
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
        starch += chlorophyllAmount * environment.carbondioxide * sun.sunlightStrength * Time.deltaTime;
        environment.DecreaseCarbondioxide();
        environment.IncreaseOxygen();
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
        environment.DecreaseOxygen();
        environment.IncreaseCarbondioxide();
    }

}
