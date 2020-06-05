using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReproManager : MonoBehaviour
{
    private Animal animal1;
    private Animal animal2;

    public ParticleSystem loveFX;
    private float reproduceAmount;
    private bool canStartReproducing;

    public void SetAnimal1(Animal animal1)
    {
        this.animal1 = animal1;
    }

    public void SetAnimal2(Animal animal2)
    {
        this.animal2 = animal1;
    }

    public void StartReproduction()
    {
        canStartReproducing = true;
        Instantiate(loveFX, transform.position, loveFX.transform.rotation, transform);
    }


    // Update is called once per frame
    void Update()
    {
        if (canStartReproducing)
        {
            if (animal1.state != AnimalState.Reproducing || animal2.state != AnimalState.Reproducing)
            {
                canStartReproducing = false;
                loveFX.Stop();
                AnimalInteractionManager.instance.FinishReproducing(this, false);
            }
            else
            {
                reproduceAmount += animal1.GetProperties().ReproducingSpeed * Time.deltaTime;
                if (reproduceAmount >= animal1.GetProperties().ReproducingMaximum)
                {
                    canStartReproducing = false;
                    loveFX.Stop();
                    AnimalInteractionManager.instance.FinishReproducing(this, true);
                }
            }
        }
    }
}
