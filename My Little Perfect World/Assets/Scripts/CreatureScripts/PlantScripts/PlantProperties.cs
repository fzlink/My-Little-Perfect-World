using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant Properties", menuName = "Plant/Plant Properties", order = 0)]
public class PlantProperties : Properties
{
    [SerializeField] private GameObject plant;

    [SerializeField] private Vector3 maxGrowth;
    [SerializeField] private float growSpeed;
    [SerializeField] private float maxStarch;
    [SerializeField] private float edibilityThreshold;
    [SerializeField] private float nutritionValue;

    public GameObject Plant { get => plant; set => plant = value; }
    public float EdibilityThreshold { get => edibilityThreshold; set => edibilityThreshold = value; }
    public float GrowSpeed { get => growSpeed; set => growSpeed = value; }
    public float MaxStarch { get => maxStarch; set => maxStarch = value; }
    public Vector3 MaxGrowth { get => maxGrowth; set => maxGrowth = value; }
    public float NutritionValue { get => nutritionValue; set => nutritionValue = value; }
}
