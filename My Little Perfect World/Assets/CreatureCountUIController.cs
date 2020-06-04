using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureCountUIController : MonoBehaviour
{

    private AnimalInteractionManager animalInteractionManager;

    private ScrollRect scrollRect;
    private ObjectPlacer objectPlacer;
    public GameObject countContainer;

    private List<Transform> creatureContainers;
    private List<GameObject> creatureCountContainer;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        objectPlacer = FindObjectOfType<ObjectPlacer>();
        animalInteractionManager = FindObjectOfType<AnimalInteractionManager>();
    }

    void Start()
    {
        objectPlacer.onObjectsPlaced += InitCreatures;
        animalInteractionManager.onAnimalDied += UpdateCreatureCount;
    }

    private void UpdateCreatureCount(GameObject animal)
    {
        for (int i = 0; i < creatureContainers.Count; i++)
        {
            creatureCountContainer[i].GetComponentInChildren<TMP_Text>().text = creatureContainers[i].childCount.ToString();
        }
    }

    private void InitCreatures(List<ObjectsToPlace> creatures)
    {
        creatureContainers = new List<Transform>(creatures.Count);
        creatureCountContainer = new List<GameObject>(creatures.Count);
        for (int i = 0; i < creatures.Count; i++)
        {
            GameObject g = Instantiate(countContainer);
            g.transform.SetParent(scrollRect.content.GetComponent<RectTransform>());
            g.transform.localScale = Vector3.one;

            g.GetComponentInChildren<RawImage>().texture = creatures[i].UIIcon;
            g.GetComponentInChildren<TMP_Text>().text = creatures[i].container.childCount.ToString();

            creatureContainers.Add(creatures[i].container);
            creatureCountContainer.Add(g);
        }
    }

}
