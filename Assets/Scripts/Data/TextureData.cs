using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MapGenerator {
    [CreateAssetMenu()]
    public class TextureData : UpdatableData {
        public float mixScale;
        public float mixPower;
        public List<Texture2D> texture2Ds;

        public void ApplyToMaterial(Material material, byte[,] map) {
            UpdateMapTexture(material, map);
            UpdateMapMixing(material, mixScale, mixPower);
        }

        public void UpdateMapTexture(Material material, byte[,] map) {
            Texture2D convertToTextureLeft = CodeMapToTexture(map, eTerrainType.Snow, eTerrainType.Lava, eTerrainType.Dirt, eTerrainType.Normal);
            Texture2D convertToTextureRight = CodeMapToTexture(map, eTerrainType.Mountain, eTerrainType.Heel, eTerrainType.Ground, eTerrainType.Ocean);
            material.SetTexture("_Map0", convertToTextureLeft);
            material.SetTexture("_Map1", convertToTextureRight);

            string texName = "_MainTex";
            for (int i = 0; i < texture2Ds.Count; ++i) {
                material.SetTexture(texName + i.ToString(), texture2Ds[i]);
            }
        }

        public void UpdateMapMixing(Material material, float mixScale, float mixPower) {
            material.SetFloat("mixScale", mixScale);
            material.SetFloat("mixPower", mixPower);
        }

        Texture2D CodeMapToTexture(byte[,] map, eTerrainType type0, eTerrainType type1, eTerrainType type2, eTerrainType type3) {
            int height = map.GetLength(1) * 2 - 1;
            int width = map.GetLength(0) * 2 - 1;
            Color[] convertToColor = new Color[width * height];
            Texture2D convertToTexture = new Texture2D(width, height);

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    byte even = map[x / 2, y / 2];
                    byte odd = map[x / 2 + x % 2, y / 2 + y % 2];
                    float r = ((even == (byte)type0) ? 0.5f : 0) + ((odd == (byte)type0) ? 0.5f : 0);
                    float g = ((even == (byte)type1) ? 0.5f : 0) + ((odd == (byte)type1) ? 0.5f : 0);
                    float b = ((even == (byte)type2) ? 0.5f : 0) + ((odd == (byte)type2) ? 0.5f : 0);
                    float a = ((even == (byte)type3) ? 0.5f : 0) + ((odd == (byte)type3) ? 0.5f : 0);
                    convertToColor[width * y + x] = new Color(r, g, b, a);
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