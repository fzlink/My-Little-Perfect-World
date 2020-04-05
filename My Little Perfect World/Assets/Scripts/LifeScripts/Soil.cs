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

    private Decomposers decomposers;

    private void Awake()
    {
        decomposers = FindObjectOfType<Decomposers>();
    }

    private void Start()
    {
        GetFed(totalNutrition);
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

}