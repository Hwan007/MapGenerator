using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CalculatorFor2DMap {
    public static int FindEnclosedArea(float[,] heightMap, byte[,] terrainMap, byte[] conditionTerrainCode, out byte[,] area) {
        int height = heightMap.GetLength(1);
        int width = heightMap.GetLength(0);
        byte[,] map = new byte[width, height];
        // area는 해당하는 영역을 표시하는 맵
        area = new byte[width, height];
        // count 해당 영역의 개수
        int count = 0;

        // TODO : find area

        return count;
    }

}
