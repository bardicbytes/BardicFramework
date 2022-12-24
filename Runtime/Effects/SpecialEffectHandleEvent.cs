//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "Event Var: Special Effect Handle")]
    public class SpecialEffectHandleEvent : SimpleGenericEventVar<SpecialEffect.ActiveHandle>
    {
        public override SpecialEffect.ActiveHandle To(EventVars.EVInstData bc) => (SpecialEffect.ActiveHandle)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(SpecialEffect.ActiveHandle val, EventVars.EVInstData config) => config.SystemObjectValue = val;
#endif
    }
}