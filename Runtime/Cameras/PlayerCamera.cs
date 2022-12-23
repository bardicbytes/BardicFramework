//alex@bardicbytes.com
using System.Collections;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{

    [RequireComponent(typeof(Camera))]
    public class PlayerCamera : CameraController, IOffsetCameraController
    {
        [SerializeField]
        private ActorTag targetTag = default;
        [Space]
        [SerializeField]
        private Camera overlay = default;
        
        [Space]
        [SerializeField]
        private Vector2 distLeash = Vector2.zero;
        [SerializeField]
        private Vector2 leashOffset = Vector2.zero;
        [SerializeField]
        private bool easeRot;
        [SerializeField]
        private float camRotRate = 3;
        [SerializeField]
        private bool easePos;
        [SerializeField]
        private float camMoveRate = 5;
        
        [Space]
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
            if (altOffsets == null) return;
            //altOffsets[0].local = false;
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

        protected override void ActorUpdate()
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

            float t = easePos && Application.isPlaying ? camMoveRate * Time.deltaTime : 1;
            Vector3 p = Vector3.Lerp(initCamPos, goalCamPos, t);

            targetCam.transform.position = currentOffset.local ? p : currentOffset.position;

            Debug.DrawLine(lookAtTarget, transform.position, Color.gray);
            if (Application.isPlaying)
            {
                Debug.DrawLine(initCamPos, transform.position, Color.gray, 3f);
            }

            Quaternion lookRot = Quaternion.LookRotation(lookAtTarget - transform.position, Vector3.up);
            t = easeRot && Application.isPlaying ? camRotRate * Time.deltaTime : 1;
            Quaternion r = Quaternion.Lerp(targetCam.transform.rotation, lookRot, t);
            targetCam.transform.rotation = currentOffset.local ? r : Quaternion.LookRotation(currentOffset.lookAt - transform.position, Vector3.up);
            SetOrthoSize(OrthoSize);
        }

        protected override void SetOrthoSize(float s)
        {
//#if DEBUG
//            if(targetCam.orthographicSize != s) Debug.Log("new OS "+s);
//#endif
            base.SetOrthoSize(s);
            if(overlay != null) overlay.orthographicSize = s;
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