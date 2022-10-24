using BB.BardicFramework.Actions;
using UnityEngine;

namespace BB.BardicFramework.Demo
{
    [CreateAssetMenu(menuName = Prefixes.BardicBase+"Demo/Gather Action")]
    public class GatherAction : Action<GatherAction, GatherPerformer, GatherRuntime>
    {
        public class GatherPhaseData : GatherAction.PhaseData
        {

        }
        public override int PhaseDataCount => 1;

        public override PhaseData GetPhaseData(int i)
        {
            return new GatherPhaseData();
        }

        public override GatherRuntime GetRuntime(GatherPerformer actionModule)
        {
            throw new System.NotImplementedException();
        }
    }
}