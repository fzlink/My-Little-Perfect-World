using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreatureUIManager : MonoBehaviour
{

    public RawImage creatureIcon;
    public TMP_Text ageText;
    private StatBar[] statBars;
    public Animal animalOnInterest = null;
    private float animalOnInterestDangerThreshold;

    public static CreatureUIManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        statBars = GetComponentsInChildren<StatBar>();
    }

    // Update is called once per frame
    void Update()
    {
        GetCreatureWithMouseClick();
        if (animalOnInterest != null)
        {
            GetValues();
        }
        else
        {
            ResetValues();
        }
    }

    private void ResetValues()
    {
        creatureIcon.gameObject.SetActive(false);
        for (int i = 0; i < statBars.Length; i++)
        {
            statBars[i].ChangeValues(0, 0);
        }
    }

    public void ChangeInterest(Animal animal)
    {
        if(animalOnInterest != null)
        {
            animalOnInterest.gameObject.GetComponentInChildren<OutlineCamera>().enabled = false;
        }
        animalOnInterest = animal;
        AnimalProperties properties = animal.GetProperties();
        animal.gameObject.GetComponentInChildren<OutlineCamera>().enabled = true;

        Camera.main.GetComponent<CameraTargetFollower>().SetTarget(animal.transform);

        creatureIcon.gameObject.SetActive(true);
        creatureIcon.texture = properties.AnimalIcon;
        for (int i = 0; i < statBars.Length; i++)
        {
            statBars[i].parentCapsule.SetActive(true);
            switch (statBars[i].barType)
            {
                case StatBar.BarType.Food:
                    statBars[i].ChangeInterest(animal.foodAmount, properties.FoodMaximum, properties.FoodDangerThreshold);
                    break;
                case StatBar.BarType.Water:
                    statBars[i].ChangeInterest(animal.waterAmount, properties.WaterMaximum, properties.WaterDangerThreshold);
                    break;
                case StatBar.BarType.Sleep:
                    statBars[i].ChangeInterest(animal.sleepAmount, properties.SleepMaximum, properties.SleepDangerThreshold);
                    break;
                case StatBar.BarType.Stress:
                    statBars[i].ChangeInterest(animal.socialAmount, properties.SocialMaximum, properties.SocialDangerThreshold);
                    break;
                case StatBar.BarType.Pregnancy:
                    if(animal.dNA.sex == Sex.Female)
                    {
                        statBars[i].ChangeInterest(animal.GetComponent<FemaleAttributes>().pregnancyAmount, properties.PregnancyMaximum, 0f);
                    }
                    else
                    {
                        statBars[i].parentCapsule.SetActive(false);
                    }
                    break;
                default:
                    statBars[i].parentCapsule.SetActive(false);
                    break;
            }
        }
        ageText.text = "Age: " + animal.age;
    }

    private void GetValues()
    {
        AnimalProperties properties = animalOnInterest.GetProperties();
        for(int i=0; i < statBars.Length; i++)
        {
            if (statBars[i].gameObject.activeInHierarchy)
            {
                switch (statBars[i].barType)
                {
                    case StatBar.BarType.Food:
                        statBars[i].ChangeValues(animalOnInterest.foodAmount, properties.FoodDangerThreshold);
                        break;
                    case StatBar.BarType.Water:
                        statBars[i].ChangeValues(animalOnInterest.waterAmount, properties.WaterDangerThreshold);
                        break;
                    case StatBar.BarType.Sleep:
                        statBars[i].ChangeValues(animalOnInterest.sleepAmount, properties.SleepDangerThreshold);
                        break;
                    case StatBar.BarType.Stress:
                        statBars[i].ChangeValues(animalOnInterest.socialAmount, properties.SocialDangerThreshold);
                        break;
                    case StatBar.BarType.Pregnancy:
                        statBars[i].ChangeValues(animalOnInterest.GetComponent<FemaleAttributes>().pregnancyAmount, 0f);
                        break;
                    default:
                        break;

                }
            }
            ageText.text = "Age: " + animalOnInterest.age;
        }
    }



    private void GetCreatureWithMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, Masks.animalMask))
            {
                if(hit.collider.gameObject.GetComponent<Animal>() != null)
                {
                    ChangeInterest(hit.collider.gameObject.GetComponent<Animal>());
                }
            }
        }
    }
}
