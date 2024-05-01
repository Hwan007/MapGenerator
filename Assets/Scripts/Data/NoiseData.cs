using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MapGenerator {
    [CreateAssetMenu()]
    public class NoiseData : UpdatableData {
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
    }

    [System.Serializable]
    public struct NoiseMapSetting {
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