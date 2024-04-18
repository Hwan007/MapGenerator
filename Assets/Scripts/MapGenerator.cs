using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings.SplashScreen;

public enum eDrawMode
{
    NoiseMap,
    ColorMap,
    MeshMap,
}

public class MapGenerator : MonoBehaviour
{
    public eDrawMode drawMode;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public Vector2 offset;

    public NoiseMapSetting[] settings;
    public int seed;

    public float heightMultiplier;
    public AnimationCurve heightMultiplierCurve;
    public Texture2D desireShape;
    public bool autoUpdate;

    public TerrainType[] regions;

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, offset, seed, settings);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length;  i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == eDrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == eDrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == eDrawMode.MeshMap)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, heightMultiplierCurve, desireShape), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    private void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}