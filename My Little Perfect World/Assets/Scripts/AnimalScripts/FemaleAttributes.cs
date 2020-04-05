using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleAttributes : MonoBehaviour
{
    public bool isPregnant { get; set; }
    public float pregnancyAmount { get; set; }

    public FemaleProperties femaleProperties;
    private Animal animal;

    private void Awake()
    {
        animal = GetComponent<Animal>();
        femaleProperties = animal.GetFemaleProperties();
    }

    void Update()
    {
        if (isPregnant)
        {
            pregnancyAmount += femaleProperties.PregnancySpeed * Time.deltaTime;
            if (animal.state == AnimalState.Wandering && pregnancyAmount >= femaleProperties.PregnancyMaximum)
            {
                isPregnant = false;
                pregnancyAmount = 0;
                GiveBirth(animal.currentPartner);
            }
        }
    }


    private void GiveBirth(Animal father)
    {
        for (int i = 0; i < femaleProperties.PregnancyChildAmount; i++)
        {
            AnimalFactory.CreateChild(animal, father);

        }
    }

}
