using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace MapGenerator {
    public enum eDrawMode {
        NoiseMap,
        ColorMap,
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
        public static int mapChunkSize = 241;

        public eDrawMode drawMode;
        public bool autoUpdate;
        [Range(1, 5)] public int numberOfMesh;
        public NoiseData noiseData;
        public TextureData textureData;
        public TerrainData terrainData;
        public Material terrainMaterial;
        public Material defaultMaterial;
        public MapDisplay mapPrefab;
        public List<MapDisplay> deployedMeshList;

        public void GenerateMap() {
            List<MapData> mapList = new List<MapData>();
            for (int y = 0; y < numberOfMesh; ++y) {
                for (int x = 0; x < numberOfMesh; ++x) {
                    mapList.Add(GenerateMapData(noiseData.Offset(new Vector2(mapChunkSize * x, mapChunkSize * y)), terrainData));
                }
            }

            while (deployedMeshList.Count > numberOfMesh * numberOfMesh) {
                DestroyImmediate(deployedMeshList[deployedMeshList.Count-1]);
                deployedMeshList.RemoveAt(deployedMeshList.Count-1);
            }
            while (deployedMeshList.Count < numberOfMesh * numberOfMesh) {
                deployedMeshList.Add(Instantiate(mapPrefab).Init());
            }

            Material targetMat = drawMode == eDrawMode.TextureMesh ? terrainMaterial : defaultMaterial;
            for (int y = 0; y < numberOfMesh; ++y) {
                for (int x = 0; x < numberOfMesh; ++x) {
                    deployedMeshList[y * numberOfMesh + x].Draw(drawMode, mapList[y * numberOfMesh + x], targetMat, terrainData, textureData);
                    deployedMeshList[y * numberOfMesh + x].transform.position = new Vector3(x * terrainData.baseXZLength * mapChunkSize, 0, y * terrainData.baseXZLength * mapChunkSize);
                }
            }
        }

        MapData GenerateMapData(NoiseData noiseData, TerrainData terrainData) {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, noiseData.noiseScale, noiseData.offset, noiseData.seed, noiseData.settings, noiseData.normalizeMode, noiseData.heightMultiplierCurve);
            if (noiseData.useCircle)
                noiseMap = Noise.EditHeightMapWithCircle(noiseMap, noiseData.gradient, noiseData.gradientRate);
            if (noiseData.useShape)
                noiseMap = Noise.EditHeightMapWithTexture2D(noiseMap, noiseData.desireShape);

            byte[,] codeMap = GenerateTerrainMap(noiseMap, terrainData);
            Color[] colorMap = GenerateColorMap(codeMap, terrainData);

            return new MapData(noiseMap, colorMap, codeMap);
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

        private void OnValidate() {
            if (noiseData != null) {
                noiseData.OnValuesUpdate -= GenerateMap;
                noiseData.OnValuesUpdate += GenerateMap;
            }
            if (terrainData != null) {
                terrainData.OnValuesUpdate -= GenerateMap;
                terrainData.OnValuesUpdate += GenerateMap;
            }
            if (textureData != null) {
                textureData.OnValuesUpdate -= GenerateMap;
                textureData.OnValuesUpdate += GenerateMap;
            }
        }
    }

    [System.Serializable]
    public struct MapData {
        public float[,] heightMap;
        public Color[] colorMap;
        public byte[,] codeMap;

        public MapData(float[,] heightMap, Color[] colorMap, byte[,] codeMap) {
            this.colorMap = colorMap;
            this.heightMap = heightMap;
            this.codeMap = codeMap;
        }
    }

    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor {

        public override void OnInspectorGUI() {
            MapGenerator mapGen = (MapGenerator)target;

            if (DrawDefaultInspector()) {
                if (mapGen.autoUpdate) {
                    mapGen.GenerateMap();
                    EditorUtility.SetDirty(mapGen);
                }
            }

            if (GUILayout.Button("Generate")) {
                mapGen.GenerateMap();
                EditorUtility.SetDirty(mapGen);
            }
        }
    }
}