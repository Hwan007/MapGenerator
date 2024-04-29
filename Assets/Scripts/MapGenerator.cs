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

    public class MapGenerator : MonoBehaviour {
        public eDrawMode drawMode;

        const int mapChunkSize = 241;
        [Range(0, 6)]
        public int levelOfDetail = 1;
        public float baseXZLength = 1f;
        public float noiseScale;
        public Vector2 offset;
        public Noise.eNormalizeMode normalizeMode;
        public NoiseMapSetting[] settings;
        public int seed;

        public float heightMultiplier;
        public AnimationCurve heightMultiplierCurve;

        public bool useShape;
        public Texture2D desireShape;

        public bool useCircle;
        public float gradient;
        public float gradientRate;

        public bool autoUpdate;

        public TerrainType[] regions;

        [Header("È®ÀÎ¿ë")]
        public List<MeshObject> meshObjs;

        public void DrawMapInEditor() {
            MapData map = GenerateMapData();

            MapDisplay display = FindObjectOfType<MapDisplay>();
            if (drawMode == eDrawMode.NoiseMap) {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(map.heightMap));
            }
            else if (drawMode == eDrawMode.ColorMap) {
                display.DrawTexture(TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize));
            }
            else if (drawMode == eDrawMode.MeshMap) {
                display.DrawMesh(MeshGenerator.GenerateMesh(map.heightMap, levelOfDetail, heightMultiplier, baseXZLength), TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize));
            }
        }

        MapData GenerateMapData() {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseScale, offset, seed, settings, normalizeMode, heightMultiplierCurve);
            if (useCircle)
                noiseMap = Noise.EditHeightMapWithCircle(noiseMap, gradient, gradientRate);
            if (useShape)
                noiseMap = Noise.EditHeightMapWithTexture2D(noiseMap, desireShape);

            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            for (int y = 0; y < mapChunkSize; y++) {
                for (int x = 0; x < mapChunkSize; x++) {
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i++) {
                        if (currentHeight <= regions[i].height) {
                            colorMap[y * mapChunkSize + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            return new MapData(noiseMap, colorMap);
            
            //switch (drawMode) {
            //    case eDrawMode.NoiseMap:
            //        break;
            //    case eDrawMode.ColorMap:
            //        break;
            //    case eDrawMode.MeshMap:
            //        break;
            //}
        }
    }

    [System.Serializable]
    public struct TerrainType {
        public string name;
        public float height;
        public Color color;
    }

    [System.Serializable]
    public struct MapData {
        public float[,] heightMap;
        public Color[] colorMap;

        public MapData(float[,] heightMap, Color[] colorMap) {
            this.colorMap = colorMap;
            this.heightMap = heightMap;
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