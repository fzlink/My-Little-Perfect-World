using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FemaleAttributes : MonoBehaviour
{
    public bool isPregnant { get; set; }
    public float pregnancyAmount { get; set; }

    private AnimalProperties properties;
    private Animal animal;

    private void Awake()
    {
        animal = GetComponent<Animal>();
        properties = animal.GetProperties();
    }

    void Update()
    {
        if (isPregnant)
        {
            pregnancyAmount += properties.PregnancySpeed * Time.deltaTime;
            if (animal.state == AnimalState.Wandering && pregnancyAmount >= properties.PregnancyMaximum)
            {
                isPregnant = false;
                pregnancyAmount = 0;
                GiveBirth(animal.currentPartner);
            }
        }
    }


    private void GiveBirth(Animal father)
    {
        int n = properties.PregnancyChildAmount;
        for (int i = 0; i < n; i++)
        {
            AnimalFactory.GiveBirthChild(animal, father);
        }
    }

}
