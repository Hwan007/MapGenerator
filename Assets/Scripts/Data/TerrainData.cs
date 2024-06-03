using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MapGenerator {
    [CreateAssetMenu()]
    public class TerrainData : UpdatableData {
        [Range(0, 6)]
        public int levelOfDetail = 1;
        public float baseXZLength = 1f;
        public float heightMultiplier;
        public TerrainSetting[] terrainSettings;

        public byte[,] GenerateTerrainCode(float[,] heightMap) {
            byte[,] map = new byte[heightMap.GetLength(0), heightMap.GetLength(1)];
            foreach (var terrain in terrainSettings) {
                terrain.condition?.SetTerrainCode(terrain.terrainCode, heightMap, ref map);
            }
            return map;
        }
    }

    [System.Serializable]
    public struct TerrainSetting {
        public eTerrainType terrainCode;
        public Color fallbackColor;
        public TerrainCondition condition;
    }


    [CustomEditor(typeof(TerrainData))]
    public class CustomEditorTerrainData : CustomEditorUpdatableData {

    }
}