
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUIController : MonoBehaviour
{
    public Texture winterTexture;
    public Texture springTexture;
    public Texture summerTexture;
    public Texture fallTexture;

    private TMP_Text label;
    private RawImage icon;

    private void Awake()
    {
        label = GetComponentInChildren<TMP_Text>();
        icon = GetComponentInChildren<RawImage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Climate>().onSeasonChange += ChangeSeason;
    }

    private void ChangeSeason(Climate.Season season)
    {
        if (season == Climate.Season.WINTER)
        {
            icon.texture = winterTexture;
            label.text = "WINTER";
        }
        else if (season == Climate.Season.SPRING)
        {
            icon.texture = springTexture;
            label.text = "SPRING";
        }
        else if (season == Climate.Season.SUMMER)
        {
            icon.texture = summerTexture;
            label.text = "SUMMER";
        }
        else
        {
            icon.texture = fallTexture;
            label.text = "FALL";
        }
    }
}
