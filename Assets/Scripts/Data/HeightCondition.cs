using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class HeightCondition : TerrainCondition {
    public float fromHeight;
    public float toHeight;
    public eTerrainType[] allowedCode;

    public override void SetTerrainCode(eTerrainType code, float[,] heightMap, ref byte[,] terrainMap) {
        int height = heightMap.GetLength(1);
        int width = heightMap.GetLength(0);
        for (int y = 0; y < height; ++y) {
            for (int x = 0; x < width; ++x) {
                float altitude = heightMap[x, y];
                eTerrainType codeTerrain = (eTerrainType)terrainMap[x, y];
                if (allowedCode.Contains(codeTerrain) && altitude < toHeight && altitude > fromHeight) {
                    terrainMap[x, y] = (byte)code;
                }
                //else
                //    terrainMap[x, y] = terrainMap[x, y];
            }
        }
    }
}