using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    [Range(0, 6)]
    public int levelOfDetail = 1;
    public float baseXZLength = 1f;
    public float heightMultiplier;
    public TerrainSetting[] terrainSettings;

    public byte[,] GenerateTerrainCode(float[,] heightMap)
    {
        byte[,] map = new byte[heightMap.GetLength(0), heightMap.GetLength(1)];
        foreach (var terrain in terrainSettings)
        {
            terrain.condition?.SetTerrainCode(terrain.terrainCode, heightMap, map);
        }
        return map;
    }
}

[System.Serializable]
public struct TerrainSetting
{
    [Range(0,255)]
    public byte terrainCode;
    public Texture2D texture;
    public Color fallbackColor;
    public TerrainCondition condition;
}
