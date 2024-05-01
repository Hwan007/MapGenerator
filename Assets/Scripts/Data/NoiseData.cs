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
        /// �̾Ƴ� PerlinNoise ���� ��� ���� �ݿ��� ������ �����Ѵ�.
        /// </summary>
        public float valueRatio;
        /// <summary>
        /// PerlinNoise�� �����ϴ� ��ǥ��(x,y)�� ����(��ȭ��)�̴�.
        /// </summary>
        public float scaleRatio;
    }
}