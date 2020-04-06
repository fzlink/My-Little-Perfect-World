using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{

    public GameObject tree;

    public List<GameObject> animals;
    public List<Transform> animalContainers;

    public List<GameObject> plants;
    public List<Transform> plantContainers;


    private bool startedPlacing;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded += PlaceObjects;
    }


    private void PlaceObjects(List<Vector3> safeSpots)
    {
        for (int i = 0; i < safeSpots.Count; i++)
        {
            float rnd = UnityEngine.Random.value;
            if(rnd < .35)
            {
                GameObject plant = Instantiate(plants[0], safeSpots[i], Quaternion.identity);
                plant.name = "Plant" + i;
            }
            else if(rnd < .85)
            {
                GameObject prey = AnimalFactory.CreateChild(animals[0], safeSpots[i], animalContainers[0]);
                prey.name = "Prey" + i;
            }
            else
            {
                GameObject hunter = AnimalFactory.CreateChild(animals[1], safeSpots[i], animalContainers[1]);
                hunter.GetComponent<Animal>().foodChainIndex = 1;
                hunter.name = "Hunter" + i;
            }
        }
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }
}
