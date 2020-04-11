using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalState {
    Wandering = 0, //Wander, ScanForWater,ScanForFood,ScanForEnemy,ScanForPartner
    Sleeping = 1, //Sleep
    Drinking = 2, //ScanForOtherCreature, Drink
    Eating = 3, //ScanForOtherCreature, MakeNoise(maybe),Eat
    Reproducing = 4, //ScanForOtherCreature, Reproduce
    GoingToSomething = 5
}

public class Animal : Creature
{

    public AnimalState state { get; set; }

    public DNA dNA { get; set; }
    [SerializeField] private AnimalProperties properties;
    public AnimalProperties GetProperties() { return properties; }

    public float foodAmount { get; set; }
    public float waterAmount { get; set; }
    public float sleepAmount { get; set; }
    public float stressAmount { get; set; }

    public bool isHungry { get; set; }
    public bool isThirsty { get; set; }
    public bool isSleepy { get; set; }
    public bool isStressed { get; set; }

    public bool isCarnivore => properties.IsCarnivore;
    public bool isHerbivore => properties.IsHerbivore;

    public float age => FourthDimension.currentDay - dayOfBirth;
    public Sex sex => dNA.sex;

    public Food currentFood { get; set; }
    public Animal currentPartner { get; set; }
    public Transform currentEnemy { get; set; }

    public bool hasAncestor { get; set; }
    public int dayOfBirth { get; set; }
    public Animal mother { get; set; }
    public Animal father { get; set; }
    public List<Animal> childs { get; set; }

    public ReproManager reproManager { get; set; }
    public EatingManager eatingManager { get; set; }

    private Plant currentPlant;

    private void Start()
    {
        GetComponent<ScanController>().onTargetFound += OnTargetFound;
        childs = new List<Animal>();
        InitValues();
    }

    protected virtual void InitValues()
    {

        foodAmount = UnityEngine.Random.Range(25f, 100f);
        waterAmount = UnityEngine.Random.Range(25f, 100f);
        sleepAmount = UnityEngine.Random.Range(25f, 100f);
    }

    private void Update()
    {
        switch (state)
        {
            case AnimalState.Sleeping:
                Sleep();
                break;
            case AnimalState.Reproducing:
                break;
            case AnimalState.Eating:
                Eat();
                //MakeNoise();
                break;
            case AnimalState.Drinking:
                DrinkWater();
                break;
            default:
                break;
        }
        ReEvaluateProperties();
    }

    private void OnTargetFound(Location target)
    {
        switch (target.GetLocationType())
        {
            case LocationType.Enemy:
                currentEnemy = target.GetObjectToFollow();
                Interrupted();
                break;
            case LocationType.Partner:
                currentPartner = target.GetObjectToFollow().GetComponent<Animal>();
                break;
            case LocationType.Food:
                currentFood = target.GetObjectToFollow().GetComponent<Food>();
                break;
            case LocationType.Plant:
                currentPlant = target.GetObjectToFollow().GetComponent<Plant>();
                break;
        }
        state = AnimalState.GoingToSomething;
    }


    ///////////////Food
    public void FoodConfrontation()
    {
        if(AnimalInteractionManager.instance.StartEating(this, currentFood))
        {
            AnimalInteractionManager.instance.onEatingFinished += OnEatingFinished;
            state = AnimalState.Eating;
        }
    }

    private void OnEatingFinished(EatingManager eatingManager, bool isSuccess)
    {
        if (this.eatingManager == eatingManager && isSuccess && currentFood != null)
        {
            currentFood.Finish(this);
            currentFood = null;
        }
        if (this.eatingManager == eatingManager)
        {
            eatingManager = null;
            state = AnimalState.Wandering;
            AnimalInteractionManager.instance.onEatingFinished -= OnEatingFinished;
        }
    }

    private void Eat()
    {
        if (currentFood == null)
        {
            state = AnimalState.Wandering;
        }
        else
        {
            foodAmount += properties.FoodEatingSpeed * Time.deltaTime * currentFood.nutritionValue;
            if (foodAmount > properties.FoodMaximum)
            {
                foodAmount = properties.FoodMaximum;
                state = AnimalState.Wandering;
            }
        }
    }

    public void PlantConfrontation()
    {
        if(currentPlant != null)
        {
            currentFood = currentPlant.Die();
            FoodConfrontation();
        }
    }

    ///////////////Water
    public void WaterConfrontation()
    {
        state = AnimalState.Drinking;
    }

    private void DrinkWater()
    {
        waterAmount += properties.WaterDrinkingSpeed * Time.deltaTime;
        if (waterAmount > properties.WaterMaximum)
        {
            waterAmount = properties.WaterMaximum;
            //RotationEvent(-transform.forward, 0);
            state = AnimalState.Wandering;
        }
    }

    ///////////////Sleep
    private void Sleep()
    {
        sleepAmount += properties.SleepGettingSpeed * Time.deltaTime;
        if (sleepAmount >= properties.SleepMaximum)
        {
            sleepAmount = properties.SleepMaximum;
            state = AnimalState.Wandering;
        }
    }

    ///////////////Fight
    public void EnemyConfrontation()
    {
        //if (currentEnemy.GetComponent<Animal>().foodChainIndex > this.foodChainIndex)
        //{
        //    Die();
        //}
        try
        {
            if(currentEnemy.GetComponent<Animal>().foodChainIndex < this.foodChainIndex)
            {
                currentEnemy.GetComponent<Animal>().Die();
                currentFood = currentEnemy.GetComponent<Food>();
                FoodConfrontation();
            }

        }
        catch { }
    }

    public void EnemyLost()
    {
        currentEnemy = null;
        state = AnimalState.Wandering;
    }

    private void Die()
    {
        Meat meat = gameObject.AddComponent<Meat>();
        AnimalInteractionManager.instance.MakePoof(this);
        meat.nutritionValue = properties.NutritionValue;
        meat.animalType = tag;
        Destroy(this);
        Destroy(GetComponent<MoveController>());
        Destroy(GetComponent<ScanController>());
        if (sex == Sex.Female)
            Destroy(GetComponent<FemaleAttributes>());
        transform.parent = AnimalInteractionManager.instance.foodContainer.transform;
        AnimalInteractionManager.instance.PrintDeadCount();
    }

    ///////////////Reproduction

    public void PartnerConfrontation()
    {
        AnimalInteractionManager.instance.StartReproducing(this, currentPartner, properties.ReproducingSpeed, properties.ReproducingMaximum);
        AnimalInteractionManager.instance.onReproducingFinished += OnReproductionFinished;
        state = AnimalState.Reproducing;
    }

    private void OnReproductionFinished(ReproManager reproManager, bool isSuccess)
    {
        if(this.reproManager == reproManager && isSuccess)
        {
            if(sex == Sex.Female)
            {
                GetComponent<FemaleAttributes>().isPregnant = true;
            }
        }
        if(this.reproManager == reproManager)
        {
            reproManager = null;
            state = AnimalState.Wandering;
            AnimalInteractionManager.instance.onReproducingFinished -= OnReproductionFinished;
        }
    }
    
    public bool isEligiblePartner(Animal potentialPartner) // Bad
    {
        if (sex == Sex.Female)
            return !(GetComponent<FemaleAttributes>().isPregnant) && age >= properties.ReproducingAge && (state == AnimalState.Wandering || (state == AnimalState.GoingToSomething && potentialPartner == currentPartner));
        else
            return age >= properties.ReproducingAge && (state == AnimalState.Wandering || (state == AnimalState.GoingToSomething && potentialPartner == currentPartner));
    }


    //////////////////Misc

    public void Interrupted()
    {
        AnimalInteractionManager.instance.Interrupted(this);
    }

    private void ReEvaluateProperties()
    {
        if(state != AnimalState.Drinking)
        {
            waterAmount -= Time.deltaTime;
            if(waterAmount < 0) { Interrupted(); Die(); }
        }
        if(waterAmount < properties.WaterDangerThreshold)
        {
            isThirsty = true;
        }
        else
        {
            isThirsty = false;
        }

        //////////////////////
        if (state != AnimalState.Eating)
        {
            foodAmount -= Time.deltaTime;
            if (foodAmount < 0) { Interrupted();  Die(); }
        }
        if (foodAmount < properties.FoodDangerThreshold)
        {
            isHungry = true;
        }
        else
        {
            isHungry = false;
        }
        ///////////////////////
        
        if(state != AnimalState.Sleeping)
        {
            sleepAmount -= Time.deltaTime;
            if(state == AnimalState.Wandering && sleepAmount < properties.SleepDangerThreshold)
            {
                if(sleepAmount <= 0) { sleepAmount = 0; }
                state = AnimalState.Sleeping;
            }
        }
    }
    
 
    private void OnDestroy()
    {
        GetComponent<ScanController>().onTargetFound -= OnTargetFound;
    }

}

