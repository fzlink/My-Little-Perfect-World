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

    [SerializeField] private ClimateProperties properties;
    [SerializeField] private int dayInSeason;
    [SerializeField] private GameObject rain;

    private Air air;
    private FourthDimension time;
    float[] nums;
    float rainDuration;

    public event Action onRainStart;
    public event Action onRainStop;

    private void Awake()
    {
        time = FindObjectOfType<FourthDimension>();
    }

    private void Start()
    {
        time.onPassDay += OnPassDay;

        season = (Season)UnityEngine.Random.Range(0, 4);
        CalculateRain();
    }

    private void CalculateRain()
    {
        float totalSeasonRainAmount;
        if (season == Season.WINTER) totalSeasonRainAmount = properties.WinterRainAmount;
        else if (season == Season.SPRING) totalSeasonRainAmount = properties.SpringRainAmount;
        else if (season == Season.SUMMER) totalSeasonRainAmount = properties.SummerRainAmount;
        else totalSeasonRainAmount = properties.FallRainAmount;

        nums = new float[UnityEngine.Random.Range(3, 5)];

        for (int i = 0; i < nums.Length - 1; i++)
        {
            nums[i] = UnityEngine.Random.Range(0, totalSeasonRainAmount);
            totalSeasonRainAmount -= nums[i];
        }
        nums[nums.Length - 1] = totalSeasonRainAmount;
        StartCoroutine(WaitRain());
    }

    private IEnumerator WaitRain()
    {
        float[] rainTimes = new float[nums.Length];
        float prevTimes = 0;
        float max = FourthDimension.fCT * dayInSeason;
        for (int i = 0; i < nums.Length; i++)
        {
            rainTimes[i] = UnityEngine.Random.Range(0f, max);
        }
        rainTimes = rainTimes.OrderBy(x => x).ToArray();

        for (int i = 0; i < rainTimes.Length; i++)
        {
            rainDuration = (nums[i] / 10f) / FourthDimension.tSM;
            yield return new WaitForSeconds((rainTimes[i] - prevTimes)/FourthDimension.tSM);
            StartRain();
            prevTimes += rainTimes[i];
        }

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
        print("Raining");
        rain.SetActive(true);
        StartCoroutine(StopRain());
        if(onRainStart != null)
        {
            onRainStart();
        }

    }

    private IEnumerator StopRain()
    {
        yield return new WaitForSeconds(rainDuration);
        rain.SetActive(false);
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
            CalculateRain();
        }
    }

    private void OnDestroy()
    {
        time.onPassDay -= OnPassDay;
    }

}

