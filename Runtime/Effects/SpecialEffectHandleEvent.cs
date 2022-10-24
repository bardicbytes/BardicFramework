//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;

namespace BB.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "Event Var: Special Effect Handle")]
    public class SpecialEffectHandleEvent : GenericEventVar<SpecialEffect.ActiveHandle>
    {
        public override SpecialEffect.ActiveHandle To(EventVarInstanceField bc) => (SpecialEffect.ActiveHandle)bc.SystemObjectValue;
    }
}