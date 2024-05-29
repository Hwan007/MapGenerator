using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGenerator {
    [CreateAssetMenu()]
    public class TextureData : UpdatableData {
        public float mixScale;
        public List<Texture2D> texture2Ds;

        public void ApplyToMaterial(Material material, byte[,] map) {
            UpdateMapTexture(material, map);
            UpdateMapMixing(material, mixScale);
        }

        public void UpdateMapTexture(Material material, byte[,] map) {
            Texture2D convertToTextureLeft = CodeMapToTexture(map, eTerrainType.Snow, eTerrainType.Lava, eTerrainType.Dirt, eTerrainType.Normal);
            Texture2D convertToTextureRight = CodeMapToTexture(map, eTerrainType.Mountain, eTerrainType.Heel, eTerrainType.Ground, eTerrainType.Ocean);
            material.SetTexture("_Map0", convertToTextureLeft);
            material.SetTexture("_Map1", convertToTextureRight);

            string texName = "_MainTex";
            for (int i = 0; i <  texture2Ds.Count; ++i) {
                material.SetTexture(texName+i.ToString(), texture2Ds[i]);
            }
        }

        public void UpdateMapMixing(Material material, float mix) {
            material.SetFloat("mixScale", mix);
        }

        Texture2D CodeMapToTexture(byte[,] map, eTerrainType type0, eTerrainType type1, eTerrainType type2, eTerrainType type3) {
            int height = map.GetLength(1);
            int width = map.GetLength(0);
            Color[] convertToColor = new Color[width * height];
            Texture2D convertToTexture = new Texture2D(width, height);

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    convertToColor[width * y + x] = new Color(
                        (map[x, y] & (int)type0) != 0 ? 1 : 0,
                        (map[x, y] & (int)type1) != 0 ? 1 : 0,
                        (map[x, y] & (int)type2) != 0 ? 1 : 0,
                        (map[x, y] & (int)type3) != 0 ? 1 : 0);
                }
            }

            convertToTexture.filterMode = FilterMode.Point;
            convertToTexture.wrapMode = TextureWrapMode.Clamp;
            convertToTexture.SetPixels(convertToColor);
            convertToTexture.Apply();

            return convertToTexture;
        }

        protected void OnValidate() {
            if (texture2Ds.Count < 8) {
                while (texture2Ds.Count != 8) {
                    texture2Ds.Add(null);
                }
            }
            else if (texture2Ds.Count > 8) {
                while (texture2Ds.Count != 8) {
                    texture2Ds.RemoveAt(texture2Ds.Count - 1);
                }
            }
        }
    }

    [CustomEditor(typeof(TextureData))]
    public class CustomEditorTextureData : CustomEditorUpdatableData {

    }
}