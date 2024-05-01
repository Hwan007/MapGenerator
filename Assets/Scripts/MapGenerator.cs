using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapGenerator {
    public enum eDrawMode {
        NoiseMap,
        ColorMap,
        MeshMap,
        TerrainMap,
    }
    public enum eTerrainType {
        Ocean,
        Beach,
        Grass,
        Forest,
        Dirt,
        Rocky,
        Lava,
        Snow,
        Frozen,
    }

    public class MapGenerator : MonoBehaviour {
        public eDrawMode drawMode;

        const int mapChunkSize = 241;
        [Range(0, 6)]
        public int levelOfDetail = 1;
        public float baseXZLength = 1f;

        public NoiseData noiseData;

        public bool autoUpdate;
        public float heightMultiplier;
        public TerrainData regions;

        [Header("È®ÀÎ¿ë")]
        public List<MeshObject> meshObjs;

        public void DrawMapInEditor() {
            MapData map = GenerateMapData(noiseData, regions);

            MapDisplay display = FindObjectOfType<MapDisplay>();
            switch (drawMode) {
                case eDrawMode.NoiseMap:
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(map.heightMap));
                    break;
                case eDrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize));
                    break;
                case eDrawMode.MeshMap:
                    display.DrawMesh(MeshGenerator.GenerateMesh(map.heightMap, levelOfDetail, heightMultiplier, baseXZLength), TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize));
                    break;
            }
        }

        MapData GenerateMapData(NoiseData noiseData, TerrainData terrainData) {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.noiseScale, noiseData.offset, noiseData.seed, noiseData.settings, noiseData.normalizeMode, noiseData.heightMultiplierCurve);
            if (noiseData.useCircle)
                noiseMap = Noise.EditHeightMapWithCircle(noiseMap, noiseData.gradient, noiseData.gradientRate);
            if (noiseData.useShape)
                noiseMap = Noise.EditHeightMapWithTexture2D(noiseMap, noiseData.desireShape);

            byte[,] terrainMap = GenerateTerrainMap(noiseMap, terrainData);
            Color[] colorMap = GenerateColorMap(terrainMap);

            return new MapData(noiseMap, colorMap, terrainMap);
        }

        Color[] GenerateColorMap(byte[,] terrainMap) {
            int height = terrainMap.GetLength(1);
            int width = terrainMap.GetLength(0);
            Color[] map = new Color[width * height];
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    float gray = terrainMap[x, y] / 8f;
                    map[y * width + x] = new Color(gray, gray, gray);
                }
            }
            return map;
        }

        byte[,] GenerateTerrainMap(float[,] heightMap, TerrainData terrainData) {
            return terrainData.GenerateTerrainCode(heightMap);;
        }
    }

    [System.Serializable]
    public struct MapData {
        public float[,] heightMap;
        public Color[] colorMap;
        public byte[,] terrainMap;

        public MapData(float[,] heightMap, Color[] colorMap, byte[,] terrainMap){
            this.colorMap = colorMap;
            this.heightMap = heightMap;
            this.terrainMap = terrainMap;
        }
    }

    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor {

        public override void OnInspectorGUI() {
            MapGenerator mapGen = (MapGenerator)target;

            if (DrawDefaultInspector()) {
                if (mapGen.autoUpdate) {
                    mapGen.DrawMapInEditor();
                }
            }

            if (GUILayout.Button("Generate")) {
                mapGen.DrawMapInEditor();
            }
        }
    }
}