using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MiscUIManager : MonoBehaviour
{
    public RawImage dayNightIcon;
    public Texture dayTexture;
    public Texture nightTexture;

    public Text dayText;

    public Text carbondioxideText;
    public Text oxygenText;
    public Text nitrogenText;
    public TMP_Text timeMultiplierText;

    private Air air;
    private FourthDimension fourthDimension;

    private float changeDayStatDelay = 2f;
    private float changeDayStatTimer;


    private void Awake()
    {
        air = SimulationManger.instance.air;
        fourthDimension = SimulationManger.instance.timeManager;
    }

    void Start()
    {
        timeMultiplierText.text = FourthDimension.tSM.ToString() + "x";
        fourthDimension.onDayNightChange += ChangeDayNightIcon;
        fourthDimension.onPassDay += ChangeDayText;
        dayText.text = "0";
        //StartCoroutine(ChangeAirStat());
    }



    public void StopTime()
    {
        Time.timeScale = 0;
        FourthDimension.tSM = 0;
        timeMultiplierText.text = FourthDimension.tSM.ToString() + "x";
    }

    public void NormalSpeed()
    {
        Time.timeScale = 1;
        FourthDimension.tSM = 1;
        timeMultiplierText.text = FourthDimension.tSM.ToString() + "x";
    }

    public void DoubleSpeed()
    {
        Time.timeScale = 1;
        FourthDimension.tSM = 2;
        timeMultiplierText.text = FourthDimension.tSM.ToString() + "x";
    }

    public void IterativeSpeed()
    {
        Time.timeScale = 1;
        if (FourthDimension.tSM == 0)
            FourthDimension.tSM = 1;
        FourthDimension.tSM *= 2;
        timeMultiplierText.text = FourthDimension.tSM.ToString() + "x";
    }

    private void ChangeDayText()
    {
         dayText.text = FourthDimension.currentDay + "";
    }

    private void ChangeDayNightIcon(FourthDimension.TimeOfDay timeofDay)
    {
        if (timeofDay == FourthDimension.TimeOfDay.Day)
            dayNightIcon.texture = dayTexture;
        else
            dayNightIcon.texture = nightTexture;
    }

   
    //private IEnumerator ChangeAirStat()
    //{
    //    while (true)
    //    {
    //        carbondioxideText.text = air.carbondioxide.ToString("F3") + "%";
    //        oxygenText.text = air.oxygen.ToString("F1") +  "%";
    //        nitrogenText.text = air.nitrogen.ToString("F2") + "%";
    //        yield return new WaitForSeconds(0.5f);
    //    }
    //}
}
