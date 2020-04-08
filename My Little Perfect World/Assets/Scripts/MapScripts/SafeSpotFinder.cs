using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeSpotFinder : MonoBehaviour
{
    private const int chunkCoordInc = 98;
    private const int EdgeOffset = 3;
    private const int groundWaterMask = (1 << 4) | (1 << 8);

    public List<Vector3> safeSpots = new List<Vector3>();
    public Transform mapChunks;
    public int spotPerChunk;

    public event System.Action<List<Vector3>> onSafeSpotsFounded;

    private void Start()
    {
        MapEventManager.instance.onTerrainGenerationFinished += FindSafeSpots;
    }

    public void FindSafeSpots()
    {
        Vector3 center;
        RaycastHit hit;
        float xRange;
        float yRange;
        for (int i = 0; i < mapChunks.childCount; i++)
        {
            center = mapChunks.GetChild(i).transform.position;
            for (int j = 0; j < spotPerChunk; j++)
            {
                xRange = UnityEngine.Random.Range(center.x - chunkCoordInc / 2 + EdgeOffset, center.x + chunkCoordInc / 2 - EdgeOffset);
                yRange = UnityEngine.Random.Range(center.z - chunkCoordInc / 2 + EdgeOffset, center.z + chunkCoordInc / 2 - EdgeOffset);

                if (Physics.Raycast(new Vector3(xRange, 100, yRange), Vector3.down, out hit, 100, groundWaterMask))
                {
                    if (hit.collider.gameObject.layer == 8 && Vector3.Angle(Vector3.up,hit.normal) < 25f)
                    {
                        safeSpots.Add(hit.point);
                        //Debug.Log("Found Spot" + hit.point);
                    }
                }
            }
        }

        MapEventManager.instance.onTerrainGenerationFinished -= FindSafeSpots;
        if(onSafeSpotsFounded != null)
        {
            onSafeSpotsFounded(safeSpots);
        }
    }


}
