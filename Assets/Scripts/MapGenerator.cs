using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapGenerator {
    public enum eDrawMode {
        NoiseMap,
        ColorMap,
        ColorMesh,
        TextureMesh,
    }
    public enum eTerrainType {
        Ocean = 1,
        Ground = 2,
        Heel = 4,
        Mountain = 8,
        Normal = 16,
        Dirt = 32,
        Lava = 64,
        Snow = 128,
    }

    public class MapGenerator : MonoBehaviour {
        public eDrawMode drawMode;
        public bool autoUpdate;
        const int mapChunkSize = 241;

        public NoiseData noiseData;
        public TextureData textureData;
        public TerrainData terrainData;
        public Material colorMaterial;
        public Material terrainMaterial;

        [Header("확인용")]
        public List<MeshObject> meshObjs;

        public void DrawMapInEditor() {
            MapData map = GenerateMapData(noiseData, terrainData);
            // TODO : change display to Pooling object;
            MapDisplay display = FindObjectOfType<MapDisplay>();
            switch (drawMode) {
                case eDrawMode.NoiseMap:
                    display.DrawTexture(TextureGenerator.TextureFromHeightMap(map.heightMap));
                    break;
                case eDrawMode.ColorMap:
                    display.DrawTexture(TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize));
                    break;
                case eDrawMode.ColorMesh:
                    display.DrawMesh(MeshGenerator.GenerateMesh(map.heightMap, terrainData.levelOfDetail, terrainData.heightMultiplier, terrainData.baseXZLength), TextureGenerator.TextureFromColorMap(map.colorMap, mapChunkSize, mapChunkSize), colorMaterial);
                    break;
                case eDrawMode.TextureMesh:
                    // TODO 
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
            Color[] colorMap = GenerateColorMap(terrainMap, terrainData);

            return new MapData(noiseMap, colorMap, terrainMap);
        }

        Color[] GenerateColorMap(byte[,] terrainMap, TerrainData terrainData) {
            int height = terrainMap.GetLength(1);
            int width = terrainMap.GetLength(0);
            Color[] map = new Color[width * height];
            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    map[y * width + x] = terrainData.terrainSettings[terrainMap[x, y]].fallbackColor;
                }
            }
            return map;
        }

        byte[,] GenerateTerrainMap(float[,] heightMap, TerrainData terrainData) {
            return terrainData.GenerateTerrainCode(heightMap);
        }

        void OnValuesUpdate() {
            if (!Application.isPlaying) {
                DrawMapInEditor();
            }
        }

        void OnTextureValuesUpdate() {
            textureData.ApplyToMaterial(terrainMaterial);
        }


        private void OnValidate() {
            if (terrainData != null) {
                terrainData.OnValuesUpdate -= OnValuesUpdate;
                terrainData.OnValuesUpdate += OnValuesUpdate;
            }
            if (noiseData != null) {
                noiseData.OnValuesUpdate -= OnValuesUpdate;
                noiseData.OnValuesUpdate += OnValuesUpdate;
            }
            if (textureData != null) {
                textureData.OnValuesUpdate -= OnTextureValuesUpdate;
                textureData.OnValuesUpdate += OnTextureValuesUpdate;
            }
        }
    }

    [System.Serializable]
    public struct MapData {
        public float[,] heightMap;
        public Color[] colorMap;
        public byte[,] terrainMap;

        public MapData(float[,] heightMap, Color[] colorMap, byte[,] terrainMap) {
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