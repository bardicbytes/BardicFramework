using BardicBytes.BardicFramework;
using BardicBytes.BardicFramework.Actions;
using UnityEngine;

namespace BardicBytes.BardicSamples.Demo
{
    [CreateAssetMenu(menuName = Prefixes.BardicBase+"Demo/Gather Action")]
    public class GatherAction : GenericAction<GatherAction, GatherPerformer, GatherRuntime>
    {
        public class GatherPhaseData : GatherAction.PhaseData
        {

        }
        public override int PhaseDataCount => 1;

        public override PhaseData GetPhaseData(int i)
        {
            return new GatherPhaseData();
        }

        public override GatherRuntime CreateRuntime(GatherPerformer actionModule)
        {
            throw new System.NotImplementedException();
        }
    }
}