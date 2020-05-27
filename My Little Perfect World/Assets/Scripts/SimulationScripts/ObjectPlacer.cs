using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<ObjectsToPlace> objectsToPlace;
    public ObjectsToPlace treeObject;
    public event Action<List<ObjectsToPlace>> onObjectsPlaced;

    public Transform creatureContainer;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded += PlaceObjects;
    }


    private void PlaceObjects(List<Vector3> safeSpots)
    {
        List<ObjectsToPlace> advancedObjectsToPlaces = null;
        if (FindObjectOfType<MenuData>() != null)
            advancedObjectsToPlaces = FindObjectOfType<MenuData>().GetItems();
        if(advancedObjectsToPlaces != null)
        {
            SpecificPlacement(safeSpots,advancedObjectsToPlaces);
        }
        else
        {
            RandomPlacement(safeSpots);
        }
    }

    private void SpecificPlacement(List<Vector3> safeSpots, List<ObjectsToPlace> advancedObjectsToPlaces)
    {
        safeSpots = safeSpots.OrderBy(item => Guid.NewGuid()).ToList();
        int spotInd = 0;

        for (int i = 0; i < advancedObjectsToPlaces.Count; i++)
        {
            GameObject g = new GameObject(advancedObjectsToPlaces[i].name);
            g.transform.parent = creatureContainer;
            advancedObjectsToPlaces[i].container = g.transform;
            for (int j = 0; j < advancedObjectsToPlaces[i].population; j++)
            {
                Place(advancedObjectsToPlaces[i], safeSpots[spotInd],j,g.transform);
                spotInd++;
            }
        }
        while(spotInd < safeSpots.Count)
        {
            Instantiate(treeObject.obj, safeSpots[spotInd], Quaternion.identity, treeObject.container);
            spotInd++;
        }
        if(onObjectsPlaced != null)
        {
            onObjectsPlaced(advancedObjectsToPlaces);
        }
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }

    private void Place(ObjectsToPlace advancedObjectsToPlace, Vector3 safeSpot, int ind, Transform container)
    {
        GameObject creature;
        CreatureFactory animalFactory = new AnimalFactory();
        CreatureFactory plantFactory = new PlantFactory();
        if (advancedObjectsToPlace.obj.GetComponent<Creature>() != null)
        {
            if (advancedObjectsToPlace.obj.GetComponent<Creature>().GetType().BaseType == typeof(Animal))
            {
                creature = animalFactory.CreateChild(advancedObjectsToPlace.obj, safeSpot, container);
                creature.GetComponent<Creature>().foodChainIndex = advancedObjectsToPlace.foodChain;
            }
            else
            {
                creature = plantFactory.CreateChild(advancedObjectsToPlace.obj, safeSpot, container);
                if (UnityEngine.Random.value < 0.3f)
                {
                    creature.transform.localScale = Vector3.one * 0.1f;
                    creature.GetComponent<Plant>().isDormant = true;
                }
            }
            creature.name = creature.GetComponent<Creature>().creatureType + ind;
        }
        else
        {
            creature = Instantiate(advancedObjectsToPlace.obj, safeSpot, Quaternion.identity, container);
        }
    }

    private void RandomPlacement(List<Vector3> safeSpots)
    {
        int spotPerChunk = gameObject.GetComponent<SafeSpotFinder>().spotPerChunk;
        bool isTreeChunk = false;
        GameObject creature;
        CreatureFactory animalFactory = new AnimalFactory();
        CreatureFactory plantFactory = new PlantFactory();
        for (int i = 0; i < safeSpots.Count; i++)
        {
            float rnd = UnityEngine.Random.value;
            if (isTreeChunk && i % spotPerChunk == 0)
            {
                isTreeChunk = false;
            }
            else if (i % spotPerChunk == 0 && rnd < treeObject.magnitude)
            {
                isTreeChunk = true;
            }


            if (isTreeChunk)
            {
                Instantiate(treeObject.obj, safeSpots[i], Quaternion.identity, treeObject.container);
            }
            else
            {
                float prev = 0;
                for (int j = 0; j < objectsToPlace.Count; j++)
                {
                    if (rnd < objectsToPlace[j].magnitude + prev)
                    {
                        if (objectsToPlace[j].obj.GetComponent<Creature>() != null)
                        {
                            if (objectsToPlace[j].obj.GetComponent<Creature>().GetType().BaseType == typeof(Animal))
                            {
                                creature = animalFactory.CreateChild(objectsToPlace[j].obj, safeSpots[i], objectsToPlace[j].container);
                            }
                            else
                            {
                                creature = plantFactory.CreateChild(objectsToPlace[j].obj, safeSpots[i], objectsToPlace[j].container);
                                if (UnityEngine.Random.value < 0.3f)
                                {
                                    creature.transform.localScale = Vector3.one * 0.1f;
                                    creature.GetComponent<Plant>().isDormant = true;
                                }
                            }

                            creature.name = creature.GetComponent<Creature>().creatureType + i;
                        }
                        else
                        {
                            creature = Instantiate(objectsToPlace[j].obj, safeSpots[i], Quaternion.identity, objectsToPlace[j].container);
                        }

                        break;
                    }
                    prev += objectsToPlace[j].magnitude;
                }
            }

        }
        if (onObjectsPlaced != null)
        {
            onObjectsPlaced(objectsToPlace);
        }
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }

}
