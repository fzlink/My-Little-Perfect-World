using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : ScaleManager
{

    protected override void Scale()
    {
        transform.localScale = new Vector3(((chunkCoordInc * xOffset) + (chunkCoordInc / 2)) *2f, ((chunkCoordInc * yOffset) + (chunkCoordInc / 2)) * 2f, 1f);
    }
}
