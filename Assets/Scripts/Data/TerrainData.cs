using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData {
    public TerrainSetting[] terrainSettings;

    public byte[,] GenerateTerrainCode(float[,] heightMap) {
        byte[,] map = new byte[heightMap.GetLength(0), heightMap.GetLength(1)];
        foreach (var terrain in terrainSettings) {
            terrain.condition?.SetTerrainCode(heightMap, map);
        }
        return map;
    }
}

[System.Serializable]
public struct TerrainSetting {
    public byte terrainCode;
    public Texture2D texture;
    public iTerrainCondition condition;
}

public interface iTerrainCondition {
    public byte[,] SetTerrainCode(float[,] heightMap, byte[,] terrainMap);
}