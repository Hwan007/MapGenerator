using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MapGenerator {
    public static class MeshGenerator {
        public static MeshData GenerateMesh(float[,] heightMap, int levelOfDetail, float heightMultiplier, float xzBaseLength = 1f) {
            int widthVertex = heightMap.GetLength(0);
            int heightVertex = heightMap.GetLength(1);

            float topLeftX = xzBaseLength * (widthVertex - 1) / -2f;
            float topLeftZ = xzBaseLength * (heightVertex - 1) / 2f;

            int vertexSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            int verticesPerLine = (widthVertex - 1) / vertexSimplificationIncrement + 1;

            // MeshData meshData = new MeshData(width, height);
            MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
            int vertexIndex = 0;

            for (int y = 0; y < heightVertex; y += vertexSimplificationIncrement) {
                for (int x = 0; x < widthVertex; x += vertexSimplificationIncrement) {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x * xzBaseLength, heightMultiplier * heightMap[x, y], topLeftZ - y * xzBaseLength);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)widthVertex, y / (float)heightVertex);

                    if (x < widthVertex - 1 && y < heightVertex - 1) {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }
    }

    public class MeshData {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;

        int triangleIndex;

        public MeshData(int meshWidth, int meshHeight) {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c) {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public static MeshData PlaneMesh(int width, int height) {
            float[,] plane = new float[width, height];
            MeshData mesh = MeshGenerator.GenerateMesh(plane, 0, 0);
            return mesh;
        }

        public Mesh CreateMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}