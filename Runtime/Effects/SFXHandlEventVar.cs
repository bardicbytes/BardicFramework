using BardicBytes.BardicFramework.Core;
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Core.Prefixes.Effects+"SFX Handle")]
    public class SFXHandleEventVar : GenericEventVar<SoundEffect.ActiveHandle> 
    {
        public override SoundEffect.ActiveHandle To(EventVarInstanceField bc) => (SoundEffect.ActiveHandle)bc.SystemObjectValue;
    }
}