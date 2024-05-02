using MapGenerator;
using System.Collections;
using System.Collections.Generic;
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

        protected override void OnValidate()
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

            base.OnValidate();
        }
    }

    [System.Serializable]
    public struct NoiseMapSetting
    {
        /// <summary>
        /// Perlin Noise 값이 어느 정도 반영되는지 결정
        /// </summary>
        public float valueRatio;
        /// <summary>
        /// Perlin Noise 에 넣는 좌표값을 변화율
        /// </summary>
        public float scaleRatio;
    }
}