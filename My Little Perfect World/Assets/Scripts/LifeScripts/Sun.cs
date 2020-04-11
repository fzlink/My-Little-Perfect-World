using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Light sun;

    public float sunlightStrength { get; set; }
    public float maxSunLightStrength;
    private float sunInitIntensity;

    private void Start()
    {
        sunInitIntensity = sun.intensity;
    }

    private void Update()
    {
        UpdateSun();
    }

    private void UpdateSun()
    {
        float currentTime = FourthDimension.currentTime;
        sun.transform.localRotation = Quaternion.Euler((currentTime * 360f), 0, 0);
        if(currentTime > 0.5f)
        {
            sunlightStrength = 0;
        }
        else if(currentTime < 0.25f)
        {
            sunlightStrength = currentTime * (maxSunLightStrength/0.25f) ;
        }
        else if(currentTime > 0.25f)
        {
            sunlightStrength = (2 * maxSunLightStrength) - currentTime * (maxSunLightStrength / 0.25f);
        }
        //float intensityMultiplier = 1;
        //if (currentTime <= 0.23f || currentTime >= 0.75f)
        //{
        //    intensityMultiplier = 0;
        //}
        //else if (currentTime <= 0.25f)
        //{
        //    intensityMultiplier = Mathf.Clamp01((currentTime - 0.23f) * (1 / 0.02f));
        //}
        //else if (currentTime >= 0.73f)
        //{
        //    intensityMultiplier = Mathf.Clamp01(1 - ((currentTime - 0.73f) * (1 / 0.02f)));
        //}

        //sun.intensity = sunInitIntensity * intensityMultiplier;
    }

}
