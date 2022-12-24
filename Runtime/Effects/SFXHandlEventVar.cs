//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects+"SFX Handle")]
    public class SFXHandleEventVar : SimpleGenericEventVar<SoundEffect.ActiveHandle> 
    {
        public override SoundEffect.ActiveHandle To(EventVars.EVInstData bc) => (SoundEffect.ActiveHandle)bc.SystemObjectValue;
#if UNITY_EDITOR
        protected override void SetInitialvalueOfInstanceConfig(SoundEffect.ActiveHandle val, EventVars.EVInstData config)
        {
            config.SystemObjectValue = val;
        }
#endif
    }
}