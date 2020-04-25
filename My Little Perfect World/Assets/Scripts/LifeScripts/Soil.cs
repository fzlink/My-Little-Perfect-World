using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{
    public float totalNutrition;
    public float nitrogenVariants;
    public float phosphorus;
    public float potassium;

    public float water;
    private Climate climate;

    private Decomposers decomposers;
    public float emissionRate;
    public float vaporizeRate;
    private bool isRaining;

    public float randomPlantFactor;
    public float plantCreateThreshold;
    private float plantCreateDuration;

    public Transform plantsContainer;
    private List<Transform> seeds;

    private void Awake()
    {
        climate = FindObjectOfType<Climate>();
        decomposers = FindObjectOfType<Decomposers>();
    }

    private void Start()
    {
        FindObjectOfType<ObjectPlacer>().onObjectsPlaced += IdentifySeeds;

        water = 5;
        plantCreateDuration = plantCreateThreshold;
        climate.onRainStart += OnRainStarted;
        climate.onRainStop += OnRainStopped;
        GetFed(totalNutrition);
    }

    private void IdentifySeeds()
    {
        seeds = new List<Transform>();
        foreach (Transform transform in plantsContainer)
        {
            if (transform.GetComponent<Plant>().isDormant)
                seeds.Add(transform);
        }
        FindObjectOfType<ObjectPlacer>().onObjectsPlaced -= IdentifySeeds;
    }

    private void WakeSeed()
    {
        int index = UnityEngine.Random.Range(0, seeds.Count - 1);
        seeds[index].GetComponent<Plant>().isDormant = false;
        seeds.RemoveAt(index);
    }

    private void Update()
    {

        if(UnityEngine.Random.value < randomPlantFactor + water/50f) 
        {
            plantCreateDuration -= Time.deltaTime * FourthDimension.tSM;
            if (plantCreateDuration <= 0)
            {
                WakeSeed();
                plantCreateDuration = plantCreateThreshold;
            }
        }
        water -= Time.deltaTime * vaporizeRate * FourthDimension.tSM;
        if(isRaining)
            water += Time.deltaTime * emissionRate * FourthDimension.tSM;
    }



    private void OnRainStarted()
    {
        isRaining = true;
    }

    private void OnRainStopped()
    {
        isRaining = false;
    }



    public void IncreaseDead(float magnitude)
    {
        decomposers.GetDecomposeJob(magnitude);
    }

    public void GetFed(float nutrition)
    {
        int r1 = UnityEngine.Random.Range(1, 10); //3
        int r2 = UnityEngine.Random.Range(1, 10); //7
        int r3 = UnityEngine.Random.Range(1, 10); //6
        float sum = r1 + r2 + r3;

        nitrogenVariants += nutrition * (r1 / sum);
        phosphorus += nutrition * (r2 / sum);
        potassium += nutrition * (r3 / sum);

    }

    private void OnDestroy()
    {
        climate.onRainStart -= OnRainStarted;
        climate.onRainStop -= OnRainStopped;
    }

}