using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public static class MeshGenerator
    {
        public static MeshData[] GenerateMultipleMesh(int sizeXsize, float[,] heightMap, int levelOfDetail, float xzBaseLength = 1f)
        {
            int sizeSquare = sizeXsize * sizeXsize;
            MeshData[] meshDatas = new MeshData[sizeSquare];

            int xSizeUnit = (heightMap.GetLength(0) - 1) / sizeXsize + 1;
            int ySizeUnit = (heightMap.GetLength(1) - 1) / sizeXsize + 1;

            while (xSizeUnit > 241 && ySizeUnit > 241)
            {
                ++sizeXsize;
                sizeSquare = sizeXsize * sizeXsize;

                xSizeUnit = (heightMap.GetLength(0) - 1) / sizeXsize + 1;
                ySizeUnit = (heightMap.GetLength(1) - 1) / sizeXsize + 1;
            }

            for (int i = 0; i < sizeSquare; ++i)
            {
                float[,] targetHeightMap = new float[xSizeUnit, ySizeUnit];
                for (int x = 0; x < xSizeUnit; ++x)
                {
                    int originX = xSizeUnit * (i % sizeXsize) + x;
                    for (int y = 0; y < ySizeUnit; ++y)
                    {
                        int originY = ySizeUnit * (i / sizeXsize) + y;
                        targetHeightMap[x, y] = heightMap[originX, originY];
                    }
                }
                // meshDatas[i] = GenerateMesh();
            }

            return meshDatas;
        }

        public static MeshData GenerateMesh(float[,] heightMap, int levelOfDetail, float xzBaseLength = 1f)
        {
            int widthVertex = heightMap.GetLength(0);
            int heightVertex = heightMap.GetLength(1);

            float topLeftX = xzBaseLength * (widthVertex - 1) / -2f;
            float topLeftZ = xzBaseLength * (heightVertex - 1) / 2f;

            int vertexSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            int verticesPerLine = (widthVertex - 1) / vertexSimplificationIncrement + 1;

            // MeshData meshData = new MeshData(width, height);
            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int vertexIndex = 0;

            for (int y = 0; y < heightVertex; y += vertexSimplificationIncrement)
            {
                for (int x = 0; x < widthVertex; x += vertexSimplificationIncrement)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x * xzBaseLength, heightMap[x, y], topLeftZ - y * xzBaseLength);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)widthVertex, y / (float)heightVertex);

                    if (x < widthVertex - 1 && y < heightVertex - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        int triangleIndex;

        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}