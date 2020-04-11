using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CreatureData : ScriptableObject
{
    //[System.Serializable]
    //public class CreatureList
    //{
    //    public Creature[] creatureList;
    //}

    public List<Creature> creatures;
    public List<byte> creatureFoodChainIndices;
    //public CreatureList[] creaturesEat;



}
