using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Animal/AnimalProperties")]
public class AnimalProperties : ScriptableObject
{
    [SerializeField] private float wanderingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float awarenessRadius;
    [SerializeField] private float fallRiseSpeed;
    [SerializeField] private float changeDirectionDelay;
    [SerializeField] private float drinkSpeed;
    [SerializeField] private float eatingSpeed;
    [SerializeField] private float thirstThreshold;
    [SerializeField] private Color commonSkinColor;
    [SerializeField] private Vector3[] directions;

    public float WanderingSpeed { get => wanderingSpeed; set => wanderingSpeed = value; }
    public float RunningSpeed { get => runningSpeed; set => runningSpeed = value; }
    public float AwarenessRadius { get => awarenessRadius; set => awarenessRadius = value; }
    public float FallRiseSpeed { get => fallRiseSpeed; set => fallRiseSpeed = value; }
    public float ChangeDirectionDelay { get => changeDirectionDelay; set => changeDirectionDelay = value; }
    public float DrinkSpeed { get => drinkSpeed; set => drinkSpeed = value; }
    public float EatingSpeed { get => eatingSpeed; set => eatingSpeed = value; }
    public float ThirstThreshold { get => thirstThreshold; set => thirstThreshold = value; }
    public Color CommonSkinColor { get => commonSkinColor; set => commonSkinColor = value; }
    public Vector3[] Directions { get => directions; set => directions = value; }
}
