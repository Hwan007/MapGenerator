using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrainCondition : ScriptableObject {
    public abstract byte[,] SetTerrainCode(byte code, float[,] heightMap, byte[,] terrainMap);
}

