using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public class MapDisplay : MonoBehaviour {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void Draw(eDrawMode mode, MapData map, Material material, TerrainData terrainData = null, TextureData textureData = null) {
            meshRenderer.sharedMaterial = material;
            switch (mode) {
                case eDrawMode.NoiseMap:
                    DrawPlaneTexture(TextureGenerator.TextureFromHeightMap(map.heightMap));
                    break;
                case eDrawMode.ColorMap:
                    DrawPlaneTexture(TextureGenerator.TextureFromColorMap(map.colorMap, map.heightMap.GetLength(0), map.heightMap.GetLength(1)));
                    break;
                case eDrawMode.TextureMesh:
                    DrawMesh(MeshGenerator.GenerateMesh(map.heightMap, terrainData.levelOfDetail, terrainData.heightMultiplier, terrainData.baseXZLength));
                    textureData.ApplyToMaterial(material, map.codeMap);
                    break;
            }
        }

        void DrawPlaneTexture(Texture2D texture) {
            meshRenderer.sharedMaterial.mainTexture = texture;
            meshFilter.sharedMesh = MeshData.PlaneMesh(241, 241).CreateMesh();
        }

        void DrawMesh(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh();
        }

        public MapDisplay Init() {
            if (!TryGetComponent<MeshFilter>(out meshFilter)) {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }
            if (!TryGetComponent<MeshRenderer>(out meshRenderer)) {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
            return this;
        }
    }
}