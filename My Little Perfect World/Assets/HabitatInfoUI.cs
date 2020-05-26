using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class HabitatInfoUI : MonoBehaviour
{
    public TMP_Text habitatTitleText;
    public TMP_Text habitatInfoText;
    private float startScaleY;

    // Start is called before the first frame update
    void Start()
    {
        startScaleY = transform.localScale.y;
        MainMenu mainMenu = FindObjectOfType<MainMenu>();
        mainMenu.OnHabitatSelected += ShowInfo;
    }
    
    private void ShowInfo(string landPieceType)
    {
        AnimateFrame();
        habitatTitleText.SetText(landPieceType);
        string info = "";
        switch (landPieceType)
        {
            case "Desert":
                info = "A desert is a barren area of landscape where little precipitation occurs and, consequently, living conditions are hostile for plant and animal life. The lack of vegetation exposes the unprotected surface of the ground to the processes of denudation. About one-third of the land surface of the world is arid or semi-arid.";
                break;
            case "Tundra":
                info = "In physical geography, tundra (/ˈtʌndrə, ˈtʊn-/) is a type of biome where the tree growth is hindered by low temperatures and short growing seasons. The term tundra comes through Russian тундра (tûndra) from the Kildin Sámi word тӯндар (tūndâr) meaning \"uplands\", \"treeless mountain tract\". Tundra vegetation is composed of dwarf shrubs, sedges and grasses, mosses, and lichens. Scattered trees grow in some tundra regions. The ecotone (or ecological boundary region) between the tundra and the forest is known as the tree line or timberline. The tundra soil is rich in nitrogen and phosphorus.";
                break;
            case "Rain Forest":
                info = "Rainforests are forests characterized by high and continuous rainfall, with annual rainfall in the case of tropical rainforests between 2.5 and 4.5 metres (98 and 177 in)[1] and definitions varying by region for temperate rainforests. The monsoon trough, alternatively known as the intertropical convergence zone, plays a significant role in creating the climatic conditions necessary for the Earth's tropical rainforests: which are distinct from monsoonal areas of seasonal tropical forest.";
                break;
            case "Savannah":
                info = "A savanna or savannah is a mixed woodland-grassland ecosystem characterised by the trees being sufficiently widely spaced so that the canopy does not close. The open canopy allows sufficient light to reach the ground to support an unbroken herbaceous layer consisting primarily of grasses.";
                break;
        }
        habitatInfoText.SetText(info);
    }

    private void AnimateFrame()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.2f, transform.localScale.z);
        transform.DOScaleY(startScaleY, 0.5f);
    }
}
