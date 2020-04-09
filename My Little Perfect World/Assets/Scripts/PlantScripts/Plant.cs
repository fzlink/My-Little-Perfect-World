using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Creature
{
    [SerializeField] private PlantProperties properties;
    public PlantProperties GetProperties() { return properties; }

    
    private Air air;
    private FourthDimension time;
    private Sun sun;

    public float starchAmount;
    public float growAmount => transform.localScale.magnitude;
    public bool isEdible => growAmount >= properties.EdibilityThreshold;

    private void Awake()
    {
        air = FindObjectOfType<Air>();
        time = FindObjectOfType<FourthDimension>();
        sun = FindObjectOfType<Sun>();
    }

    private void Update()
    {
        if(starchAmount > 0)
        {
            Grow();
        }

        if( air.carbondioxide > 0 && FourthDimension.timeOfDay == FourthDimension.TimeOfDay.Day)
        {
            Photosynthesis();
        }
    }

    private void Photosynthesis()
    {
        starchAmount += air.carbondioxide * sun.sunlightStrength * Time.deltaTime;
        if (starchAmount > properties.MaxStarch)
            starchAmount = properties.MaxStarch;
        air.DecreaseCarbondioxide();
        air.IncreaseOxygen();
    }

    private void Grow()
    {
        starchAmount -= Time.deltaTime * 10;
        if (starchAmount < 0)
            starchAmount = 0;

        air.DecreaseOxygen();
        air.IncreaseCarbondioxide();

        Vector3 clampedScale = transform.localScale;
        clampedScale += Vector3.one * starchAmount * properties.GrowSpeed * Time.deltaTime;
        clampedScale.x = Mathf.Clamp(clampedScale.x, 0, properties.MaxGrowth.x);
        clampedScale.y = Mathf.Clamp(clampedScale.y, 0, properties.MaxGrowth.y);
        clampedScale.z = Mathf.Clamp(clampedScale.z, 0, properties.MaxGrowth.z);
        transform.localScale = clampedScale;

    }

    public Vegetation Die()
    {
        Vegetation vegetation = gameObject.AddComponent<Vegetation>();
        vegetation.nutritionValue = properties.NutritionValue;
        vegetation.vegetationType = tag;
        enabled = false;
        return vegetation;
    }
}
