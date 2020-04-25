using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Climate Properties", menuName = "Climate Properties", order = 0)]
public class ClimateProperties: Properties
{
    [Header("Rain Properties")]
    [SerializeField] private float winterRainAmount;
    [SerializeField] private float springRainAmount;
    [SerializeField] private float summerRainAmount;
    [SerializeField] private float fallRainAmount;
    [Range(0f,100f)][SerializeField] private float rainVolume;

    [Header("Temperature Properties")]
    [Tooltip("In Celcius")][Range(-88f, 58f)] [SerializeField] private float winterTemperature;
    [Tooltip("In Celcius")][Range(-88f, 58f)] [SerializeField] private float springTemperature;
    [Tooltip("In Celcius")][Range(-88f, 58f)] [SerializeField] private float summerTemperature;
    [Tooltip("In Celcius")][Range(-88f, 58f)] [SerializeField] private float fallTemperature;
    [Range(0f, 100f)] [SerializeField] private float temperatureVolume;

    public float WinterRainAmount { get => winterRainAmount; set => winterRainAmount = value; }
    public float SpringRainAmount { get => springRainAmount; set => springRainAmount = value; }
    public float SummerRainAmount { get => summerRainAmount; set => summerRainAmount = value; }
    public float FallRainAmount { get => fallRainAmount; set => fallRainAmount = value; }
    public float RainVolume { get => rainVolume; set => rainVolume = value; }

    public float WinterTemperature { get => winterTemperature; set => winterTemperature = value; }
    public float SpringTemperature { get => springTemperature; set => springTemperature = value; }
    public float SummerTemperature { get => summerTemperature; set => summerTemperature = value; }
    public float FallTemperature { get => fallTemperature; set => fallTemperature = value; }
}

