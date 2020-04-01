using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    public enum Sex { Male = 0, Female = 1};
    public Sex sex { get; set; }
    public float skinIlluminance { get; set; }
    public Color skinColor { get; set; }

    public DNA(Color commonSkinColor) //With no ancestor
    {
        DetermineSex();

        skinIlluminance = Random.Range(0.50f, 1.50f);
        DetermineSkinIlluminance(commonSkinColor, skinIlluminance);

    }


    public DNA(Color commonSkinColor, float skinIlluminance) // With ancestor
    {
        DetermineSex();

        this.skinIlluminance = skinIlluminance;
        DetermineSkinIlluminance(commonSkinColor, skinIlluminance);
    }


    private void DetermineSex()
    {
        if (Random.value <= 0.5f) { sex = Sex.Male; }
        else { sex = Sex.Female; }
    }

    private void DetermineSkinIlluminance(Color commonSkinColor, float skinIlluminance)
    {
        Color newColor = commonSkinColor * skinIlluminance;
        newColor.a = 1f;
        skinColor = newColor;
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
