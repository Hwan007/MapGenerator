using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainCondition : ScriptableObject {
    public abstract void SetTerrainCode(eTerrainType code, float[,] heightMap, ref byte[,] terrainMap);
}

