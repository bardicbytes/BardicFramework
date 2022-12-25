//alex@bardicbytes.com

namespace BardicBytes.BardicFramework.Actions
{
    public abstract class GenericAction<TAction, TPerformer, TRuntime> : Action
        where TAction : GenericAction<TAction, TPerformer, TRuntime>
        where TPerformer : GenericActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : GenericActionRuntime<TAction, TPerformer, TRuntime>
    {
        public abstract int PhaseDataCount { get; }
        public class PhaseData
        {
            public string name;
            public float duration;

            public virtual bool HandleOnEnter(TRuntime runtime) { return true; }
            public virtual bool HandleOnUpdate(TRuntime runtime) { return true; }
            public virtual bool HandleOnExit(TRuntime runtime) { return true; }
        }

        public abstract PhaseData GetPhaseData(int i);

        public abstract TRuntime CreateRuntime(TPerformer actionPerformerModule);
    }
}