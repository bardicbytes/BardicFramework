using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    [CreateAssetMenu(menuName = Core.Prefixes.Cameras+"Camera Manager")]
    public class CameraManager : ScriptableObject
    {
        [SerializeField]
        private CameraControllerEventVar cameraChangeEvent = default;
        [SerializeField]
        private CameraShakeEventVar shakeEvent = default;

        private CameraController activeCamera = null;
        private CursorCameraController cursorCam = null;

        public CameraController ActiveCamera => activeCamera;

        private Stack<CameraController> history;

        public void Initialize()
        {
            history = new Stack<CameraController>();
            cameraChangeEvent.AddListener(HandleCameraChange);
            shakeEvent.AddListener(HandleShakeEv);
        }

        private void HandleShakeEv(CameraShakeConfig shakeConfig)
        {
            //Debug.Log("HandleShakeEv");
            activeCamera.Shake(shakeConfig);
        }

        private void HandleCameraChange(CameraController nextCamera)
        {
            ActivateCamera(nextCamera);
        }

        public Ray ScreenPointToRay(Vector3 screenPosition)
        {
            return cursorCam.Camera.ScreenPointToRay(screenPosition);
        }

        public float GetClippingDistance()
        {
            return activeCamera.Camera.farClipPlane - activeCamera.Camera.nearClipPlane;
        }

        public void ActivateCamera(CursorCameraController cursorCam)
        {
            this.cursorCam = cursorCam;
            cursorCam.Enter();
        }

        public void ActivateCamera(CameraController camera)
        {
            if (history == null) history = new Stack<CameraController>();
            //Debug.Log("camera activated "+camera.name);
            if (activeCamera != null)
            {
                history.Push(activeCamera);
                activeCamera.Exit();
            }
            activeCamera = camera;
            if (activeCamera != null)
            {
                activeCamera.Enter();
            }
        }

        public void ActivatePrevCamera()
        {
            if (history == null || history.Count == 0) return;
            ActivateCamera(history.Pop());
        }
    }
}