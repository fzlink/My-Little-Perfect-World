using System;
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
    private GameObject runManagersParent;

    public Transform foodContainer;
    public Transform interactionContainer;

    private int reproManagerCount;
    private int eatingManagerCount;
    private int runManagerCount;
    private List<ReproManager> reproManagers;
    private List<EatingManager> eatingManagers;
    private List<RunManager> runManagers;
    public event Action<ReproManager, bool> onReproducingFinished;
    public event Action<EatingManager, bool> onEatingFinished;

    public event Action<Transform> onNewInteraction;
    public event Action<Transform> onFinishInteraction;
    public event Action<GameObject> onAnimalDied;

    private void Awake()
    {
        instance = this;
        reproducingManagersParent = new GameObject("Repros");
        eatingManagersParent = new GameObject("Eatings");
        runManagersParent = new GameObject("Running");
        reproducingManagersParent.transform.parent = interactionContainer;
        eatingManagersParent.transform.parent = interactionContainer;
        runManagersParent.transform.parent = interactionContainer;
    }

    private void Start()
    {
        reproManagers = new List<ReproManager>();
        eatingManagers = new List<EatingManager>();
        runManagers = new List<RunManager>();
    }

    public void Died(GameObject originalAnimalObj)
    {
        if(onAnimalDied != null)
        {
            onAnimalDied(originalAnimalObj);
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
        if (animal == null || partner == null) return;
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
            if (partner.reproManager.IsFull()) return;
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
                if(onFinishInteraction != null)
                {
                    onFinishInteraction(reproManager.transform);
                }
                Destroy(reproManager.gameObject);
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
                if(eatingManager.food != null && eatingManager.food.GetComponent<Food>() != null)
                    eatingManager.food.GetComponent<Food>().isBeingEaten = false;
                onEatingFinished(eatingManager, isSuccess);
                if (onFinishInteraction != null)
                {
                    onFinishInteraction(eatingManager.transform);
                }
                Destroy(eatingManager.gameObject);
            }
        }
    }

    //////////Running
    public void StartRunning(Animal hunter, Transform prey)
    {
        if (hunter == null || prey == null) return;
        GameObject runGameObject = new GameObject("Run Manager" + runManagerCount++);
        runGameObject.transform.parent = runManagersParent.transform;
        RunManager runManager = runGameObject.AddComponent<RunManager>();
        runManager.SetHunterAndPrey(hunter, prey);
        hunter.runManager = runManager;
        runManagers.Add(runManager);
        if (onNewInteraction != null)
        {
            onNewInteraction(runManager.transform);
        }
    }

    public void FinishRunning(RunManager runManager)
    {
        if(runManager != null)
        {
            runManagers.Remove(runManager);
            if (onFinishInteraction != null)
            {
                onFinishInteraction(runManager.transform);
            }
            Destroy(runManager.gameObject);
        }
    }

    public void MakePoof(Animal animal)
    {
        Instantiate(meatPrefab, animal.transform.position, meatPrefab.transform.rotation, animal.transform);
        animal.GetComponentInChildren<Renderer>().enabled = false;

        ParticleSystem particle = Instantiate(turnToFoodFX, animal.transform.position, Quaternion.identity);
        Destroy(particle.gameObject, particle.duration + particle.startLifetime/2);
    }

}
