﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInteractionManager : MonoBehaviour
{

    public static AnimalInteractionManager instance;

    public ParticleSystem turnToFoodFX;
    public ParticleSystem loveFX;
    public ParticleSystem zzzFX;

    public GameObject meatPrefab;

    private GameObject reproducingManagersParent;
    private GameObject eatingManagersParent;

    public Transform foodContainer;
    public Transform interactionContainer;

    private int reproManagerCount;
    private int eatingManagerCount;
    private List<ReproManager> reproManagers;
    private List<EatingManager> eatingManagers;
    public event Action<ReproManager, bool> onReproducingFinished;
    public event Action<EatingManager, bool> onEatingFinished;

    public event Action<Transform> onNewInteraction;
    public event Action<Transform> onFinishInteraction;
    public event Action onAnimalDied;

    private void Awake()
    {
        instance = this;
        reproducingManagersParent = new GameObject("Repros");
        eatingManagersParent = new GameObject("Eatings");
        reproducingManagersParent.transform.parent = interactionContainer;
        eatingManagersParent.transform.parent = interactionContainer;
    }

    private void Start()
    {
        reproManagers = new List<ReproManager>();
        eatingManagers = new List<EatingManager>();
    }

    public void PrintDeadCount()
    {
        if(onAnimalDied != null)
        {
            onAnimalDied();
        }
        print("Dead Animal Count: " + foodContainer.childCount);
    }

    public void Interrupted(Animal animal)
    {
        FinishReproducing(animal.reproManager, false);
        FinishEating(animal.eatingManager, false);
    }

    /////////Reproduction
    public void StartReproducing(Animal animal, Animal partner)
    {
        if(partner.reproManager == null)
        {
            GameObject reproGameObject = new GameObject("Repro Manager" + reproManagerCount++);
            reproGameObject.transform.position = animal.transform.position;
            reproGameObject.transform.parent = reproducingManagersParent.transform;

            ReproManager repro = reproGameObject.AddComponent<ReproManager>();
            animal.reproManager = repro;
            repro.loveFX = loveFX;
            repro.SetAnimal1(animal);
            reproManagers.Add(repro);
        }
        else
        {
            animal.reproManager = partner.reproManager;
            animal.reproManager.SetAnimal2(animal);
            animal.reproManager.StartReproduction();
            if(onNewInteraction != null)
            {
                onNewInteraction(animal.reproManager.transform);
            }
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
                if(onFinishInteraction != null)
                {
                    onFinishInteraction(reproManager.transform);
                }
            }
        }
    }

    ///////// Eating
    public bool StartEating(Animal animal, Food food)
    {
        if(animal == null || food == null) { return false; }
        GameObject eatingGameObject = new GameObject("Eating Manager" + eatingManagerCount++);
        eatingGameObject.transform.position = food.transform.position;
        eatingGameObject.transform.parent = eatingManagersParent.transform;
        EatingManager eating = eatingGameObject.AddComponent<EatingManager>();
        eatingManagers.Add(eating);
        animal.eatingManager = eating;
        eating.StartEating(food,animal);
        if(onNewInteraction != null)
        {
            onNewInteraction(eating.transform);
        }
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
                if (onFinishInteraction != null)
                {
                    onFinishInteraction(eatingManager.transform);
                }
            }
        }
    }

    public void MakePoof(Animal animal)
    {
        Instantiate(meatPrefab, animal.transform.position, meatPrefab.transform.rotation, animal.transform);
        animal.GetComponentInChildren<MeshRenderer>().enabled = false;
     
        ParticleSystem particle = Instantiate(turnToFoodFX, animal.transform.position, Quaternion.identity);
        Destroy(particle.gameObject, particle.duration + particle.startLifetime/2);
    }

}
