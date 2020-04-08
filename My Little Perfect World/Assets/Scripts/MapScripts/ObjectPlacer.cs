using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{


    public GameObject[] creatures;
    public Properties[] creatureProperties;
    public Transform[] creatureContainers;
    public float[] creaturePercent;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded += PlaceObjects;
    }


    private void PlaceObjects(List<Vector3> safeSpots)
    {
        for (int i = 0; i < safeSpots.Count; i++)
        {
            float prev = 0;
            float rnd = UnityEngine.Random.value;
            for(int j = 0; j < creatures.Length; j++)
            {
                if(rnd < creaturePercent[j] + prev)
                {
                    //Type type = creatures[j].GetComponent<Creature>().GetType();
                    ////var type = typeof(ctype);
                    //var field = type.GetField("properties", BindingFlags.Public | BindingFlags.Static);
                    //var obj = type.GetProperty("properties");
                    //field.SetValue(null, creatureProperties[j]);
                    GameObject creature;
                    if(creatures[j].GetComponent<Creature>().GetType() == typeof(Animal))
                    {
                        creature = AnimalFactory.CreateChild(creatures[j], safeSpots[i], creatureContainers[j]);
                    }
                    else
                    {
                        creature = Instantiate(creatures[j], safeSpots[i], Quaternion.identity, creatureContainers[j]);
                        Plant.properties = (PlantProperties) creatureProperties[2];
                    }
                    creature.name = creature.GetComponent<Creature>().creatureType + i;
                    break;
                }
                prev += creaturePercent[j];
            }
            //if (rnd < .35)
            //{
            //    GameObject plant = Instantiate(plants[0], safeSpots[i], Quaternion.identity, plantContainers[0]);
            //    plant.name = "Plant" + i;
            //    Plant.properties = (PlantProperties)plantProperties;
            //}
            //else if (rnd < .85)
            //{
            //    GameObject prey = AnimalFactory.CreateChild(animals[0], safeSpots[i], animalContainers[0]);
            //    prey.name = "Prey" + i;
            //}
            //else
            //{
            //    GameObject hunter = AnimalFactory.CreateChild(animals[1], safeSpots[i], animalContainers[1]);
            //    hunter.GetComponent<Animal>().foodChainIndex = 1;
            //    hunter.name = "Hunter" + i;
            //}
        }
        print("Prey Count : " + creatureContainers[0].childCount);
        print("Hunter Count : " + creatureContainers[1].childCount);
        print("Plant Count : " + creatureContainers[2].childCount);
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }
}
