using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class ObjectsToPlace
{
    [Range(0f, 1f)]
    public float magnitude;
    public GameObject obj;
    public Transform container;
}