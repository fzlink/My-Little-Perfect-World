using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuData : MonoBehaviour
{
    public static bool created = false;
    public enum HabitatType { TUNDRA = 1, SAVANNAH = 2, RAINFOREST = 3, DESERT = 4};
    public string tundraSceneName;
    public string savannahSceneName;
    public string rainForestSceneName;
    public string desertSceneName;

    public int mapSizeX;
    public int mapSizeY;


    private HabitatType habitatType;
    private List<ObjectsToPlace> objectsToPlaceList;

    public void SaveItems(List<PreviewProperties> previewProperties)
    {
        objectsToPlaceList = new List<ObjectsToPlace>();
        for (int i = 0; i < previewProperties.Count; i++)
        {
            ObjectsToPlace item = new ObjectsToPlace();
            item.population = (int)previewProperties[i].Population;
            item.foodChain = (int)previewProperties[i].FoodChain;
            item.UIIcon = previewProperties[i].Icon;
            item.name = previewProperties[i].Name;
            item.obj = previewProperties[i].ActualObject;
            item.willOptimized = previewProperties[i].WillOptimized;
            objectsToPlaceList.Add(item);
        }
    }

    public float GetSpotPerChunk(int mapChunkCount)
    {
        int total = 0;
        for (int i = 0; i < objectsToPlaceList.Count; i++)
        {
            total += objectsToPlaceList[i].population;
        }
        return total / (float)mapChunkCount;
    }

    public List<ObjectsToPlace> GetItems()
    {
        return objectsToPlaceList;
    }

    public int[] GetMapSizeXY()
    {
        return new int[]{mapSizeX,mapSizeY};
    }

    public string GetScene()
    {
        switch (habitatType)
        {
            case HabitatType.DESERT:
                return desertSceneName;
            case HabitatType.RAINFOREST:
                return rainForestSceneName;
            case HabitatType.SAVANNAH:
                return savannahSceneName;
            case HabitatType.TUNDRA:
                return tundraSceneName;
        }
        return null;
    }

    public void SetMapSize(int mapSizeX, int mapSizeY)
    {
        this.mapSizeX = mapSizeX;
        this.mapSizeY = mapSizeY;
    }

    public void SetHabitatType(string landPieceType)
    {
        switch (landPieceType)
        {
            case "Desert":
                habitatType = HabitatType.DESERT;
                break;
            case "Tundra":
                habitatType = HabitatType.TUNDRA;
                break;
            case "Rain Forest":
                habitatType = HabitatType.RAINFOREST;
                break;
            case "Savannah":
                habitatType = HabitatType.DESERT;
                break;
            default:
                habitatType = HabitatType.DESERT;
                break;
        }
    }

    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
        else
        {
            Destroy(gameObject);
        }

    }

}

