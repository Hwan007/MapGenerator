using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class HeightCondition : TerrainCondition {
    public float fromHeight;
    public float toHeight;
    public byte[] allowedCode;

    public override byte[,] SetTerrainCode(byte code, float[,] heightMap, byte[,] terrainMap) {
        int height = heightMap.GetLength(1);
        int width = heightMap.GetLength(0);
        byte[,] map = new byte[width, height];
        // TODO : make decesion that place is suitable for the terrain code
        for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
                float altitude = heightMap[x, y];
                if (allowedCode.Contains(terrainMap[x, y]) && altitude <= toHeight && altitude >= fromHeight) {
                    map[x, y] = code;
                }
                else
                    map[x, y] = terrainMap[x, y];
            }
        }
        return map;
    }
}