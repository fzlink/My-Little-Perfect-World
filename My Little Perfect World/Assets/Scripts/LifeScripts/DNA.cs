using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Sex
{
    Male = 0,
    Female = 1
}

public class DNA
{
    public Sex sex { get; set; }
    public float skinIlluminance { get; set; }
    public Color skinColor { get; set; }

    public DNA(Color skinColor,float skinIlluminance,Sex sex) //With no ancestor
    {
        this.skinColor = skinColor;
        this.skinIlluminance = skinIlluminance;
        this.sex = sex;
    }
    
    public bool isOppositeSex(Sex sex)
    {
        if(this.sex == sex) { return false; }
        else { return true; }
    }

    public static Sex GetOppositeSex(Sex sex)
    {
        if(sex == Sex.Female) { return Sex.Male; }
        else { return Sex.Female; }
    }
}
