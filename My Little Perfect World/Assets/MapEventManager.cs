﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEventManager : MonoBehaviour
{
    TerrainGenerator terrainGenerator;

    private int mapChunkNum;
    private int registeredChunkNum;
    private const int passCount = 2;
    public Dictionary<Vector2, int> registeredChunks = new Dictionary<Vector2, int>();

    public static MapEventManager instance;

        public event Action onTerrainGenerationFinished;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        mapChunkNum = terrainGenerator.MapChunkCount;
    }


    public void RegisterChunk(Vector2 chunkCoord)
    {
        if (!registeredChunks.ContainsKey(chunkCoord))
        {
            registeredChunks.Add(chunkCoord, 1);
        }
        else
        {
            registeredChunks[chunkCoord]++;
        }

        if (registeredChunks[chunkCoord] == passCount)
        {
            registeredChunkNum++;
            if(registeredChunkNum == mapChunkNum)
            {
                if(onTerrainGenerationFinished != null)
                {
                    onTerrainGenerationFinished();
                }
            }
        }
    }

}
