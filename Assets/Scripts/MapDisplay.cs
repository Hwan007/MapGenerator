using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public class MapDisplay : MonoBehaviour {
        public Renderer textureRender;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public void DrawTexture(Texture2D texture) {
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }

        public void DrawMesh(MeshData meshData, Texture2D texture, Material material = null) {
            meshFilter.sharedMesh = meshData.CreateMesh();
            if (material != null ) {
                meshRenderer.sharedMaterial = material;
            }
            meshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}