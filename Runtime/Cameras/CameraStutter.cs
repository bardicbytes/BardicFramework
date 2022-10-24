using UnityEngine;

namespace BB.BardicFramework.Cameras
{
    public class CameraStutter : MonoBehaviour
    {
        public Camera target;
        public float stutterRate = 2;

        private void Start()
        {
            target.enabled = false;
        }

        void Update()
        {
            if (Mathf.Approximately(Time.frameCount % stutterRate, 0f))
            {

                target.Render();
            }
        }
    }
}