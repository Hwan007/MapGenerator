using System;
using System.Collections;
using UnityEngine;
namespace MapGenerator {
    public static class Noise {

        public enum eNormalizeMode { Local, Global }
        /// <summary>
        /// 주어진 설정값으로 PerlinNoise를 사용하여 Noise Map을 생성한다.
        /// </summary>
        /// <param name="mapWidth">가로</param>
        /// <param name="mapHeight">세로</param>
        /// <param name="scale">기본 PerlinNoise 비율 = 기본 NoiseMap 형상 비율</param>
        /// <param name="offset">기본 PerlinNoise offset = 기본 NoiseMap 형상 위치</param>
        /// <param name="overlapSeed">offset random seed로 동일한 맵이 나올 수 있게 만든다. = Perlin Noise에 넣을 좌표값을 동일하게 만든다.</param>
        /// <param name="settings">중첩될 각 PerlinNoise의 설정 = 중첩될 각 NoiseMap의 설정</param>
        /// <returns></returns>
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, Vector2 offset, int overlapSeed, NoiseMapSetting[] settings, eNormalizeMode normalizeMode, AnimationCurve heightCurve = null) {
            // 리턴할 노이즈맵이다.
            float[,] noiseMap = new float[mapWidth, mapHeight];
            float maxPossibleHeight = 0;
            // 중첩하여 더해질 각 PerlinNoise에 offset를 넣어, 같은 지점을 넣을 수 없게 만들어 최종적으로 NoiseMap이 불규칙하게 만들어진다.
            System.Random rand = new System.Random(overlapSeed);
            Vector2[] overlapOffset = new Vector2[settings.Length];
            for (int i = 0; i < settings.Length; i++) {
                float offsetX = rand.Next(-100000, 100000) + offset.x;
                float offsetY = rand.Next(-100000, 100000) + offset.y;
                overlapOffset[i] = new Vector2(offsetX, offsetY);
                maxPossibleHeight += settings[i].valueRatio;
            }

            // 0으로 나누는 오류 제거
            if (scale <= 0)
                scale = 0.00001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2;
            float halfHeight = mapHeight / 2;

            for (int y = 0; y < mapHeight; ++y) {
                for (int x = 0; x < mapWidth; ++x) {
                    float noiseHeight = 0;

                    for (int i = 0; i < settings.Length; i++) {
                        // Perlin Noise에 대입하기 위한 좌표로 최종적인 NoiseMap의 형상을 결정한다.
                        // scale은 Noise Map의 비율을 결정하며, scaleRatio로 각 중첩될 값들의 Noise Map 비율을 결정한다.
                        // overlapOffset은 동일한 좌표를 Perlin Noise에 넣지 않도록 만들어, 최종적인 Noise Map이 좀 더 불규칙하게 만들어준다.
                        float sampleX = (x - halfWidth + overlapOffset[i].x) * settings[i].scaleRatio / scale;
                        float sampleY = (y - halfHeight + overlapOffset[i].y) * settings[i].scaleRatio / scale;

                        // -1 ~ 1 까지의 PerlinNoise를 만들기 위하여 "* 2 - 1"을 하였다.
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        // 구한 Perlin Noise를 중첩하여 최종적인 해당 지점의 Noise Height 값을 구한다.
                        noiseHeight += perlinValue * settings[i].valueRatio;
                    }

                    noiseMap[x, y] = noiseHeight;

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;
                }
            }

            // 0,1 사이의 값으로 변화하여, 좀 더 편하게 사용할 수 있도록 만든다.
            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    if (normalizeMode == eNormalizeMode.Local)
                        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                    else
                        noiseMap[x, y] = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight);

                    if (heightCurve != null) {
                        heightCurve.Evaluate(noiseMap[x, y]);
                    }
                }
            }

            return noiseMap;
        }

        public static float[,] EditHeightMapWithTexture2D(float[,] originHeightMap, Texture2D desireShape) {

            Color[] desireHeightMap = new Color[] { };
            int desireWidth = 1;
            int desireHeight = 1;

            if (desireShape != null) {
                desireHeightMap = desireShape.GetPixels(0);
                desireWidth = desireShape.width;
                desireHeight = desireShape.height;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            int mapWidth = originHeightMap.GetLength(0);
            int mapHeight = originHeightMap.GetLength(1);

            float[,] newHeightMap = new float[mapWidth, mapHeight];

            int coefHeight = (int)((float)desireHeight / mapHeight);
            int coefWidth = (int)((float)desireWidth / mapWidth);

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    float height = desireHeightMap[(desireWidth * y * coefHeight) + (x * coefWidth)].a * originHeightMap[x, y];
                    newHeightMap[x, y] = height;

                    if (height > maxNoiseHeight)
                        maxNoiseHeight = height;
                    else if (height < minNoiseHeight)
                        minNoiseHeight = height;
                }
            }

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    newHeightMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, newHeightMap[x, y]);
                }
            }

            return newHeightMap;
        }

        public static float[,] EditHeightMapWithCircle(float[,] originHeightMap, float gradientSize, float circularGradientRate) {
            int mapWidth = originHeightMap.GetLength(1);
            int mapHeight = originHeightMap.GetLength(0);

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            float halfGradientSize = gradientSize * Mathf.Sqrt(halfWidth * halfWidth + halfHeight * halfHeight) / 2f;

            float[,] newHeightMap = new float[mapWidth, mapHeight];

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    // edit by settings
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(halfWidth, halfHeight));
                    float gradient = Mathf.Clamp01(1 - distance / halfGradientSize) * circularGradientRate;

                    float height = originHeightMap[x, y] + gradient;

                    newHeightMap[x, y] = height;

                    if (height > maxNoiseHeight)
                        maxNoiseHeight = height;
                    else if (height < minNoiseHeight)
                        minNoiseHeight = height;
                }
            }

            for (int y = 0; y < mapHeight; y++) {
                for (int x = 0; x < mapWidth; x++) {
                    newHeightMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, newHeightMap[x, y]);
                }
            }

            return newHeightMap;
        }
    }


}