using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace MapGenerator
{
    [CreateAssetMenu()]
    public class NoiseData : UpdatableData
    {
        public float noiseScale;
        public Vector2 offset;
        public Noise.eNormalizeMode normalizeMode;
        public NoiseMapSetting[] settings;
        public int seed;

        public AnimationCurve heightMultiplierCurve;

        public bool useShape;
        public Texture2D desireShape;

        public bool useCircle;
        public float gradient;
        public float gradientRate;

        protected void OnValidate()
        {
            for (int i = 0; i < settings.Length; ++i)
            {
                if (settings[i].valueRatio > 1)
                {
                    settings[i].valueRatio = 1;
                }
                if (settings[i].scaleRatio < 1)
                {
                    settings[i].scaleRatio = 1;
                }
            }
        }

        public NoiseData Offset(Vector2 offset) {
            this.offset = offset;
            return this;
        }
    }

    [System.Serializable]
    public struct NoiseMapSetting
    {
        public float valueRatio;
        public float scaleRatio;
    }

    [CustomEditor(typeof(NoiseData))]
    public class CustomEditorNoiseData : CustomEditorUpdatableData {

    }
}