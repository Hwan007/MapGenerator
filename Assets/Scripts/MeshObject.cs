using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator {
    public class MeshObject : MonoBehaviour {
        public MeshFilter MeshFilter { get; protected set; }
        public MeshRenderer MeshRenderer { get; protected set; }
    
        public MeshData MeshData { get; protected set; }
        public MeshObject CreateObject(MeshData meshData = null, Texture2D texture = null) {
            var obj = new GameObject();
            MeshRenderer = obj.AddComponent<MeshRenderer>();
            MeshFilter = obj.AddComponent<MeshFilter>();
            if (meshData != null)
                SetMesh(meshData);
            else
                SetMesh(MeshData.PlaneMesh(texture.width, texture.height));

            if (texture != null)
                SetTexture(texture);
            return this;
        }

        public void SetMesh(MeshData meshData) {
            MeshData = meshData;
            MeshFilter.sharedMesh = meshData.CreateMesh();
        }

        public void SetTexture(Texture2D texture) {
            MeshRenderer.sharedMaterial.mainTexture = texture;
        }
    }
}