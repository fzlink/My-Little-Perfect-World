using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManger : MonoBehaviour
{
    public static SimulationManger instance;

    public FourthDimension timeManager;
    public Air air;
    public Sun sun;
    public Soil soil;
    public Climate climate;
    public ObjectPlacer objectPlacer;
    public AnimalInteractionManager animalInteractionManager;
    public PopulationOptimizer populationOptimizer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        
    }
}
