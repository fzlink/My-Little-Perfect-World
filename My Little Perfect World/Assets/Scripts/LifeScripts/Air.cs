using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : MonoBehaviour
{
    public float carbondioxide; // %0.03
    public float oxygen; // %21
    public float nitrogen; // %78
    private float oxygenChangeFactor = 0.1f;
    private float carbondioxideChangeFactor = 0.005f;

    private void Start()
    {
        InitializeGases();
    }

    private void InitializeGases()
    {
        float allGass = 100;
        nitrogen = UnityEngine.Random.Range(77.5f, 78.5f);
        allGass -= nitrogen;
        oxygen = UnityEngine.Random.Range(20.5f, 21.5f);
        allGass -= oxygen;
        carbondioxide = allGass;
    }

    private void Update()
    {
        BalanceGasses();
    }

    private void BalanceGasses()
    {
        oxygen = Mathf.Clamp(oxygen, 20.5f, 21.5f);
        carbondioxide = Mathf.Clamp(carbondioxide, 0.025f, 0.035f);
    }

    public void IncreaseOxygen()
    {
        oxygen += oxygenChangeFactor * Time.deltaTime;
    }
    public void DecreaseOxygen()
    {
        oxygen -= oxygenChangeFactor * Time.deltaTime;
    }
    public void IncreaseCarbondioxide()
    {
        carbondioxide += carbondioxideChangeFactor * Time.deltaTime;
    }
    public void DecreaseCarbondioxide()
    {
        carbondioxide -= carbondioxideChangeFactor * Time.deltaTime;
    }


}
