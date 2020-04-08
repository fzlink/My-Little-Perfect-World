using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : ScaleManager
{
    protected override void Scale()
    {
        transform.localScale = new Vector3 ((chunkCoordInc * xOffset) + (chunkCoordInc / 2), 50f, (chunkCoordInc * yOffset) + (chunkCoordInc / 2));
    }
}
