using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Animal Properties", menuName ="Animal/Animal Properties",order = 0)]
public class AnimalProperties : Properties
{
    PopulationOptimizer creatureCountOptimizer;

    [Header("General Values")]
    [SerializeField] private GameObject animal;
    [SerializeField] private Texture animalIcon;
    [SerializeField] private Color commonSkinColor;

    [Header("Wandering Values")]
    [SerializeField] private float wanderingSpeed;
    [SerializeField] private float wanderChangeDirectionDelay;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float awarenessRadius;
    [SerializeField] private Vector3[] lookForWaterdirections;

    [Header("Food Values")]
    [SerializeField] private bool isCarnivore;
    [SerializeField] private bool isHerbivore;
    [SerializeField] private float nutritionValue;
    [SerializeField] private float foodMaximum;
    [SerializeField] private float foodDangerThreshold;
    [SerializeField] private float foodEatingSpeed;
    [SerializeField] private float foodDecreaseSpeed;

    [Header("Water Values")]
    [SerializeField] private float waterMaximum;
    [SerializeField] private float waterDangerThreshold;
    [SerializeField] private float waterDrinkingSpeed;
    [SerializeField] private float waterDecreaseSpeed;

    [Header("Sleep Values")]
    [SerializeField] private float sleepMaximum;
    [SerializeField] private float sleepDangerThreshold;
    [SerializeField] private float sleepGettingSpeed;
    [SerializeField] private float sleepDecreaseSpeed;

    [Header("Social Values")]
    [SerializeField] private float socialMaximum;
    [SerializeField] private float socialDangerThreshold;
    [SerializeField] private float socialGettingSpeed;
    [SerializeField] private float socialDecreaseSpeed;

    [Header("Reproducing Values")]
    [SerializeField] private float reproducingMaximum;
    [SerializeField] private float reproducingSpeed;
    [SerializeField] private float reproducingAge;

    [Header("Pregnancy Values")]
    [SerializeField] private float pregnancyMaximum;
    [SerializeField] private float pregnancySpeed;
    [SerializeField] private int pregnancyChildAmount;
    [SerializeField] private int pregnancyChildAmountMax;
    [SerializeField] private int maximumLifetimeInDays;
    


    public GameObject Animal { get => animal; set => animal = value; }
    public Texture AnimalIcon { get => animalIcon; set => animalIcon = value; }
    public Color CommonSkinColor { get => commonSkinColor; set => commonSkinColor = value; }
    public float WanderingSpeed { get => wanderingSpeed * FourthDimension.tSM; set => wanderingSpeed = value; }
    public float WanderChangeDirectionDelay { get => wanderChangeDirectionDelay / FourthDimension.tSM; set => wanderChangeDirectionDelay = value; }
    public float RunningSpeed { get => runningSpeed * FourthDimension.tSM; set => runningSpeed = value; }
    public float AwarenessRadius { get => awarenessRadius; set => awarenessRadius = value; }
    public Vector3[] LookForWaterdirections { get => lookForWaterdirections; set => lookForWaterdirections = value; }
    public bool IsCarnivore { get => isCarnivore; set => isCarnivore = value; }
    public bool IsHerbivore { get => isHerbivore; set => isHerbivore = value; }
    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
    public float FoodMaximum { get => foodMaximum; set => foodMaximum = value; }
    public float FoodDangerThreshold { get => foodDangerThreshold; set => foodDangerThreshold = value; }
    public float FoodEatingSpeed { get => foodEatingSpeed * FourthDimension.tSM; set => foodEatingSpeed = value; }
    public float FoodDecreaseSpeed { get => foodDecreaseSpeed * FourthDimension.tSM; set => foodDecreaseSpeed = value; }
    public float WaterMaximum { get => waterMaximum; set => waterMaximum = value; }
    public float WaterDangerThreshold { get => waterDangerThreshold; set => waterDangerThreshold = value; }
    public float WaterDrinkingSpeed { get => waterDrinkingSpeed * FourthDimension.tSM; set => waterDrinkingSpeed = value; }
    public float WaterDecreaseSpeed { get => waterDecreaseSpeed * FourthDimension.tSM; set => waterDecreaseSpeed = value; }
    public float SleepMaximum { get => sleepMaximum; set => sleepMaximum = value; }
    public float SleepDangerThreshold { get => sleepDangerThreshold; set => sleepDangerThreshold = value; }
    public float SleepGettingSpeed { get => sleepGettingSpeed * FourthDimension.tSM; set => sleepGettingSpeed = value; }
    public float SleepDecreaseSpeed { get => sleepDecreaseSpeed * FourthDimension.tSM; set => sleepDecreaseSpeed = value; }
    public float SocialMaximum { get => socialMaximum; set => socialMaximum = value; }
    public float SocialDangerThreshold { get => socialDangerThreshold; set => socialDangerThreshold = value; }
    public float SocialGettingSpeed { get => socialGettingSpeed * FourthDimension.tSM; set => socialGettingSpeed = value; }
    public float SocialDecreaseSpeed { get => socialDecreaseSpeed * FourthDimension.tSM; set => socialDecreaseSpeed = value; }
    public float ReproducingMaximum { get => reproducingMaximum; set => reproducingMaximum = value; }
    public float ReproducingSpeed { get => reproducingSpeed * FourthDimension.tSM; set => reproducingSpeed = value; }
    public float ReproducingAge { get => reproducingAge; set => reproducingAge = value; }

    public float PregnancyMaximum { get => pregnancyMaximum; set => pregnancyMaximum = value; }
    public float PregnancySpeed { get {
            if(creatureCountOptimizer == null)
            {
                creatureCountOptimizer = SimulationManger.instance.populationOptimizer;
            }
            return pregnancySpeed* FourthDimension.tSM* creatureCountOptimizer.GetEvaluatedPregnancySpeedFactor(animal);
        }  set => pregnancySpeed = value; }
    public int PregnancyChildAmount { get {
            if (creatureCountOptimizer == null)
            {
                creatureCountOptimizer = SimulationManger.instance.populationOptimizer;
            }
            if (pregnancyChildAmount < pregnancyChildAmountMax)
                return pregnancyChildAmount * creatureCountOptimizer.GetEvaluatedBirthCountFactor(animal);
            else
                return pregnancyChildAmountMax;
        }  set => pregnancyChildAmount = value; }

    public int MaximumLifetimeInDays { get => maximumLifetimeInDays; set => maximumLifetimeInDays = value; }
}
