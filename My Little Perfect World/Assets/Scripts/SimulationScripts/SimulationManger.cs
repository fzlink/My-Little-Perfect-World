using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManger : MonoBehaviour
{
    public static SimulationManger instance;

    //public List<GameObject> creaturePrefabs;
    //public List<Properties> creatureProperties;



    [SerializeField] private CreatureController creatureController;
    [SerializeField] private FourthDimension timeManager;
    [SerializeField] private Air air;
    [SerializeField] private static Sun sun;
    [SerializeField] private Soil soil;
    [SerializeField] private Decomposers decomposers;

    


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

    public void SoilIncreaseDead(float magnitude)
    {
        soil.IncreaseDead(magnitude);
    }
}
