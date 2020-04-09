using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public List<ObjectsToPlace> objectsToPlace;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded += PlaceObjects;
    }


    private void PlaceObjects(List<Vector3> safeSpots)
    {
        GameObject creature;
        CreatureFactory animalFactory = new AnimalFactory();
        CreatureFactory plantFactory = new PlantFactory();
        for (int i = 0; i < safeSpots.Count; i++)
        {
            float prev = 0;
            float rnd = UnityEngine.Random.value;
            for(int j = 0; j < objectsToPlace.Count; j++)
            {
                if(rnd < objectsToPlace[j].magnitude + prev)
                {
                    if(objectsToPlace[j].obj.GetComponent<Creature>() != null)
                    {
                        if (objectsToPlace[j].obj.GetComponent<Creature>().GetType() == typeof(Animal))
                        {
                            creature = animalFactory.CreateChild(objectsToPlace[j].obj, safeSpots[i], objectsToPlace[j].container);
                        }
                        else
                        {
                            creature = plantFactory.CreateChild(objectsToPlace[j].obj, safeSpots[i], objectsToPlace[j].container);
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
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }
}
