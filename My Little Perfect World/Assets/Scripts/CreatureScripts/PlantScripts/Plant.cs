using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : Creature
{
    [SerializeField] private PlantProperties properties;
    public PlantProperties GetProperties() { return properties; }
    public bool isDormant { get; set; }
    
    private Air air;
    private Sun sun;
    private Soil soil;

    public float starchAmount;
    public float growAmount => transform.localScale.magnitude;
    public bool isEdible => growAmount >= properties.EdibilityThreshold && !isDormant;

    private void Awake()
    {
        soil = SimulationManger.instance.soil;
        air = SimulationManger.instance.air;
        sun = SimulationManger.instance.sun;
    }

    private void Update()
    {
        if (!isDormant)
        {
            if(starchAmount > 0)
            {
                Grow();
            }

            if( FourthDimension.timeOfDay == FourthDimension.TimeOfDay.Day)
            {
                Photosynthesis();
            }
        }
    }

    private void Photosynthesis()
    {
        starchAmount +=  sun.sunlightStrength * soil.water * Time.deltaTime * properties.PhotosynthesisSpeed;
        if (starchAmount > properties.MaxStarch)
            starchAmount = properties.MaxStarch;
        //air.DecreaseCarbondioxide();
        //air.IncreaseOxygen();
    }

    private void Grow()
    {
        starchAmount -= Time.deltaTime * properties.GrowSpeed;
        if (starchAmount < 0)
            starchAmount = 0;

        //air.DecreaseOxygen();
        //air.IncreaseCarbondioxide();

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
