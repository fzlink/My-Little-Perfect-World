using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu()]
public class PreviewProperties : Properties
{
    public enum PreviewType { PLANT = 1, ANIMAL_HERBIVORE = 2, ANIMAL_CARNIVORE = 3, ANIMAL_OMNIVORE = 4 }

    [SerializeField] private Texture icon;
    [SerializeField] private GameObject previewObject;
    [SerializeField] private string name;
    [SerializeField] private PreviewType type;
    [SerializeField] private float population;
    [SerializeField] private float foodChain;

    public Texture Icon { get => icon; set => icon = value; }
    public GameObject PreviewObject { get => previewObject; set => previewObject = value; }
    public string Name { get => name; set => name = value; }
    public PreviewType TYPE { get => type; set => type = value; }
    public float Population { get => population; set => population = value; }
    public float FoodChain { get => foodChain; set => foodChain = value; }
}

