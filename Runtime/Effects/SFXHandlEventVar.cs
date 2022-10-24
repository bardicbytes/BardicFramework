//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEngine;

namespace BB.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects+"SFX Handle")]
    public class SFXHandleEventVar : GenericEventVar<SoundEffect.ActiveHandle> 
    {
        public override SoundEffect.ActiveHandle To(EventVarInstanceField bc) => (SoundEffect.ActiveHandle)bc.SystemObjectValue;
    }
}