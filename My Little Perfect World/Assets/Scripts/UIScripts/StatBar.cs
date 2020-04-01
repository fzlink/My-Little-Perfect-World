using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatBar : MonoBehaviour
{
    public enum BarType { Food = 0, Water= 1, Sleep = 2, Stress = 3, Pregnancy = 4};
    public BarType barType;
    public GameObject parentCapsule;
    public Slider slider;
    public Image fill;

    public void ChangeInterest(float value, float maxValue, float dangerThreshold)
    {
        slider.maxValue = maxValue;
        ChangeValues(value, dangerThreshold);
    }

    public void ChangeValues(float value, float dangerThreshold)
    {
        slider.value = value;
        if (value < dangerThreshold)
        {
            fill.color = Color.red;
        }
        else
        {
            fill.color = Color.green;
        }
    }

}
