//alex@bardicbytes.com
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public abstract class Action : ScriptableObject
    {
        [field:SerializeField]
        protected bool Valid { get; set; } = true;
        [field: SerializeField]
        public virtual string FullName { get; protected set; }

        protected virtual void Reset()
        {
            FullName = name;
        }
        protected virtual void OnValidate() {
            if (string.IsNullOrEmpty(FullName)) FullName = name;
        }
    }

    public abstract class Action<TAction, TPerformer, TRuntime> : Action
        where TAction : Action<TAction, TPerformer, TRuntime>
        where TPerformer : ActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : ActionRuntime<TAction, TPerformer, TRuntime>
    {
        public abstract int PhaseDataCount { get; }
        public abstract class PhaseData
        {
            public string name;
            public float duration;

            public virtual bool HandleOnEnter(TRuntime runtime) { return true; }
            public virtual bool HandleOnUpdate(TRuntime runtime) { return true; }
            public virtual bool HandleOnExit(TRuntime runtime) { return true; }
        }

        public abstract PhaseData GetPhaseData(int i);

        public abstract TRuntime GetRuntime(TPerformer actionModule);
    }
}