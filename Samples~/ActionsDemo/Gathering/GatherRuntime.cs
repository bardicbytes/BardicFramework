using BB.BardicFramework.Actions;

namespace BB.BardicSamples.Demo
{
    [System.Serializable]
    public class GatherRuntime : ActionRuntime<GatherAction, GatherPerformer, GatherRuntime>
    {
        public GatherRuntime(GatherAction action, GatherPerformer actionPerformer) : base(action, actionPerformer) { }
    }
}