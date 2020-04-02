using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{

    public GameObject tree;
    public GameObject prey;
    public GameObject hunter;

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
            Instantiate(prey, safeSpots[i], Quaternion.identity);
        }
        gameObject.GetComponent<SafeSpotFinder>().onSafeSpotsFounded -= PlaceObjects;
    }
}
