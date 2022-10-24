//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;

namespace BB.BardicFramework.Cameras
{
    public class FixedCameraController : CameraController
    {
        [SerializeField]
        private Vector3 goalRot = default;
        [SerializeField]
        private float dur = default;
        [SerializeField]
        private AnimationCurve curve = default;
        [SerializeField]
        private Vector3 wobbleAngle = default;
        [SerializeField]
        private float wobbleRate = 1f;
        [SerializeField]
        private BoolEventVar screenFadeEvent = default;
        [SerializeField]
        private BoolEventVar instantFadeEvent = default;

        [Space]
        [SerializeField]
        public float aspectRatioA = 1.85f;
        [SerializeField]
        public float aspectRatioB = 1f;
        [SerializeField]
        public float orthoSizeA = 12.5f;
        [SerializeField]
        public float orthoSizeB = 20f;

        private Quaternion initParentRot;
        private float startTime;
        private bool isInRot;
        private bool isInitialized;
        private Quaternion initWobbleRot;

        public float OrthoSize => GetOrthoSize(ScreenAspect);
        public float ScreenAspect => (float)Screen.width / (float)Screen.height;

        private void Update()
        {
            DoRot();
            Wobble();
            var os = OrthoSize;
            if (!Mathf.Approximately(Camera.orthographicSize, os))
            {
                //Debug.Log("orthosize = "+os);
                Camera.orthographicSize = os;
                for(int i =0; i < childCameras.Count; i++)
                {
                    childCameras[i].orthographicSize = os;
                }
            }
        }

        public float GetOrthoSize(float aspectRatio)
        {
            float t = Mathf.InverseLerp(aspectRatioA, aspectRatioB, aspectRatio);
            var s = Mathf.Lerp(orthoSizeA, orthoSizeB, t);
            return s;
        }

        private void Wobble()
        {
            float x = Mathf.Sin(Time.unscaledTime + 1) * wobbleRate * wobbleAngle.x;
            float y = Mathf.Sin(Time.unscaledTime + 3) * wobbleRate * wobbleAngle.y;
            float z = Mathf.Sin(Time.unscaledTime + 5) * wobbleRate * wobbleAngle.z;
            Quaternion goal = initWobbleRot * Quaternion.Euler(x, y, z);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, goal, Time.unscaledDeltaTime * 8);
        }

        public override void Enter()
        {
            instantFadeEvent.Raise(false);
            screenFadeEvent.Raise(true);
            Init();
            base.Enter();
        }

        private void Init()
        {
            isInRot = true;
            startTime = Time.unscaledTime;

            if (isInitialized) return;
            initWobbleRot = transform.localRotation;
            isInitialized = true;
            initParentRot = transform.parent.localRotation;
        }

        public override void Exit()
        {
            base.Exit();
            transform.parent.localRotation = initParentRot;
        }

        private void DoRot()
        {
            if (!isInRot) return;
            float t = (Time.unscaledTime - startTime) / dur;
            t = curve.Evaluate(t);
            transform.parent.localRotation = Quaternion.Slerp(initParentRot, Quaternion.Euler(goalRot), t);
            //transform.parent.localEulerAngles = Vector3.Lerp(initParentRot, goalRot, t);
            if (t >= 1) isInRot = false;
        }

    }
}