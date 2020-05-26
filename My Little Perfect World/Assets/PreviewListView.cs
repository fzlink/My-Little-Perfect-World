using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewListView : MonoBehaviour
{

    public List<PreviewProperties> itemProperties;
    public Button backButton;
    public TMP_Text sectionText;
    public Button nextButton;

    private List<string> sections;
    private int currentSectionID;

    public GameObject previewItemUIAsset;

    public Action<PreviewProperties,GameObject> OnItemSelected;

    private void Start()
    {
        Init();
        MakeSections();
        PopulateList();
    }

    private void Init()
    {
        sections = new List<string>();
        backButton.onClick.AddListener(() => BackButton());
        nextButton.onClick.AddListener(() => NextButton());
    }

    private void BackButton()
    {
        currentSectionID--;
        if (currentSectionID < 0)
            currentSectionID = sections.Count - 1;
        PopulateList();
    }

    private void NextButton()
    {
        currentSectionID++;
        if (currentSectionID >= sections.Count)
            currentSectionID = 0;
        PopulateList();
    }

    private void PopulateList()
    {
        sectionText.SetText(sections[currentSectionID]);
        ScrollRect scrollRect = GetComponent<ScrollRect>();
        RectTransform rectTransform = scrollRect.content.GetComponent<RectTransform>();
        foreach (Transform transform in rectTransform)
        {
            Destroy(transform.gameObject);
        }
        for (int i = 0; i < itemProperties.Count; i++)
        {
            if(itemProperties[i].TYPE.ToString() == sections[currentSectionID])
            {
                GameObject g = Instantiate(previewItemUIAsset, rectTransform);
                g.GetComponentInChildren<RawImage>().texture = itemProperties[i].Icon;
                g.GetComponentInChildren<TMP_Text>().text = itemProperties[i].Name;
                int n = i;
                GameObject item = g;
                g.GetComponentInChildren<Button>().onClick.AddListener(() => ItemClicked(n,item));
            }
        }

    }

    private void ItemClicked(int i, GameObject g)
    {
        if(OnItemSelected != null)
        {
            OnItemSelected(itemProperties[i],g);
        }
    }

    private void MakeSections()
    {
        for (int i = 0; i < itemProperties.Count; i++)
        {
            if (!sections.Contains(itemProperties[i].TYPE.ToString()))
            {
                sections.Add(itemProperties[i].TYPE.ToString());
            }
        }
    }
}