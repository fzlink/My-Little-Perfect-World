using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproManager : MonoBehaviour
{
    public int id;

    public Animal partner1 { get; set; }
    public Animal partner2 { get; set; }

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
            print(reproduceAmount);
            if (reproduceAmount >= reproMax)
            {
                AnimalInteractionManager.instance.FinishReproducing(id,true);
            }
        }
    }
}
