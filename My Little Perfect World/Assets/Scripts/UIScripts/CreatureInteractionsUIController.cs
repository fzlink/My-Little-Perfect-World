using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreatureInteractionsUIController : MonoBehaviour
{
    private AnimalInteractionManager animalInteractionManager;

    public ScrollRect scrollRect;
    public Transform interactionsContainer;

    public GameObject interactionItem;
    private List<GameObject> items;

    private void Awake()
    {
        animalInteractionManager = SimulationManger.instance.animalInteractionManager;
    }

    private void Start()
    {
        items = new List<GameObject>();
        animalInteractionManager.onNewInteraction += AddNewInteraction;
        animalInteractionManager.onFinishInteraction += DeleteInteraction;
    }

    public void ToggleScrollViewVisibility()
    {
        scrollRect.gameObject.SetActive(!scrollRect.gameObject.activeSelf);
    }

    private void AddNewInteraction(Transform interactionTransform)
    {
        GameObject g = Instantiate(interactionItem);

        g.transform.SetParent(scrollRect.content.GetComponent<RectTransform>());
        g.transform.localScale = Vector3.one;

        g.GetComponent<AnimalInteractionUIItem>().interactionTransform = interactionTransform;
        g.GetComponentInChildren<TMP_Text>().text = interactionTransform.name;
        items.Add(g);
    }

    private void DeleteInteraction(Transform interactionTransform)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i].GetComponent<AnimalInteractionUIItem>().interactionTransform == interactionTransform)
            {
                Destroy(items[i]);
                items.Remove(items[i]);
            }
        }
    }


}
