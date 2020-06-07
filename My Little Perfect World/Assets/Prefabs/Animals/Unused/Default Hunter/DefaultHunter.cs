using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultHunter : Animal
{

    protected override void InitValues()
    {
        foodAmount = UnityEngine.Random.Range(0f, 35f);
        waterAmount = UnityEngine.Random.Range(25f, 100f);
        sleepAmount = UnityEngine.Random.Range(25f, 100f);
    }

}
