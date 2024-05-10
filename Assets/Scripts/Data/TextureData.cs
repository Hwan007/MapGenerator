using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu()]
public class TextureData : UpdatableData {
    byte[,] codeMap;

    public void ApplyToMaterial(Material material) {
        UpdateMapTexture(material, codeMap);
    }

    public void UpdateMapTexture(Material material, byte[,] map) {
        codeMap = map;

        Texture2D convertToTextureLeft = CodeMapToTexture(map, eTerrainType.Snow, eTerrainType.Lava, eTerrainType.Dirt, eTerrainType.Normal);
        Texture2D convertToTextureRight = CodeMapToTexture(map, eTerrainType.Mountain, eTerrainType.Heel, eTerrainType.Ground, eTerrainType.Ocean);
        material.SetTexture("_Map0", convertToTextureLeft);
        material.SetTexture("_Map1", convertToTextureRight);
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
}
