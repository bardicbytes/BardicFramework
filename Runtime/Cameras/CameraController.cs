//Copyright 2020 Bardic Bytes, LLC
//alex@bardicbytes.com
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    [System.Serializable]
    public class CameraShakeConfig
    {
        public bool zAffectsBloom;
        public float magnitude;
        public float duration;

        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public AnimationCurve zCurve;

        public Vector3 Eval(float t)
        {
            var x = (xCurve.Evaluate(t) * magnitude) - (magnitude * 0.5f);
            var y = (yCurve.Evaluate(t) * magnitude) - (magnitude * 0.5f);
            var z = (zCurve.Evaluate(t) * magnitude) - (magnitude * 0.5f);
            return new Vector3(x, y, z);
        }
    }

    public abstract class CameraController : ActorModule
    {
        [SerializeField]
        protected CameraManager manager;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("camera")]
        protected Camera targetCam = default;
        [SerializeField]
        protected bool activateOnAwake = false;

        [SerializeField]
        protected float bloomMult = 1;

        [SerializeField]
        [HideInInspector]
        protected List<Camera> childCameras;

        public Camera Camera => targetCam;

        public virtual float SizeMult => 1f;

        protected Vector3 initLocalPos;


        protected override void OnValidate()
        {
            base.OnValidate();
            if (targetCam == null) targetCam = GetComponent<Camera>();
            targetCam.GetComponentsInChildren(true, childCameras);
            if (childCameras != null && childCameras.Contains(targetCam)) childCameras.Remove(targetCam);
        }

        protected virtual void Awake()
        {
            initLocalPos = transform.localPosition;

            if (activateOnAwake) manager.ActivateCamera(this);
        }


        public virtual void Exit()
        {
            targetCam.gameObject.SetActive(false);
        }

        public virtual void Enter()
        {
            targetCam.gameObject.SetActive(true);
        }

        public void Shake(CameraShakeConfig shakeConfig)
        {
            StartCoroutine(ShakeCoroutine(shakeConfig));
        }

        private IEnumerator ShakeCoroutine(CameraShakeConfig shakeConfig)
        {
            float st = Time.time;
            float et = Time.time + shakeConfig.duration;
            float t = 0;
            Vector3 lastOffSet = Vector3.zero;
            while (Time.time < et)
            {
                yield return null;
                t = (Time.time - st) / shakeConfig.duration;
                var o = shakeConfig.Eval(t);
                float z = Mathf.Max(o.z, .2f);
                o.z = 0;

                transform.localPosition = transform.localPosition - lastOffSet + o;
                lastOffSet = o;
            }
            transform.localPosition -= lastOffSet;
        }

        protected virtual void SetOrthoSize(float s)
        {
            targetCam.orthographicSize = s;
        }
    }
}