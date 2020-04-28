using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SoilUIController : MonoBehaviour
{

    private TMP_Text soilWaterText;
    private Soil soil;

    private void Awake()
    {
        soil = FindObjectOfType<Soil>();
        soilWaterText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        soilWaterText.text = "Soil Water: " + soil.water.ToString("F2");
    }
}
