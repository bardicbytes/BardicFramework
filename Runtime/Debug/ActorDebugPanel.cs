using BardicBytes.BardicFramework.EventVars;
using TMPro;
using UnityEngine;

namespace BardicBytes.BardicFramework
{
    [RequireComponent(typeof(RectTransform))]
    public class ActorDebugPanel : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public StringEventVar updateEV;

        [Min(1)]
        public int frameInterval = 1;

        private float lastUpdate = 0;

        private void Awake()
        {
            updateEV.AddListener(SetText);
            gameObject.SetActive(false);
        }
        public void SetText(string text)
        {
            if (Time.frameCount % frameInterval != 0) return;

            lastUpdate = Time.time;
            gameObject.SetActive(true);
            this.text.text = text;
        }
    }
}
