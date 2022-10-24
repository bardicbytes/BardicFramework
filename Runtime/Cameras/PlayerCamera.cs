//Copyright 2020 Bardic Bytes, LLC
//alex@bardicbytes.com
using System.Collections;
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

    public class PlayerCamera : CameraController, IOffsetCameraController
    {
        [SerializeField]
        private Core.ActorTag targetTag = default;
        [Space]
        [SerializeField]
        private Camera overlay = default;
        [SerializeField]
        private float camRotRate = 3;
        [SerializeField]
        private float camMoveRate = 5;
        [SerializeField]
        private float transitionDur = 1;
        [SerializeField]
        private CamOffset[] altOffsets;


        private CamOffset currentOffset = default;
        private CamOffset goalOffset = default;
        private Coroutine currentTransition = null;


        private float HDAR => 16 / 9f;

        private float DefaultWith => OrthoSize * HDAR;

        public override float SizeMult => targetCam.orthographicSize / altOffsets[0].GetOrthoSize(ScreenAspect);

        public float OrthoSize => currentOffset.GetOrthoSize(ScreenAspect);

        public float ScreenAspect => Screen.width / Screen.height;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (altOffsets == null || altOffsets.Length == 0)
                altOffsets = new CamOffset[3];

            altOffsets[0].local = false;
            altOffsets[0].position = transform.position;
            altOffsets[0].lookAt = transform.position + transform.forward;
        }

        protected override void Awake()
        {
            currentOffset = altOffsets[0];
            goalOffset = currentOffset;
            SetOrthoSize(OrthoSize);
            transform.SetParent(null);
            base.Awake();
        }


        protected void Update()
        {
            if (!Application.isPlaying)
            {
                currentOffset = altOffsets[0];
                goalOffset = currentOffset;
            }
            MoveCamera();

        }

        private void MoveCamera()
        {
            Transform tt = targetTag.Actor0.transform;
            Vector3 initCamPos = transform.position;
            Vector3 lookAtWorldOffset = tt.TransformVector(currentOffset.lookAt);
            Vector3 lookAtTarget = tt.position + lookAtWorldOffset;
            Vector3 worldOffset = tt.TransformVector(currentOffset.position);
            Vector3 goalCamPos = tt.position + worldOffset;

            float t = Application.isPlaying ? camMoveRate * Time.deltaTime : 1;
            Vector3 p = Vector3.Lerp(initCamPos, goalCamPos, t);
            targetCam.transform.position = currentOffset.local ? p : currentOffset.position;

            Debug.DrawLine(lookAtTarget, transform.position, Color.gray);
            if (Application.isPlaying)
            {
                Debug.DrawLine(initCamPos, transform.position, Color.gray, 3f);
            }


            Quaternion lookRot = Quaternion.LookRotation(lookAtTarget - transform.position, Vector3.up);
            t = Application.isPlaying ? camRotRate * Time.deltaTime : 1;
            Quaternion r = Quaternion.Lerp(targetCam.transform.rotation, lookRot, t);
            targetCam.transform.rotation = currentOffset.local ? r : Quaternion.LookRotation(currentOffset.lookAt - transform.position, Vector3.up);
            SetOrthoSize(OrthoSize);
        }

        protected override void SetOrthoSize(float s)
        {
            Debug.Log("new OS "+s);
            base.SetOrthoSize(s);
            overlay.orthographicSize = s;
        }

        #region offsets
        [ContextMenu("Offset 0")]
        public void OffsetZero()
        {
            ApplyOffset(altOffsets[0]);
        }

        [ContextMenu("Offset 1")]
        public void OffsetOne()
        {
            ApplyOffset(altOffsets[1]);
        }

        [ContextMenu("Offset 2")]
        public void OffsetTwo()
        {
            ApplyOffset(altOffsets[2]);
        }

        [ContextMenu("Offset 3")]
        public void OffsetThree()
        {
            ApplyOffset(altOffsets[2]);
        }

        public void ApplyOffset(CamOffset newOffset)
        {
            if (currentTransition != null)
            {
                StopCoroutine(currentTransition);
                currentTransition = null;
            }
            this.goalOffset = newOffset;
            currentTransition = StartCoroutine(Transition());
        }

        public void ApplyDefaultOffset()
        {
            OffsetZero();
        }

        private IEnumerator Transition()
        {
            CamOffset startOffset = currentOffset;
            float startTime = Time.time;
            while (Time.time < Time.time + transitionDur)
            {
                yield return null;
                float t = (Time.time - startTime) / transitionDur;
                currentOffset.orthoSizeA= Mathf.Lerp(startOffset.orthoSizeA, goalOffset.orthoSizeA, t);
                currentOffset.orthoSizeB = Mathf.Lerp(startOffset.orthoSizeB, goalOffset.orthoSizeB, t);
                currentOffset.lookAt = Vector3.Lerp(startOffset.lookAt, goalOffset.lookAt, t);
                currentOffset.position = Vector3.Lerp(startOffset.position, goalOffset.position, t);
            }
            currentOffset = goalOffset;
            currentTransition = null;
        }
        #endregion


    }
}