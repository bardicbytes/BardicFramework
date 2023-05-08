// alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;

namespace BardicBytes.BardicFramework.Effects
{
    [CreateAssetMenu(menuName = Prefixes.Effects + "SFX Handle")]
    public class SFXHandleEventVar : GenericSystemObjectEventVar<SoundEffect.ActiveHandle>
    {
    }
}