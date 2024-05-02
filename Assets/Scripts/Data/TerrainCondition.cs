using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class heightCondition : TerrainCondition {
    public float limitHeight;
    public override byte[,] SetTerrainCode(byte code, float[,] heightMap, byte[,] terrainMap) {
        int height = heightMap.GetLength(1);
        int width = heightMap.GetLength(0);
        byte[,] map = new byte[width, height];
        // TODO : make decesion that place is suitable for the terrain code
        for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
                if (terrainMap[x, y] == 0 && heightMap[x, y] < limitHeight) {
                    map[x, y] = code;
                }
                else
                    map[x, y] = terrainMap[x, y];
            }
        }
        return map;
    }
}

public abstract class TerrainCondition : ScriptableObject {
    public abstract byte[,] SetTerrainCode(byte code, float[,] heightMap, byte[,] terrainMap);
}

