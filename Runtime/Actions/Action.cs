//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public abstract class Action : ScriptableObject
    {
        [field: SerializeField]
        protected bool Valid { get; set; } = true;
        [field: SerializeField]
        public virtual string FullName { get; protected set; }

        protected virtual void Reset()
        {
            FullName = name;
        }
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(FullName)) FullName = name;
        }
    }
}