using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiscUIManager : MonoBehaviour
{
    public RawImage dayNightIcon;
    public Texture dayTexture;
    public Texture nightTexture;

    public Text dayText;

    public Text carbondioxideText;
    public Text oxygenText;
    public Text nitrogenText;

    private Air air;
    void Start()
    {
        air = FindObjectOfType<Air>();
        StartCoroutine(ChangeDayStat());
        StartCoroutine(ChangeAirStat());
    }

    private IEnumerator ChangeDayStat()
    {
        while (true)
        {
            dayText.text = FourthDimension.currentDay + "";
            dayNightIcon.texture = FourthDimension.timeOfDay == FourthDimension.TimeOfDay.Day ? dayTexture : nightTexture;
            yield return new WaitForSeconds(2f);
        }
    }

   
    private IEnumerator ChangeAirStat()
    {
        while (true)
        {
            carbondioxideText.text = air.carbondioxide.ToString("F3") + "%";
            oxygenText.text = air.oxygen.ToString("F1") +  "%";
            nitrogenText.text = air.nitrogen.ToString("F2") + "%";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
