//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "Event Var: Special Effect Handle")]
    public class SpecialEffectHandleEvent : GenericSystemObjectEventVar<SpecialEffect.ActiveHandle>
    {
    }
}