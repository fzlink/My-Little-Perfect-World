using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    private const int chunkCoordInc = 98;

    int xOffset;
    int yOffset;
    void Start()
    {
        TerrainGenerator tg = FindObjectOfType<TerrainGenerator>();
        xOffset =tg.mapChunkConstraintX - 1;
        yOffset = tg.mapChunkConstraintY - 1;
        PlaceWalls();
    }

    private void PlaceWalls()
    {
        transform.localScale =new Vector3 ((chunkCoordInc * xOffset) + (chunkCoordInc / 2), 50f, (chunkCoordInc * yOffset) + (chunkCoordInc / 2));
    }
}
