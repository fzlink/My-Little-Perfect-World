using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{

    public CreatureData creatureData;


    private void Start()
    {
        BriefAnimals();
    }

    private void BriefAnimals()
    {
        for(int i = 0; i < creatureData.creatures.Count; i++)
        {
            string tag = creatureData.creatures[i].tag;
            GameObject[] creatures = GameObject.FindGameObjectsWithTag(tag);
            for (int j = 0; j < creatures.Length; j++)
            {
                if(creatures[j].GetComponent<Creature>() != null)
                {
                    creatures[j].GetComponent<Creature>().foodChainIndex = creatureData.creatureFoodChainIndices[i];
                }
            }
        }
    }
}
