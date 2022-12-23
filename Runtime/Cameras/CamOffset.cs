//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    [System.Serializable]
    public class CamOffset
    {
        [SerializeField]
        public bool local = false;
        [SerializeField]
        public Vector3 position = new Vector3(10, 5, 10);
        [SerializeField]
        public Vector3 lookAt = new Vector3(0, 0, 10f);

        [Space]
        [SerializeField]
        public float aspectRatioA = 1.85f;
        [SerializeField]
        public float aspectRatioB = 1f;
        [SerializeField]
        public float orthoSizeA = 12.5f;
        [SerializeField]
        public float orthoSizeB = 20f;

        public float GetOrthoSize(float aspectRatio)
        {
            float t = Mathf.InverseLerp(aspectRatioA, aspectRatioB, aspectRatio);
            var s = Mathf.Lerp(orthoSizeA, orthoSizeB, t);
            return s;
        }
    }
}