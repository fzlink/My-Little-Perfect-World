using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomposers : MonoBehaviour
{
    public int decomposerAmount;
    private float decomposePower;
    private float nutritionJob;

    private Environment environment;
    private Soil soil;

    private void Awake()
    {
        environment = FindObjectOfType<Environment>();
        soil = FindObjectOfType<Soil>();
    }

    private void Start()
    {
        if(decomposerAmount == 0)
        {
            decomposerAmount = UnityEngine.Random.Range(100, 200);
        }
        if(decomposePower == 0)
        {
            decomposePower = UnityEngine.Random.value;
        }
    }

    private void Update()
    {
        if(nutritionJob > 0)
        {
            Decompose();
        }
    }

    public void Decompose()
    {
        float oldJob = nutritionJob;
        nutritionJob -= decomposePower * decomposerAmount * Time.deltaTime;
        if (nutritionJob < 0)
            nutritionJob = 0;
        soil.GetFed(oldJob - nutritionJob);
    }

    public void GetDecomposeJob(float nutritionValue)
    {
        nutritionJob += nutritionValue;
    }

    private void FeedSoil()
    {

    }
}
