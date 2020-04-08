using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    public int chunkCoordInc { get; set; }

    public int xOffset { get; set; }
    public int yOffset { get; set; }

    void Start()
    {
        chunkCoordInc = MapEventManager.chunkCoordInc;
        xOffset = MapEventManager.instance.xOffset;
        yOffset = MapEventManager.instance.yOffset;
        Scale();
    }

    protected virtual void Scale()
    {

    }
}
