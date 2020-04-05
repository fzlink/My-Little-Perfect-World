using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Animal Properties", menuName ="Animal/Animal Properties",order = 0)]
public class AnimalProperties : ScriptableObject
{
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

    [Header("Stress Values")]
    [SerializeField] private float stressMaximum;
    [SerializeField] private float stressDangerThreshold;
    [SerializeField] private float stressGettingSpeed;
    [SerializeField] private float stressDecreaseSpeed;

    [Header("Reproducing Values")]
    [SerializeField] private float reproducingMaximum;
    [SerializeField] private float reproducingSpeed;
    [SerializeField] private float reproducingAge;



    public GameObject Animal { get => animal; set => animal = value; }
    public Texture AnimalIcon { get => animalIcon; set => animalIcon = value; }
    public Color CommonSkinColor { get => commonSkinColor; set => commonSkinColor = value; }
    public float WanderingSpeed { get => wanderingSpeed; set => wanderingSpeed = value; }
    public float WanderChangeDirectionDelay { get => wanderChangeDirectionDelay; set => wanderChangeDirectionDelay = value; }
    public float RunningSpeed { get => runningSpeed; set => runningSpeed = value; }
    public float AwarenessRadius { get => awarenessRadius; set => awarenessRadius = value; }
    public Vector3[] LookForWaterdirections { get => lookForWaterdirections; set => lookForWaterdirections = value; }
    public bool IsCarnivore { get => isCarnivore; set => isCarnivore = value; }
    public bool IsHerbivore { get => isHerbivore; set => isHerbivore = value; }
    public float FoodMaximum { get => foodMaximum; set => foodMaximum = value; }
    public float FoodDangerThreshold { get => foodDangerThreshold; set => foodDangerThreshold = value; }
    public float FoodEatingSpeed { get => foodEatingSpeed; set => foodEatingSpeed = value; }
    public float FoodDecreaseSpeed { get => foodDecreaseSpeed; set => foodDecreaseSpeed = value; }
    public float WaterMaximum { get => waterMaximum; set => waterMaximum = value; }
    public float WaterDangerThreshold { get => waterDangerThreshold; set => waterDangerThreshold = value; }
    public float WaterDrinkingSpeed { get => waterDrinkingSpeed; set => waterDrinkingSpeed = value; }
    public float WaterDecreaseSpeed { get => waterDecreaseSpeed; set => waterDecreaseSpeed = value; }
    public float SleepMaximum { get => sleepMaximum; set => sleepMaximum = value; }
    public float SleepDangerThreshold { get => sleepDangerThreshold; set => sleepDangerThreshold = value; }
    public float SleepGettingSpeed { get => sleepGettingSpeed; set => sleepGettingSpeed = value; }
    public float SleepDecreaseSpeed { get => sleepDecreaseSpeed; set => sleepDecreaseSpeed = value; }
    public float StressMaximum { get => stressMaximum; set => stressMaximum = value; }
    public float StressDangerThreshold { get => stressDangerThreshold; set => stressDangerThreshold = value; }
    public float StressGettingSpeed { get => stressGettingSpeed; set => stressGettingSpeed = value; }
    public float StressDecreaseSpeed { get => stressDecreaseSpeed; set => stressDecreaseSpeed = value; }
    public float ReproducingMaximum { get => reproducingMaximum; set => reproducingMaximum = value; }
    public float ReproducingSpeed { get => reproducingSpeed; set => reproducingSpeed = value; }
    public float ReproducingAge { get => reproducingAge; set => reproducingAge = value; }

}
