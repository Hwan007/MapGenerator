using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainCondition : ScriptableObject {
    public abstract byte[,] SetTerrainCode(eTerrainType code, float[,] heightMap, byte[,] terrainMap);
}

