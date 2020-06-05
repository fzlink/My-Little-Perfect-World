using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Climate: MonoBehaviour
{
    public enum Season
    {
        WINTER = 0,
        SPRING = 1,
        SUMMER = 2,
        FALL = 3
    }
    public static Season season;
    public static float temperature;
    private float tempdeb;

    [SerializeField] private ClimateProperties properties;
    [SerializeField] private int dayInSeason;
    [SerializeField] private ParticleSystem rain;


    private int rainIndex;

    private FourthDimension time;
    float[] nums;
    float[] rainTimes;
    float rainFallDuration;
    private float rainWaitTimer;
    private float rainFallTimer;
    private bool isRaining;

    private float changeColorTimer;
    private bool onChangeColor;
    private Color seasonColor;

    public event Action onRainStart;
    public event Action onRainStop;

    public event Action<Season> onSeasonChange;

    private void Awake()
    {
        time = SimulationManger.instance.timeManager;
    }

    private void Start()
    {
        time.onPassDay += OnPassDay;
        season = (Season)UnityEngine.Random.Range(0, 4);
        ChangeSeason();
        SetTemperature();
        tempdeb = temperature;
        CalculateRain();
    }

    private void Update()
    {
        if(rainIndex < nums.Length)
        {
            rainWaitTimer += Time.deltaTime * FourthDimension.tSM;
            if (rainWaitTimer >= rainTimes[rainIndex])
            {
                rainIndex++;
                StartRain();
            }
        }


        if (isRaining)
        {
            rainFallTimer += Time.deltaTime * FourthDimension.tSM;
            if(rainFallTimer >= rainFallDuration)
            {
                StopRain();
                rainFallTimer = 0;
                if(rainIndex < nums.Length)
                    rainFallDuration = (nums[rainIndex] / 10f);
            }
        }

        if (onChangeColor)
        {
            changeColorTimer += Time.deltaTime * FourthDimension.tSM;
            if(changeColorTimer >= 1)
            {
                changeColorTimer = 0;
                onChangeColor = false;
            }
            else
            {
                RenderSettings.ambientLight = Color.Lerp(RenderSettings.ambientLight, seasonColor, Time.deltaTime * FourthDimension.tSM);
            }

        }
    }

    private void SetTemperature()
    {
        onChangeColor = true;
        if (season == Season.WINTER)
        {
            seasonColor = Color.blue/4;
            temperature = properties.WinterTemperature;
        }
        else if (season == Season.SPRING)
        {
            seasonColor = Color.green/4;
            temperature = properties.SpringTemperature;
        }
        else if (season == Season.SUMMER)
        {
            seasonColor = Color.red/4;
            temperature = properties.SummerTemperature;
        }
        else
        {
            seasonColor = Color.gray/4;
            temperature = properties.FallTemperature;
        }
    }

    private void CalculateRain()
    {
        float totalSeasonRainAmount;
        if (season == Season.WINTER) totalSeasonRainAmount = properties.WinterRainAmount;
        else if (season == Season.SPRING) totalSeasonRainAmount = properties.SpringRainAmount;
        else if (season == Season.SUMMER) totalSeasonRainAmount = properties.SummerRainAmount;
        else totalSeasonRainAmount = properties.FallRainAmount;

        nums = new float[UnityEngine.Random.Range(3, 5)];

        for (int i = 0; i < nums.Length; i++)
        {
            nums[i] = UnityEngine.Random.Range(0, totalSeasonRainAmount/2);
            totalSeasonRainAmount -= nums[i];
        }
        for (int i = 0; i < nums.Length; i++)
        {
            nums[i] += totalSeasonRainAmount / nums.Length;
            print(nums[i]);
        }
        CalculateRainWaits();
    }

    private void CalculateRainWaits()
    {
        rainTimes = new float[nums.Length];
        //float prevTimes = 0;
        float max = FourthDimension.fCT * dayInSeason;
        for (int i = 0; i < nums.Length; i++)
        {
            rainTimes[i] = UnityEngine.Random.Range(10f, max - 10f);
        }
        rainTimes = rainTimes.OrderBy(x => x).ToArray();

        for (int i = 0; i < rainTimes.Length; i++)
        {
            if(i % 2 == 1 && i != rainTimes.Length - 1)
            {
                rainTimes[i] = (rainTimes[i - 1] + rainTimes[i + 1]) / 2;
            }
        }

        rainIndex = 0;
        rainFallDuration = (nums[0] / 10f);
        //for (int i = 0; i < rainTimes.Length && i < nums.Length; i++)
        //{
        //    rainDuration = (nums[i] / 10f) / FourthDimension.tSM;
        //    yield return new WaitForSeconds((rainTimes[i] - prevTimes) / FourthDimension.tSM);
        //    StartRain();
        //    prevTimes += rainTimes[i];
        //}

        //float max = time.fullCycleTime * (dayInSeason/2f)/time.timeSpeedMultiplier;
        //for(int i = 0; i < nums.Length; i++)
        //{
        //    rainDuration = nums[i] / 10f;
        //    float randomWait = UnityEngine.Random.Range(rainDuration, max);
        //    print(randomWait);
        //    max -= randomWait;
        //    if (i+1 == nums.Length / 2)
        //    {
        //        max = time.fullCycleTime * (dayInSeason / 2f) / time.timeSpeedMultiplier;
        //    }
        //    yield return new WaitForSeconds(randomWait);
        //    StartRain();
        //}
    }

    private void StartRain()
    {
        isRaining = true;
        print("Raining");
        rain.Play();
        if(onRainStart != null)
        {
            onRainStart();
        }

    }

    private void StopRain()
    {
        isRaining = false;
        rain.Stop();
        if(onRainStop != null)
        {
            onRainStop();
        }
    }

    private void OnPassDay()
    {
        if(FourthDimension.currentDay % dayInSeason == 0)
        {
            season++;
            if ((int)season >= 4)
                season = 0;
            ChangeSeason();
            SetTemperature();
            CalculateRain();
        }
    }

    private void ChangeSeason()
    {
        rainFallTimer = 0;
        if (onSeasonChange != null)
        {
            onSeasonChange(season);
        }
    }

    private void OnDestroy()
    {
        time.onPassDay -= OnPassDay;
    }

}

