using System.Collections;
using UnityEngine;

using namespace MapGenerator
{    
    public static class Noise
    {
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
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, Vector2 offset, int overlapSeed, NoiseMapSetting[] settings, Texture2D desireShape = null)
        {
            // 리턴할 노이즈맵이다.
            float[,] noiseMap = new float[mapWidth, mapHeight];

            // 중첩하여 더해질 각 PerlinNoise에 offset를 넣어, 같은 지점을 넣을 수 없게 만들어 최종적으로 NoiseMap이 불규칙하게 만들어진다.
            System.Random rand = new System.Random(overlapSeed);
            Vector2[] overlapOffset = new Vector2[settings.Length];
            for (int i = 0; i < settings.Length; i++)
            {
                float offsetX = rand.Next(-100000, 100000) + offset.x;
                float offsetY = rand.Next(-100000, 100000) + offset.y;
                overlapOffset[i] = new Vector2(offsetX, offsetY);
            }

            // 0으로 나누는 오류 제거
            if (scale <= 0)
                scale = 0.00001f;

            Color[] desireHeightMap = new Color[] { };
            int desireWidth = 1;
            int desireHeight = 1;

            if (desireShape != null)
            {
                desireHeightMap = desireShape.GetPixels(0);
                desireWidth = desireShape.width;
                desireHeight = desireShape.height;
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = mapWidth / 2;
            float halfHeight = mapHeight / 2;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = 0;

                    for (int i = 0; i < settings.Length; i++)
                    {
                        // Perlin Noise에 대입하기 위한 좌표로 최종적인 NoiseMap의 형상을 결정한다.
                        // scale은 Noise Map의 비율을 결정하며, scaleRatio로 각 중첩될 값들의 Noise Map 비율을 결정한다.
                        // overlapOffset은 동일한 좌표를 Perlin Noise에 넣지 않도록 만들어, 최종적인 Noise Map이 좀 더 불규칙하게 만들어준다.
                        float sampleX = (x-halfWidth) / scale * settings[i].scaleRatio + overlapOffset[i].x;
                        float sampleY = (y-halfHeight) / scale * settings[i].scaleRatio + overlapOffset[i].y;

                        // -1 ~ 1 까지의 PerlinNoise를 만들기 위하여 "* 2 - 1"을 하였다.
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        // 구한 Perlin Noise를 중첩하여 최종적인 해당 지점의 Noise Height 값을 구한다.
                        noiseHeight += perlinValue * settings[i].valueRatio;
                    }

                    if (desireShape != null)
                        noiseMap[x, y] = desireHeightMap[desireWidth * y * desireHeight / mapHeight + x * desireWidth / mapWidth].a * noiseHeight;
                    else
                        noiseMap[x, y] = noiseHeight;

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;
                }
            }

            // 0,1 사이의 값으로 변화하여, 좀 더 편하게 사용할 수 있도록 만든다.
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
                }
            }

            return noiseMap;
        }
    }

    [System.Serializable]
    public struct NoiseMapSetting
    {
        /// <summary>
        /// 뽑아낸 PerlinNoise 값을 어느 정도 반영할 것인지 결정한다.
        /// </summary>
        public float valueRatio;
        /// <summary>
        /// PerlinNoise를 대입하는 좌표값(x,y)의 기울기(변화율)이다.
        /// </summary>
        public float scaleRatio;
    }
}