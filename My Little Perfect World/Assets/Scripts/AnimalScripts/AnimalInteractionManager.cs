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
    public event Action<int,bool> onReproducingFinished;

    public void StartReproducing(Animal animal, Animal partner, float reproSpeed, float reproMax)
    {
        if(partner.reproId == -1)
        {
            animal.reproId = reproManagerCount;
            GameObject newReproManager = new GameObject("Repro Manager" + reproManagerCount);
            ReproManager reproScript = newReproManager.AddComponent<ReproManager>();
            newReproManager.transform.parent = reproducingManagersParent.transform;
            reproScript.id = reproManagerCount++;
            reproScript.partner1 = animal;
            reproScript.reproSpeed = reproSpeed;
            reproScript.reproMax = reproMax;

            reproManagers.Add(reproScript);
        }
        else
        {
            animal.reproId = partner.reproId;
            reproManagers[partner.reproId].partner2 = animal;
            reproManagers[partner.reproId].StartReproduction();
        }
    }
    public void FinishReproducing(int id, bool isSuccess)
    {
        if(id != -1)
        {
            if(onReproducingFinished != null)
            {
                Destroy(reproManagers[id]);
                reproManagers.RemoveAt(id);
                onReproducingFinished(id,isSuccess);
            }
        }
    }

    public void Interrupted(Animal animal)
    {
        FinishReproducing(animal.reproId, false);

    }
}
