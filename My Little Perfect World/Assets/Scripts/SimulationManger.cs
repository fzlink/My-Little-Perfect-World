using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManger : MonoBehaviour
{
    public static SimulationManger instance;

    [SerializeField] private CreatureController creatureController;
    [SerializeField] private FourthDimension timeManager;
    [SerializeField] private Air air;
    [SerializeField] private static Sun sun;
    [SerializeField] private Soil soil;
    [SerializeField] private Decomposers decomposers;

    private void Awake()
    {
        instance = this;
    }

    public void SoilIncreaseDead(float magnitude)
    {
        soil.IncreaseDead(magnitude);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
