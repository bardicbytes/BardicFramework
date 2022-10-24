using BardicBytes.BardicFramework.Core;
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Core.Prefixes.Effects + "Event Var: Special Effect Handle")]
    public class SpecialEffectHandleEvent : GenericEventVar<SpecialEffect.ActiveHandle>
    {
        public override SpecialEffect.ActiveHandle To(EventVarInstanceField bc) => (SpecialEffect.ActiveHandle)bc.SystemObjectValue;
    }
}