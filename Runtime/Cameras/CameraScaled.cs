using System.Collections;
using UnityEngine;

namespace BardicBytes.BardicFramework.Cameras
{
    public class CameraScaled : MonoBehaviour
    {
        public CameraManager camMan = default;

        private Vector3 initLocalScale;

        private void OnValidate()
        {
#if UNITY_EDITOR

            if (camMan == null)
            {
                var guids = UnityEditor.AssetDatabase.FindAssets("t:CameraManager", null);
                foreach (string g in guids)
                {
                    var p = UnityEditor.AssetDatabase.GUIDToAssetPath(g);

                    camMan = UnityEditor.AssetDatabase.LoadAssetAtPath<CameraManager>(p);
                    Debug.Log("Fixed Missing camera manager reference on CameraScaled");
                    break;
                }

            }
#endif
        }

        private void Awake()
        {
            this.initLocalScale = transform.localScale;
        }

        private void OnEnable()
        {
            try
            {
                //Debug.Log(camMan.ActiveCamera.SizeMult+ "SM. "+gameObject.name);
                StartCoroutine(RescaleDelay());
            }
            catch (System.NullReferenceException ex)
            {
                Debug.LogError("null ref in camera scaled on enable. CamMan? " + camMan);
                if (camMan != null) Debug.LogError("null ref in camera scaled on enable. active cam?? " + camMan.ActiveCamera);
                throw ex;
            }
        }

        private IEnumerator RescaleDelay()
        {
            while (camMan == null || camMan.ActiveCamera == null)
            {
                Debug.LogWarning("No active camera, can't rescale " + gameObject.name);
                yield return null;
            }
            transform.localScale = initLocalScale * camMan.ActiveCamera.SizeMult;
        }
    }
}