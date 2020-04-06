using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproManager : MonoBehaviour
{
    public int id;

    public float reproSpeed { get; set; }
    public float reproMax { get; set; }
    private float reproduceAmount;
    private bool canStartReproducing;

    public void StartReproduction()
    {
        canStartReproducing = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (canStartReproducing)
        {
            reproduceAmount += reproSpeed * Time.deltaTime;
            if (reproduceAmount >= reproMax)
            {
                AnimalInteractionManager.instance.FinishReproducing(this,true);
            }
        }
    }
}
