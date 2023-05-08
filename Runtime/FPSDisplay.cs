// http://wiki.unity3d.com/index.php?title=FramesPerSecond
using UnityEngine;
using System.Collections;

namespace BardicBytes.BardicFramework
{
    public class FPSDisplay : MonoBehaviour
    {
        public Vector2 offset = default;
        public Vector2 screenPos = default;
        public Vector2 relativeSize = default;
        public int c = 30;

        private float deltaTime = 0.0f;
        private float Msec => deltaTime * 1000.0f;
        private float Fps => 1.0f / deltaTime;
        private string Text => string.Format("{0:0.0} ms ({1:0.} fps)", Msec, Fps);

        private string text = "";

        private void Awake()
        {
            enabled = false;
        }


        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            if (Time.frameCount % c == 0) text = Text;
        }

        public void Toggle()
        {
            enabled = !enabled;
        }

        void OnGUI()
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(w * screenPos.x + offset.x, h * screenPos.y + offset.y, w * relativeSize.x, h * relativeSize.y);

            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = Color.black;
            GUI.Label(rect, text, style);
            rect.position += Vector2.one;
            style.normal.textColor = Color.white;
            GUI.Label(rect, text, style);
        }
    }
}