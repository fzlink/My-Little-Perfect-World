using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectPropertiesMenu : MonoBehaviour
{
    public Transform previewItemModel;
    public TMP_Text itemNameText;

    public Button addButton;
    public Button removeButton;

    private PreviewProperties currentItemProperties;
    private GameObject currentListItem;
    private Dictionary<PreviewProperties, GameObject> addedItems;

    public RectTransform addedList;

    public Button startButton;

    public Slider populationSlider;
    public Slider foodChainSlider;
    public TMP_Text populationCountText;
    public TMP_Text foodChainCountText;
    public TMP_Text optimalHabitatText;

    public GameObject alertDialog;

    public Toggle willOptimizedToggle;
    int mapSizeMagnitude;
    void Start()
    {
        DisableUIElements();
        populationSlider.onValueChanged.AddListener(SetPopulationCount);
        foodChainSlider.onValueChanged.AddListener(SetFoodChainCount);

        MenuData menuData = FindObjectOfType<MenuData>();
        int[] mapSize = menuData.GetMapSizeXY();
        mapSizeMagnitude = (mapSize[0]-1) * (mapSize[1]-1);

        SetPopulationSliderMax(mapSizeMagnitude * 10);

        foodChainSlider.maxValue = 5;
        addedItems = new Dictionary<PreviewProperties, GameObject>();
        PreviewListView previewListView = FindObjectOfType<PreviewListView>();
        previewListView.OnItemSelected += GetItem;
        previewListView.OnSectionChanged += Clear;

        addButton.onClick.AddListener(() => AddItem());
        removeButton.onClick.AddListener(() => RemoveItem());
        startButton.onClick.AddListener(() => StartSim());
        alertDialog.GetComponentInChildren<Button>().onClick.AddListener(() => alertDialog.SetActive(false));
    }

    private void SetPopulationSliderMax(float maxVal)
    {
        populationSlider.minValue = 1;
        populationSlider.maxValue = maxVal;
        populationSlider.value = populationSlider.maxValue / 2;
    }

    private void Clear()
    {
        DisableUIElements();
        previewItemModel.gameObject.SetActive(false);
    }

    private void DisableUIElements()
    {
        removeButton.interactable = false;
        addButton.interactable = false;
        itemNameText.gameObject.SetActive(false);
        populationSlider.transform.parent.gameObject.SetActive(false);
        foodChainSlider.transform.parent.gameObject.SetActive(false);
        willOptimizedToggle.gameObject.SetActive(false);
        optimalHabitatText.gameObject.SetActive(false);
    }

    private void EnableUIElements()
    {
        removeButton.interactable = true;
        addButton.interactable = true;
        itemNameText.gameObject.SetActive(true);
        populationSlider.transform.parent.gameObject.SetActive(true);
        foodChainSlider.transform.parent.gameObject.SetActive(true);
        willOptimizedToggle.gameObject.SetActive(true);
        optimalHabitatText.gameObject.SetActive(true);
    }

    private void StartSim()
    {
        bool[] requirements = new bool[3];
        foreach (var item in addedItems.Keys)
        {
            if (item.TYPE == PreviewProperties.PreviewType.PLANT) requirements[0] = true;
            else if (item.TYPE == PreviewProperties.PreviewType.ANIMAL_HERBIVORE) requirements[1] = true;
            else if (item.TYPE == PreviewProperties.PreviewType.ANIMAL_CARNIVORE) requirements[2] = true;
        }
        bool error = false;
        for (int i = 0; i < requirements.Length; i++)
        {
            if (requirements[i] == false)
                error = true;
        }
        if (error)
        {
            alertDialog.SetActive(true);
        }
        else
        {
            FindObjectOfType<MenuData>().SaveItems(addedItems.Keys.ToList());
            LoadSim();
        }
    }

    private void LoadSim()
    {
        string simScene = FindObjectOfType<MenuData>().GetScene();
        SceneManager.LoadScene(simScene);
    }

    private void RemoveItem()
    {
        GameObject g;
        if (addedItems.TryGetValue(currentItemProperties, out g))
        {
            Destroy(g);
            addedItems.Remove(currentItemProperties);
        }
    }

    private void AddItem()
    {
        if (!addedItems.ContainsKey(currentItemProperties))
        {
            GameObject g = Instantiate(currentListItem, addedList);
            currentItemProperties.Population = populationSlider.value;
            currentItemProperties.FoodChain = foodChainSlider.value;
            currentItemProperties.WillOptimized = willOptimizedToggle.isOn;
            willOptimizedToggle.isOn = false;
            addedItems.Add(currentItemProperties,g);
        }
    }

    private void SetPopulationCount(float val)
    {
        populationSlider.value = val;
        populationCountText.text = val.ToString();
    }

    private void SetFoodChainCount(float val)
    {
        foodChainSlider.value = val;
        foodChainCountText.text = val.ToString();
    }

    private void GetItem(PreviewProperties properties, GameObject listItem)
    {
        currentItemProperties = properties;
        currentListItem = listItem;
        EnableUIElements();
        previewItemModel.gameObject.SetActive(true);
        foreach (Transform item in previewItemModel)
        {
            Destroy(item.gameObject);
        }
        GameObject g = Instantiate(properties.PreviewObject, previewItemModel.transform.position, Quaternion.Euler(0,180,0),previewItemModel);
        if(g.GetComponent<RotateAround>() == null)
        {
            RotateAround r = g.AddComponent<RotateAround>();
            r.rotationSpeed = 25;
        }
        itemNameText.SetText(properties.Name);
        SetPopulationCount(populationSlider.maxValue / 2);
        if(properties.TYPE == PreviewProperties.PreviewType.PLANT)
        {
            willOptimizedToggle.gameObject.SetActive(false);
            foodChainSlider.transform.parent.gameObject.SetActive(false);
            SetPopulationSliderMax(mapSizeMagnitude * 20);
        }
        else
        {
            foodChainSlider.transform.parent.gameObject.SetActive(true);
            willOptimizedToggle.gameObject.SetActive(true);
            SetFoodChainCount(0);
            SetPopulationSliderMax(mapSizeMagnitude * 10);
        }
        optimalHabitatText.text = "Optimal Habitat: " + properties.OptimalHabitat;

    }
}
