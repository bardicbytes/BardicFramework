using UnityEngine;
using System.Collections;
using SF = UnityEngine.SerializeField;

namespace BardicBytes.BardicFramework
{
    public class SimpleRotation : MonoBehaviour
    {
        public Vector3 eulerRot = Vector3.zero;

        public enum TimeEnum { Scaled = 0, Unscaled = 1}

        [SF] private Space space = Space.Self;
        [SF] private TimeEnum timeScale = TimeEnum.Scaled;

        [SF] private bool applyWave = false;
        [SF] private bool squareWave = false;
        [SF] private bool abs = false;
        [SF] private bool minZero = false;
        [SF] private float freq = 2f;
        private Transform TargetTransform => transform;//can make more generic later

        private float TimeScale => timeScale == TimeEnum.Scaled ? Time.deltaTime : Time.unscaledDeltaTime;

        private Quaternion initRot;

        private void Awake()
        {
            initRot = TargetTransform.rotation;
        }

        private void OnEnable()
        {
            initRot = TargetTransform.transform.rotation;
        }

        void Update()
        {
            float wave = 1;
            if (applyWave)
            {
                wave = Mathf.Sin(freq * (timeScale == TimeEnum.Scaled ? Time.time : Time.unscaledTime));
                if(squareWave)
                {
                    wave = Mathf.Round(wave);
                }
                if (abs)
                {
                    wave = Mathf.Abs(wave);
                }
                else if(minZero)
                {
                    wave = Mathf.Max(0, wave);
                }
            }
            TargetTransform.Rotate(eulerRot * TimeScale * wave, this.space);

        }
    }
}