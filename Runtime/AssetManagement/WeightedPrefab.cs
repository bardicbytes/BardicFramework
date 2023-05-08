using UnityEngine;

namespace BardicBytes.BardicFramework.AssetManagement
{
    [System.Serializable]
    public abstract class WeightedPrefab<T> where T : Component
    {
        [SerializeField]
        [HideInInspector]
        private string editorName = default;
        [SerializeField]
        protected T prefab = default;
        [SerializeField]
        protected float weight = default;

        public T Prefab => prefab;
        public float Weight => weight;

        public WeightedPrefab()
        {
            this.weight = 100;
            DoValidate();
        }

        public void DoValidate()
        {
            editorName = prefab != null ? "(" + Weight + ") " + prefab.name : "No Prefab";
        }
    }
}
