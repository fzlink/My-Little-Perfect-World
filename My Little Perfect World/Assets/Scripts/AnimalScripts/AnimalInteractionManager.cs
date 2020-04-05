using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInteractionManager : MonoBehaviour
{

    public static AnimalInteractionManager instance;

    private GameObject reproducingManagersParent;
    private GameObject eatingManagersParent;
    private void Awake()
    {
        instance = this;
        reproducingManagersParent = new GameObject("Repros");
        eatingManagersParent = new GameObject("Eatings");
    }


    public int reproManagerCount;
    public List<ReproManager> reproManagers;
    public event Action<ReproManager,bool> onReproducingFinished;

    public void StartReproducing(Animal animal, Animal partner, float reproSpeed, float reproMax)
    {
        if(partner.reproManager == null)
        {
            GameObject reproGameObject = new GameObject("Repro Manager" + reproManagerCount);
            reproGameObject.transform.position = animal.transform.position;
            reproGameObject.transform.parent = reproducingManagersParent.transform;

            ReproManager repro = reproGameObject.AddComponent<ReproManager>();
            animal.reproManager = repro;
            repro.id = reproManagerCount++;
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
                Destroy(reproManager.gameObject);
                reproManagers.Remove(reproManager);
                onReproducingFinished(reproManager,isSuccess);
            }
        }
    }

    public void Interrupted(Animal animal)
    {
        FinishReproducing(animal.reproManager, false);
    }
}
