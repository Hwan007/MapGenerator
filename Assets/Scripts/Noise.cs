using System.Collections;
using UnityEngine;

using namespace MapGenerator
{    
    public static class Noise
    {
        /// <summary>
        /// �־��� ���������� PerlinNoise�� ����Ͽ� Noise Map�� �����Ѵ�.
        /// </summary>
        /// <param name="mapWidth">����</param>
        /// <param name="mapHeight">����</param>
        /// <param name="scale">�⺻ PerlinNoise ���� = �⺻ NoiseMap ���� ����</param>
        /// <param name="offset">�⺻ PerlinNoise offset = �⺻ NoiseMap ���� ��ġ</param>
        /// <param name="overlapSeed">offset random seed�� ������ ���� ���� �� �ְ� �����. = Perlin Noise�� ���� ��ǥ���� �����ϰ� �����.</param>
        /// <param name="settings">��ø�� �� PerlinNoise�� ���� = ��ø�� �� NoiseMap�� ����</param>
        /// <returns></returns>
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, Vector2 offset, int overlapSeed, NoiseMapSetting[] settings, Texture2D desireShape = null)
        {
            // ������ ��������̴�.
            float[,] noiseMap = new float[mapWidth, mapHeight];

            // ��ø�Ͽ� ������ �� PerlinNoise�� offset�� �־�, ���� ������ ���� �� ���� ����� ���������� NoiseMap�� �ұ�Ģ�ϰ� ���������.
            System.Random rand = new System.Random(overlapSeed);
            Vector2[] overlapOffset = new Vector2[settings.Length];
            for (int i = 0; i < settings.Length; i++)
            {
                float offsetX = rand.Next(-100000, 100000) + offset.x;
                float offsetY = rand.Next(-100000, 100000) + offset.y;
                overlapOffset[i] = new Vector2(offsetX, offsetY);
            }

            // 0���� ������ ���� ����
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
                        // Perlin Noise�� �����ϱ� ���� ��ǥ�� �������� NoiseMap�� ������ �����Ѵ�.
                        // scale�� Noise Map�� ������ �����ϸ�, scaleRatio�� �� ��ø�� ������ Noise Map ������ �����Ѵ�.
                        // overlapOffset�� ������ ��ǥ�� Perlin Noise�� ���� �ʵ��� �����, �������� Noise Map�� �� �� �ұ�Ģ�ϰ� ������ش�.
                        float sampleX = (x-halfWidth) / scale * settings[i].scaleRatio + overlapOffset[i].x;
                        float sampleY = (y-halfHeight) / scale * settings[i].scaleRatio + overlapOffset[i].y;

                        // -1 ~ 1 ������ PerlinNoise�� ����� ���Ͽ� "* 2 - 1"�� �Ͽ���.
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        // ���� Perlin Noise�� ��ø�Ͽ� �������� �ش� ������ Noise Height ���� ���Ѵ�.
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

            // 0,1 ������ ������ ��ȭ�Ͽ�, �� �� ���ϰ� ����� �� �ֵ��� �����.
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
        /// �̾Ƴ� PerlinNoise ���� ��� ���� �ݿ��� ������ �����Ѵ�.
        /// </summary>
        public float valueRatio;
        /// <summary>
        /// PerlinNoise�� �����ϴ� ��ǥ��(x,y)�� ����(��ȭ��)�̴�.
        /// </summary>
        public float scaleRatio;
    }
}