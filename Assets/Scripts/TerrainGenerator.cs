using System;
using UnityEngine;

namespace MapGenerator
{
    public static class TerrainGenerator
    {
        public static MeshData[] GenerateMultipleMesh(int sizeXsize, float[,] heightMap, int levelOfDetail, float xzBaseLength = 1f)
        {
            int sizeSquare = sizeXsize * sizeXsize;
            MeshData[] meshDatas = new MeshData[sizeSquare];

            int xChunkSizeUnit = (heightMap.GetLength(0) - 1) / sizeXsize + 1;
            int yChunkSizeUnit = (heightMap.GetLength(1) - 1) / sizeXsize + 1;

            while (xChunkSizeUnit > 241 || yChunkSizeUnit > 241)
            {
                ++sizeXsize;
                sizeSquare = sizeXsize * sizeXsize;

                xChunkSizeUnit = (heightMap.GetLength(0) - 1) / sizeXsize + 1;
                yChunkSizeUnit = (heightMap.GetLength(1) - 1) / sizeXsize + 1;
            }

            for (int i = 0; i < sizeSquare; ++i)
            {
                float[,] targetHeightMap = new float[xChunkSizeUnit, yChunkSizeUnit];

                int originXOffset = xChunkSizeUnit * (i % sizeXsize);
                int originYOffset = yChunkSizeUnit * (i / sizeXsize);

                for (int x = 0; x < xChunkSizeUnit; ++x)
                {
                    for (int y = 0; y < yChunkSizeUnit; ++y)
                    {
                        targetHeightMap[x, y] = heightMap[originXOffset + x, originYOffset + y];
                    }
                }
                meshDatas[i] = MeshGenerator.GenerateMesh(targetHeightMap, levelOfDetail, xzBaseLength);
            }

            return meshDatas;
        }
    }
}