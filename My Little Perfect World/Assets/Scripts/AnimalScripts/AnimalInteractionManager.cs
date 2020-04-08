using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInteractionManager : MonoBehaviour
{

    public static AnimalInteractionManager instance;

    private GameObject reproducingManagersParent;
    private GameObject eatingManagersParent;

    public Transform MeatContainer;
    public int reproManagerCount;
    public int eatingManagerCount;
    public List<ReproManager> reproManagers;
    public List<EatingManager> eatingManagers;
    public event Action<ReproManager, bool> onReproducingFinished;
    public event Action<EatingManager, bool> onEatingFinished;

    private void Awake()
    {
        instance = this;
        reproducingManagersParent = new GameObject("Repros");
        eatingManagersParent = new GameObject("Eatings");
    }

    private void Start()
    {
        reproManagers = new List<ReproManager>();
        eatingManagers = new List<EatingManager>();
    }

    public void PrintDeadCount()
    {
        print("Dead Animal Count: " + MeatContainer.childCount);
    }

    public void Interrupted(Animal animal)
    {
        FinishReproducing(animal.reproManager, false);
        FinishEating(animal.eatingManager, false);
    }

    /////////Reproduction
    public void StartReproducing(Animal animal, Animal partner, float reproSpeed, float reproMax)
    {
        if(partner.reproManager == null)
        {
            GameObject reproGameObject = new GameObject("Repro Manager" + reproManagerCount++);
            reproGameObject.transform.position = animal.transform.position;
            reproGameObject.transform.parent = reproducingManagersParent.transform;

            ReproManager repro = reproGameObject.AddComponent<ReproManager>();
            animal.reproManager = repro;
            repro.reproSpeed = reproSpeed;
            repro.reproMax = reproMax;

            reproManagers.Add(repro);
        }
        else
        {
            animal.reproManager = partner.reproManager;
            animal.reproManager.StartReproduction();
        }
    }

    public void FinishReproducing(ReproManager reproManager, bool isSuccess)
    {
        if(reproManager != null)
        {
            if(onReproducingFinished != null)
            {
                reproManagers.Remove(reproManager);
                onReproducingFinished(reproManager,isSuccess);
                Destroy(reproManager.gameObject);
            }
        }
    }

    ///////// Eating
    public bool StartEating(Animal animal, Food food)
    {
        if(animal == null || food == null) { return false; }
        GameObject eatingGameObject = new GameObject("Eating Manager" + eatingManagerCount++);
        eatingGameObject.transform.position = animal.transform.position;
        eatingGameObject.transform.parent = eatingManagersParent.transform;

        EatingManager eating = eatingGameObject.AddComponent<EatingManager>();
        eatingManagers.Add(eating);
        animal.eatingManager = eating;
        eating.StartEating(food,animal.GetProperties().FoodEatingSpeed);
        return true;
    }

    public void FinishEating(EatingManager eatingManager, bool isSuccess)
    {
        if(eatingManager != null)
        {
            if(onEatingFinished != null)
            {
                eatingManagers.Remove(eatingManager);
                onEatingFinished(eatingManager, isSuccess);
                Destroy(eatingManager.gameObject);
            }
        }
    }

}
