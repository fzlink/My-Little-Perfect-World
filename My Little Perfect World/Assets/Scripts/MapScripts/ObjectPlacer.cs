using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{

    public GameObject tree;

    public List<GameObject> animals;
    public List<Transform> animalContainers;

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
            GameObject child = AnimalFactory.CreateChild(animals[0],safeSpots[i],animalContainers[0]);
            //Instantiate(animals[0], safeSpots[i], Quaternion.identity);
            child.name = "Prey" + i;
        }
        for (int i = 0; i < safeSpots.Count; i+= 5)
        {
            GameObject child = AnimalFactory.CreateChild(animals[1], safeSpots[i], animalContainers[1]);
            child.GetComponent<Animal>().foodChainIndex = 1;
            child.name = "Hunter" + i;
        }
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }
}
